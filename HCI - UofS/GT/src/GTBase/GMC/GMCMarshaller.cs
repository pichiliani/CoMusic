//
// GT: The Groupware Toolkit for C#
// Copyright (C) 2006 - 2009 by the University of Saskatchewan
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later
// version.
// 
// This library is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA
// 02110-1301  USA
// 

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.IO.Compression;
using System.Diagnostics;
using Common.Logging;
using GT.Net;
using GT.Utils;

// An adapative message marshaller.  This marshaller implements
// the general message compressor as described in:
// <blockquote>
// C Gutwin, C Fedak, M Watson, J Dyck, T Bell (2006).  Improving
// network efficiency in real-time groupware with general message
// compression.  In Proc of the Conference on Computer Supported
// Cooperative Work (CSCW), 119--128.
// &lt;doi:10.1145/1180875.1180894&gt;
// &lt;http://hci.usask.ca/publications/2006/compression.pdf&gt;
// </blockquote>
namespace GT.GMC
{

    /// <summary>
    /// A facade for performing compression and decompression.
    /// GMC has two parts: a template compressor that finds repeated structure and
    /// syntax elements between messages, and a within-message compressor that uses
    /// Ziv-Lempel to reduce the size of large messages.  The template compressor
    /// examines messages to find repeated sequences, which are assigned a code
    /// and entered into a dictionary; these codes are then used to replace other
    /// occurrences of the sequence in future messages.  The template compression
    /// takes three parts: identifying and creating a template, compressing the
    /// message, and announcing any new templates.
    /// </summary>
    /// <remarks>
    /// Note: the GMCMarshaller uses its own message container format, and does not 
    /// use the LWMCF.
    /// </remarks>
    public class GMCMarshaller : IMarshaller
    {
        /// <summary>
        /// Special key to indicate content type
        /// </summary>
        private enum GMCWithinMessageCompression
        {
            None = 71,          // ASCII G: no within-message compression
            Deflated = 68,      // ASCII D: deflated
        }

        /// <summary>
        /// Special key to indicate types of content value
        /// </summary>
        private enum GMCMessageKey
        {
            Template = 0,
            HuffmanFrequencyTable = 1,
            Announcements = 2,
            Huffed = 3,
            NotHuffed = 4,
            Message = 5
        }

        /// <summary>
        /// GMC operates on the results from a different marshaller.
        /// This sub-marshaller is used to transform objects, etc. to bytes.
        /// </summary>
        private readonly IMarshaller subMarshaller;

        /// <summary>
        /// The compressor used for messages being sent by this user.
        /// </summary>
        private readonly GeneralMessageCompressor compressor;

        /// <summary>
        /// The per-user decompressors used to decompress messages from other users
        /// (as keyed by a unique identifier).  Decompressors require being notified
        /// of the templates and announcements (new dictionary entries) from the other
        /// users' systems.
        /// </summary>
        private readonly Dictionary<int, GeneralMessageCompressor> decompressors; // client Identity -> GeneralMessageCompressor

        /// Statistics
        public long totalUncompressedBytes = 0;
        public long totalCompressedBytes = 0;
        public long totalTemplateBytes = 0;
        public long totalAnnouncementBytes = 0;
        public long totalFrequenciesBytes = 0;
        public long numberDeflatedMessages = 0;
        public long totalDeflatedBytesSaved = 0;

        protected ILog log;

        /// <summary>Creates a new instance of GMC.  
        /// Typically GMC will function as a singleton for each client, 
        /// however, there are cases where the designer may wish to have a variety of GMCS.</summary>
        /// <param name="subMrshlr">the submarshaller to use for marshalling objects to bytes.</param>
        public GMCMarshaller(IMarshaller subMrshlr)
        {
            log = LogManager.GetLogger(GetType());

            subMarshaller = subMrshlr;
            compressor = new GeneralMessageCompressor();
            decompressors = new Dictionary<int, GeneralMessageCompressor>();
        }

        public string Descriptor
        {
            get
            {
                string subdescriptor = subMarshaller.Descriptor;
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < subdescriptor.Length; i++)
                {
                    sb.Append(Char.ConvertFromUtf32(Char.ConvertToUtf32(subdescriptor, i) ^ 71));
                }
                return sb.ToString();
            }
        }

        public bool IsCompatible(string marshallingDescriptor, ITransport remote)
        {
            return Descriptor.Equals(marshallingDescriptor);
        }


        public IMarshalledResult Marshal(int senderIdentity, Message message, ITransportDeliveryCharacteristics tdc)
        {
            IMarshalledResult mr = subMarshaller.Marshal(senderIdentity, message, tdc);
            MarshalledResult result = new MarshalledResult();
            while (mr.HasPackets)
            {
                TransportPacket packet = mr.RemovePacket();
                byte[] encoded = Encode(packet.ToArray());
                // FIXME: this is a bit awkward: if encoded is small, then
                // we're likely better off copying these into a contiguous
                // block of memory.  But if big, then we're probably better
                // off using these byte arrays as the backing store.
                TransportPacket newPacket = new TransportPacket(
                    DataConverter.Converter.GetBytes(senderIdentity),
                    ByteUtils.EncodeLength((uint)encoded.Length),
                    encoded);
                result.AddPacket(newPacket);
            }
            mr.Dispose();
            return result;
        }

        public void Unmarshal(TransportPacket packet, ITransportDeliveryCharacteristics tdc, EventHandler<MessageEventArgs> messageAvailable)
        {
            Debug.Assert(messageAvailable != null, "callers must provide a messageAvailable handler");
            Stream input = packet.AsReadStream();
            int encoderId = DataConverter.Converter.ToInt32(ByteUtils.Read(input, 4), 0);
            uint length = ByteUtils.DecodeLength(input);
            byte[] decoded = Decode(encoderId, ByteUtils.Read(input, length));
            TransportPacket subPacket = TransportPacket.On(decoded);
            input.Close();
            subMarshaller.Unmarshal(subPacket, tdc,
                (sender, mea) => messageAvailable(this, mea));
        }

        public void Dispose()
        {
            subMarshaller.Dispose();
        }

        /// <summary>
        /// Removes the decompressors for a certain userID
        /// </summary>
        /// <param name="userID">The userID to remove</param>
        public void RemoveUserID(int userID)
        {
            decompressors.Remove(userID);
        }

        /// <summary>
        /// Encode (compress) the provided byte array.
        /// </summary>
        /// <param name="bytes">the bytes to be compressed</param>
        /// <returns>a compressed message package that holds the compressed message along
        /// with any updates required for its processing.</returns>
        public byte[] Encode(byte[] bytes)
        {
            log.Trace(">>>> ENCODING <<<<<");
            totalUncompressedBytes += bytes.Length;

            CompressedMessagePackage cmp = compressor.Encode(bytes);
            MemoryStream result = new MemoryStream();
            log.Trace("==> Encoding for template " + cmp.TemplateId);

            // We first construct a marshalled version with no within-message 
            // compression (e.g., no deflation).  We then pass this version into 
            // TryDeflating() to try whatever standard within-message compression techniques.
            // note: must remember to replace/strip-off this first byte in TryDeflating() 
            result.WriteByte((byte)GMCWithinMessageCompression.None);

            ByteUtils.EncodeLength((uint)cmp.TemplateId, result);
            if (cmp.Template != null)
            {
                if(log.IsTraceEnabled)
                {
                    log.Trace(String.Format("TID={0}: Encoding template: {1}", cmp.TemplateId,
                        ByteUtils.DumpBytes(cmp.Template)));
                }
                result.WriteByte((byte)GMCMessageKey.Template);
                EncodeTemplate(cmp.Template, result);
            }
            result.WriteByte((byte)(cmp.Huffed ? GMCMessageKey.Huffed : GMCMessageKey.NotHuffed));
            if (cmp.FrequencyTable != null)
            {
                if (log.IsTraceEnabled)
                {
                    log.Trace(String.Format("TID={0}: Encoding frequency table", cmp.TemplateId));
                }
                result.WriteByte((byte)GMCMessageKey.HuffmanFrequencyTable);
                EncodeFrequencyTable(cmp.FrequencyTable, result);
            }
            if (cmp.Announcements != null && cmp.Announcements.Count > 0)
            {
                if (log.IsTraceEnabled)
                {
                    StringBuilder message = new StringBuilder();
                    message.Append(String.Format("TID={0}: Encoding {1} announcements:",
                        cmp.TemplateId, cmp.Announcements.Count));
                    foreach(KeyValuePair<uint, byte> kvp in cmp.Announcements)
                    {
                        message.Append(' ');
                        message.Append(kvp.Key);
                        message.Append("->");
                        message.Append(kvp.Value);
                    }
                    log.Trace(message);
                }
                result.WriteByte((byte)GMCMessageKey.Announcements);
                EncodeAnnouncements(cmp.Announcements, result);
            }
            if (cmp.Message != null)    // this could be useful one day...
            {
                if (log.IsTraceEnabled)
                {
                    log.Trace(String.Format("Encoding message: {0}",
                        ByteUtils.DumpBytes(cmp.Message)));
                }
                result.WriteByte((byte)GMCMessageKey.Message);
                ByteUtils.EncodeLength((uint)cmp.Message.Length, result);
                result.Write(cmp.Message, 0, cmp.Message.Length);
            }

            bytes = TryDeflating(result.ToArray());
            totalCompressedBytes += bytes.Length;
            return bytes;
        }

        /// <summary>
        /// Try compressing the provided uncompressed packet.  Return the best
        /// result; this may actually be the uncompressed packet itself.
        /// </summary>
        /// <param name="uncompressedPacket">the uncmpressed packet</param>
        /// <returns>the possibly-compressed packet</returns>
        private byte[] TryDeflating(byte[] uncompressedPacket)
        {
            MemoryStream output = new MemoryStream();
            output.WriteByte((byte)GMCWithinMessageCompression.Deflated);

            MarshallingException.Assert(uncompressedPacket[0] == (byte)GMCWithinMessageCompression.None,
                "bytes are supposed to be uncompressed");
            // I guess ideally we could support several compression techniques here

            DeflateStream deflated = new DeflateStream(output, CompressionMode.Compress, true);
            // must strip off the first byte, which indicates no compression
            deflated.Write(uncompressedPacket, 1, uncompressedPacket.Length - 1);
            deflated.Flush();
            deflated.Close();
            // If it makes no difference, return the original uncompressed bytes
            if (uncompressedPacket.Length <= output.Length) { return uncompressedPacket; }
            if(log.IsTraceEnabled)
            {
                log.Trace(
                    String.Format(
                        "Deflating message: originally {0} bytes, deflated {1} bytes ({2}%)",
                        uncompressedPacket.Length, output.Length,
                        ((float)output.Length / (float)uncompressedPacket.Length)));
            }
            numberDeflatedMessages++;
            totalDeflatedBytesSaved += uncompressedPacket.Length - output.Length;
            return output.ToArray();
        }


        /// <summary>
        /// Examine potentially-compressed packet and apply its decompression
        /// technique.  Assumes entire contents of bytes forms the packet.
        /// </summary>
        /// <param name="bytes">the possibly-compressed packet</param>
        /// <returns>a stream with the definitely-decompressed contents</returns>
        private Stream TryInflating(byte[] bytes)
        {
            MemoryStream input = new MemoryStream(bytes);
            switch ((GMCWithinMessageCompression)input.ReadByte())
            {
                case GMCWithinMessageCompression.None:
                    return input;
                case GMCWithinMessageCompression.Deflated:
                    return new DeflateStream(input, CompressionMode.Decompress, false);
            }
            throw new MarshallingException("unknown within-message compression indicator");
        }


        private void EncodeTemplate(byte[] tmpl, Stream output)
        {
            long p = output.Position;

            ByteUtils.EncodeLength((uint)tmpl.Length, output);
            output.Write(tmpl, 0, tmpl.Length);

            totalTemplateBytes += output.Position - p;
        }

        private void EncodeAnnouncements(Dictionary<uint, byte> dictionary, Stream output)
        {
            long p = output.Position;
            
            ByteUtils.EncodeLength((uint)dictionary.Count, output);
            byte[] result = new byte[4];
            foreach (uint longForm in dictionary.Keys)
            {
                DataConverter.Converter.GetBytes(longForm).CopyTo(result, 0);
                output.Write(result, 0, 4);
                output.WriteByte(dictionary[longForm]);
            }

            totalAnnouncementBytes += output.Position - p;
        }

        private void EncodeFrequencyTable(uint[] frequencies, Stream output)
        {
            long p = output.Position;

            Debug.Assert(frequencies.Length == 256);
            for (int j = 0; j < 256; j++)
            {
                output.WriteByte((byte)((frequencies[j] >> 24) & 0xFF));
                output.WriteByte((byte)((frequencies[j] >> 16) & 0xFF));
                output.WriteByte((byte)((frequencies[j] >> 8) & 0xFF));
                output.WriteByte((byte)((frequencies[j]) & 0xFF));
            }

            totalFrequenciesBytes += output.Position - p;
        }

        /// <summary>
        /// Decode the provided byte array received from the user with unique
        /// identity <c>userId</c>.
        /// </summary>
        /// <param name="userId">the unique identity for the user</param>
        /// <param name="encodedBytes">the encoded message received</param>
        /// <returns></returns>
        public byte[] Decode(int userId, byte[] encodedBytes)
        {
            log.Trace("\n>>>> DECODING <<<<<");
            Stream input = TryInflating(encodedBytes);
            CompressedMessagePackage cmp = new CompressedMessagePackage();
            cmp.TemplateId = (short)ByteUtils.DecodeLength(input);
            log.Trace(String.Format("==> Decoding with template {0}", cmp.TemplateId));
            int inputValue;
            while((inputValue = input.ReadByte()) != -1) {
                switch ((GMCMessageKey)inputValue)
                {
                case GMCMessageKey.Huffed: cmp.Huffed = true; break;
                case GMCMessageKey.NotHuffed: cmp.Huffed = false; break;
                case GMCMessageKey.Template:
                    cmp.Template = DecodeTemplate(input);
                    if (log.IsTraceEnabled)
                    {
                        log.Trace(String.Format("TID={0}: Decoded template: {1}", 
                            cmp.TemplateId, ByteUtils.DumpBytes(cmp.Template)));
                    }
                    break;
                case GMCMessageKey.HuffmanFrequencyTable:
                    cmp.FrequencyTable = DecodeFrequencies(input);
                    log.Trace(String.Format("TID={0}: Decoded huffman frequencies", cmp.TemplateId));
                    break;
                case GMCMessageKey.Announcements:
                    cmp.Announcements = DecodeAnnouncements(input);
                    if (log.IsTraceEnabled)
                    {
                        StringBuilder message = new StringBuilder();
                        message.Append(String.Format("TID={0}: Decoded {1} dictionary announcements:",
                                cmp.TemplateId, cmp.Announcements.Count));
                        foreach(KeyValuePair<uint, byte> kvp in cmp.Announcements)
                        {
                            message.Append(' ');
                            message.Append(kvp.Key);
                            message.Append("->");
                            message.Append(kvp.Value);
                        }
                        log.Trace(message);
                    }
                    break;
                case GMCMessageKey.Message:
                    uint len = ByteUtils.DecodeLength(input);
                    cmp.Message = new byte[len];
                    input.Read(cmp.Message, 0, (int)len);
                    if (log.IsTraceEnabled)
                    {
                        log.Trace(String.Format("Decoded message: {0}",
                            ByteUtils.DumpBytes(cmp.Message)));
                    }
                    break;
                default:
                    log.Trace("Invalid GMC message");
                    throw new MarshallingException("invalid GMC message");
                }
            }

            GeneralMessageCompressor handler;
            if (!decompressors.TryGetValue(userId, out handler))
            {
                if (cmp.Template == null)
                {
                    //we haven't received any templates whatsoever from this person.
                    MissingInformationException mte = new MissingInformationException();
                    mte.Template = 0;
                    mte.UserID = userId;
                    mte.ExceptionType = EnumExceptionType.MissingTemplate;
                    throw mte;
                }
                handler = decompressors[userId] = new GeneralMessageCompressor();
            }
            return handler.Decode(cmp, userId);            
        }

        private Dictionary<uint, byte> DecodeAnnouncements(Stream input)
        {
            uint count = ByteUtils.DecodeLength(input);
            byte[] buffer = new byte[4];
            Dictionary<uint, byte> result = new Dictionary<uint,byte>();
            for (int i = 0; i < count; i++)
            {
                input.Read(buffer, 0, 4);
                result[DataConverter.Converter.ToUInt32(buffer, 0)] = (byte)input.ReadByte();
            }
            return result;
        }

        private byte[] DecodeTemplate(Stream input)
        {
            uint length = ByteUtils.DecodeLength(input);
            byte[] tmpl = new byte[length];
            input.Read(tmpl, 0, (int)length);
            return tmpl;
        }

        private uint[] DecodeFrequencies(Stream input)
        {
            uint[] result = new uint[256];
            for (int i = 0; i < 256; i++)
            {
                result[i] = (uint)input.ReadByte() << 24;
                result[i] |= (uint)input.ReadByte() << 16;
                result[i] |= (uint)input.ReadByte() << 8;
                result[i] |= (uint)input.ReadByte();
            }
            return result;
        }
    }
}

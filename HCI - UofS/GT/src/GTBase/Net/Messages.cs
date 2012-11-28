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
using System.Diagnostics;
using System.Text;
using GT.Utils;
namespace GT.Net
{
    #region Message Classes

    /// <remarks>A base GT message.</remarks>
    public abstract class Message
    {
        /// <summary>The channel carrying this message.</summary>
        virtual public byte ChannelId { get { return channelId; } }

        /// <summary>The type of message.</summary>
        virtual public MessageType MessageType { get { return type; } }

        protected byte channelId;
        protected MessageType type;

        /// <summary>Creates a new outbound message.</summary>
        /// <param name="channelId">The channel carrying this message.</param>
        /// <param name="type">The type of message.</param>
        protected Message(byte channelId, MessageType type)
        {
            this.channelId = channelId;
            this.type = type;
        }

        public override string ToString()
        {
            return GetType().Name + "(type:" + type + " channel:" + channelId + ")";
        }
    }

    /// <summary>
    /// A GT message containing byte content.
    /// </summary>
    public class BinaryMessage : Message
    {
        /// <summary>The binary byte content.</summary>
        public byte[] Bytes { get { return bytes; } }

        protected byte[] bytes;

        /// <summary>Creates a new outbound message.</summary>
        /// <param name="channelId">The channel carrying this message.</param>
        /// <param name="bytes">the contents</param>
        public BinaryMessage(byte channelId, byte[] bytes)
            : base(channelId, MessageType.Binary)
        {
            this.bytes = bytes;
        }

        public override string ToString()
        {
            StringBuilder result = new StringBuilder(GetType().Name);
            result.Append(": ");
            result.Append(bytes.LongLength);
            result.Append(" bytes");
            for (int i = 0; i < Math.Min(bytes.Length, 128); i += 16)
            {
                result.Append("\n  ");
                result.Append(i.ToString("X3"));
                result.Append(": ");
                result.Append(ByteUtils.DumpBytes(bytes, i, 16));
                result.Append(ByteUtils.AsPrintable(bytes, i, 16));
            }
            if (bytes.Length > 128) { result.Append("\n..."); }
            return result.ToString();
        }
    }

    /// <summary>
    /// A GT message containing string content.
    /// </summary>
    public class StringMessage : Message
    {
        /// <summary>The string text.</summary>
        public string Text { get { return text; } }

        protected string text;

        public StringMessage(byte channelId, string text)
            : base(channelId, MessageType.String)
        {
            this.text = text;
        }

        public override string ToString()
        {
            return GetType().Name + "(type:" + type + " channel:" + channelId + " text:\"" + text + "\")";
        }

    }

    /// <summary>
    /// A GT message containing an object as content.
    /// </summary>
    public class ObjectMessage : Message
    {
        /// <summary>The message's object.</summary>
        public object Object { get { return obj; } }

        protected object obj;

        public ObjectMessage(byte channelId, object obj)
            : base(channelId, MessageType.Object)
        {
            this.obj = obj;
        }

        public override string ToString()
        {
            return GetType().Name + "(type:" + type + " channel:" + channelId + " object:\"" + obj + "\")";
        }
    }

    /// <summary>A message from a session about a particular client</summary>
    public class SessionMessage : Message
    {
        /// <summary>What occurred on the session.</summary>
        public SessionAction Action { get { return action; } }

        /// <summary>Which client was affected.</summary>
        public int ClientId { get { return clientId; } }

        protected SessionAction action;
        protected int clientId;

        /// <summary>Create a new SessionMessage</summary>
        /// <param name="channelId">The channel carrying this message.</param>
        /// <param name="clientId">The subject of the session action.</param>
        /// <param name="e">The session action.</param>
        public SessionMessage(byte channelId, int clientId, SessionAction e)
            : base(channelId, MessageType.Session)
        {
            this.clientId = clientId;
            this.action = e;
        }

        public override string ToString()
        {
            return String.Format("Client {0}: {1}", clientId, action);
        }
    }

    /// <summary>
    /// A GT control message.  System messages aren't sent
    /// on a channel; the descriptor (the type of system message) is 
    /// instead encoded as the channelId.
    /// </summary>
    public class SystemMessage : Message
    {
        /// <summary>Create a new SystemMessage</summary>
        public SystemMessage(SystemMessageType t)
            : base((byte)t, MessageType.System)
        {
        }

        /// <summary>
        /// System messages aren't carried on a channel.  But some code assumes
        /// that all messages have a channelId.  So return a valid value.
        /// </summary>
        public override byte ChannelId { get { return 0; } }

        /// <summary>
        /// Return the system message descriptor.  System messages aren't sent
        /// on a channel; the descriptor is instead encoded as the channelId.
        /// </summary>
        public SystemMessageType Descriptor { get { return (SystemMessageType)channelId; } }

        public override string ToString()
        {
            StringBuilder result = new StringBuilder();
            result.Append(GetType().Name);
            result.Append('(');
            result.Append(Descriptor);
            string data = DataString();
            if (data.Length > 0)
            {
                result.Append(':');
                result.Append(data);
            }
            result.Append(')');
            return result.ToString();
        }

        protected virtual string DataString()
        {
            return "";
        }
    }

    /// <summary>
    /// A system message either requesting or responding to a ping.
    /// This is primarily intended for internal use.
    /// </summary>
    public class SystemPingMessage : SystemMessage
    {
        public uint Sequence { get; set; }
        public int SentTime { get; set; }

        public SystemPingMessage(SystemMessageType mt, uint sequence, int sentTime)
            : base(mt)
        {
            Debug.Assert(mt == SystemMessageType.PingRequest || mt == SystemMessageType.PingResponse);
            Sequence = sequence;
            SentTime = sentTime;
        }

        protected override string DataString()
        {
            return "seq=" + Sequence + " sent=" + SentTime;
        }
    }

    /// <summary>
    /// A system message carrying a connexion's identity.
    /// </summary>
    public class SystemIdentityResponseMessage : SystemMessage
    {
        public int Identity { get; set; }

        public SystemIdentityResponseMessage(int identity) : base(SystemMessageType.IdentityResponse)
        {
            Identity = identity;
        }

        protected override string DataString()
        {
            return "id=" + Identity;
        }
    }

    #endregion

    /// <summary>
    /// Carries all the details necessary for sending a message.
    /// Not intended for public use.
    /// </summary>
    public class PendingMessage
    {
        public PendingMessage() {}

        public PendingMessage(Message m, MessageDeliveryRequirements mdr, ChannelDeliveryRequirements cdr)
        {
            Message = m;
            MDR = mdr;
            CDR = cdr;
        }

        public Message Message { get; set; }
        public MessageDeliveryRequirements MDR { get; set; }
        public ChannelDeliveryRequirements CDR { get; set; }

        public void Clear()
        {
            Message = null;
            MDR = null;
            CDR = null;
        }

        public override string ToString()
        {
            return "PM{" + GetHashCode() + "}: " + Message;
        }
    }
}

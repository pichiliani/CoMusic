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
using System.Drawing;
using System.Windows.Forms;
using GT.Net;
using GT.UI;

namespace Telepointers
{
    /// <summary>
    /// A simple demonstration of using a streamed tuple to communicate
    /// telepointer presence messages.  The local mouse pointers is propagated
    /// automatically on a periodic basis.
    /// </summary>
    public partial class Form1 : Form
    {
        /// <summary>
        /// Use channel #2 for sending and receiving object updates
        /// </summary>
        private const int ObjectChannelId = 2;
        
        /// <summary>
        /// Use channel #1 for sending and receiving telepointer updates
        /// </summary>
        private const int TelepointersChannelId = 1;

        /// <summary>
        /// The client repeater uses channel #0 by default to send updates
        /// on clients joining or leaving the group.
        /// </summary>
        private const int SessionUpdatesChannelId = 0;

        private Client client;

        /// <summary>
        /// Used to send and receive updated telepointer coordinates.
        /// </summary>
        private IStreamedTuple<int, int> coords;

        /// <summary>
        /// Receives session updates from the client repeater
        /// when clients join or leave the group.
        /// </summary>
        private ISessionChannel updates;


        /// <summary>
        /// Objecto que vai ser utilizado para enviar e receber coisas
        /// </summary>
        private  IObjectChannel objts;

        /// <summary>
        /// The list of current clients and their respective telepointers.
        /// </summary>
        private Dictionary<int, Telepointer> telepointers = new Dictionary<int, Telepointer>();

        public int ContaEnviadas = 0;
        public int ContaRecebidas= 0;

        private ClienteGT cGT;

        // Seta as configurações no forma
        #region Form1
        public Form1()
        {
            InitializeComponent();

            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.DoubleBuffer, true);
        }
        #endregion

        #region Form1_Load
        private void Form1_Load(object sender, EventArgs e)
        {
            InputDialog d = new InputDialog("Connection details", "Which server:port ?", "localhost:9999");
            if (d.ShowDialog() != DialogResult.OK)
            {
                throw new InvalidOperationException();
            }
            string[] parts = d.Input.Split(':');
            string host = parts[0];
            string port = parts.Length > 1 ? parts[1] : "9999";

            // Aqui vou colocar a chamada para o cliente GT!
            // cGT = new ClienteGT(host, port);
            
            // Set up GT
            client = new Client(new DefaultClientConfiguration());
            client.ErrorEvent += es => Console.WriteLine(es);
            
            // Evento do client
            client.ConnexionRemoved += client_ConnexionRemoved;
            client.Start();

            // Evento do client
            client.MessageSent += new MessageHandler(this.MensagemEnviada);

            updates = client.OpenSessionChannel(host, port, SessionUpdatesChannelId,
                ChannelDeliveryRequirements.SessionLike);

            // Evento do updates
            updates.MessagesReceived += updates_SessionMessagesReceived;
            
            // coords armazena os dados recebidos (é a tupla)
           // coords = client.OpenStreamedTuple<int, int>(host, port, TelepointersChannelId,
           //     TimeSpan.FromMilliseconds(50),
           //     ChannelDeliveryRequirements.AwarenessLike); 

            coords = client.OpenStreamedTuple<int, int>(host, port, TelepointersChannelId,
                TimeSpan.FromMilliseconds(50),
                ChannelDeliveryRequirements.AwarenessLike); 

            

            // Evento do coords
            coords.StreamedTupleReceived += coords_StreamedTupleReceived;

            // coords.Identity 
             
            // Utilizar o OpenObjectChannel para enviar um objeto ao invés de enviar uma tupla
            // objts = client.OpenObjectChannel(host, port, ObjectChannelId, ChannelDeliveryRequirements.AwarenessLike);
            objts = client.OpenObjectChannel(host, port, ObjectChannelId, ChannelDeliveryRequirements.CommandsLike);
            
            objts.MessagesReceived += new Action<IObjectChannel>(objts_MessagesReceived); 

            
        }
        #endregion

        // Evento de Mensagem recebida para objetos genéricos
        #region objts_MessagesReceived
        void objts_MessagesReceived(IObjectChannel obj)
        {
            // objts.Messages 

            // label4.Text = "Qtd:" + objts.Messages.Count.ToString();

            Object o;

            while ((o = objts.DequeueMessage(0)) != null)
            {
                System.Collections.ArrayList l = (System.Collections.ArrayList)o;

                int x = (int)l[0];
                int y = (int)l[1];

                label3.Text = "X: " + x.ToString() + " Y: " + y.ToString();

                ContaRecebidas++;
                this.label2.Text = "Recebidas:" + ContaRecebidas.ToString();
            }



            // objts.DequeueMessage(0)

            // throw new NotImplementedException();
        }
        #endregion

        // Evento de Mensagem enviada para objetos genéricos
        #region MensagemEnviada
        private void MensagemEnviada(GT.Net.Message a, IConnexion b, ITransport c)
        {

            if ((a.MessageType.Equals(MessageType.Object)) && (a.ChannelId.Equals(2)))
            {
                ContaEnviadas++;
                this.label1.Text = "Enviadas:" + ContaEnviadas.ToString() + " de:" + coords.Identity.ToString();
            }
            else
            {
                // ContaEnviadas++;
                // this.label1.Text = "Enviadas:" + ContaEnviadas.ToString() + " de:" + coords.Identity.ToString();
            }
        }
        #endregion

        // Evento para receber conexões perdidas
        #region client_ConnexionRemoved
        private void client_ConnexionRemoved(Communicator c, IConnexion conn)
        {
            if(!IsDisposed && client.Connexions.Count == 0)
            {
                MessageBox.Show(this, "Disconnected from server", Text);
                Close();
            }
        }
        #endregion

        // Evento para receber mensagens (entrou, saiu, inativo e vivo)
        #region updates_SessionMessagesReceived
        /// <summary>
        /// Process an update about my fellows
        /// </summary>
        /// <param name="channel"></param>
        private void updates_SessionMessagesReceived(ISessionChannel channel)
        {
            SessionMessage m;
            while ((m = channel.DequeueMessage(0)) != null)
            {
                Console.WriteLine("Session: " + m);
                if (m.Action == SessionAction.Left)
                {
                    telepointers.Remove(m.ClientId);
                    Redraw();
                }
            }
        }
        #endregion

        // Evento que indica que recebeu a tupla 
        #region coords_StreamedTupleReceived
        /// <summary>
        /// Update telepointers for some client (may be my own!)
        /// </summary>
        /// <param name="tuple"></param>
        /// <param name="clientId"></param>
        private void coords_StreamedTupleReceived(RemoteTuple<int, int > tuple, int clientId)
        {
            if (!telepointers.ContainsKey(clientId))
            {
                // Ensure we're different
                Color tpc = clientId == coords.Identity ? Color.Red : Color.Blue;
                telepointers.Add(clientId, new Telepointer(tpc));
            }

            // int y = (int)tuple.Y[0];
            
            telepointers[clientId].Update(tuple.X, tuple.Y);

            if (!telepointers[clientId].getColor().Equals(Color.Red))
            {

             //  ContaRecebidas++;
             //  this.label2.Text = "Recebidas:" + ContaRecebidas.ToString() + " de:" + clientId.ToString();

                // this.label2.Text = "Recebido:" + DateTime.Now.ToLongTimeString() + " " + DateTime.Now.Millisecond.ToString();
            }

        }
        #endregion

        #region Form1_MouseMoved
        /// <summary>
        /// Cause the streamed tuple to be updated on local mouse movement.
        /// This value is only stored locally, and only propagated to the other 
        /// clients every 50 ms as defined by the tuple stream creation in the
        /// <see cref="IStreamedTuple{T_X,T_Y}.UpdatePeriod"/>
        /// <see cref="Form1">constructor</see>.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_MouseMoved(object sender, MouseEventArgs e)
        {
            
            
            coords.X = e.X;
            coords.Y = e.Y;
            
            // this.label1.Text = "Enviado:" + DateTime.Now.ToLongTimeString() + " " + DateTime.Now.Millisecond.ToString() ;

            System.Collections.ArrayList l = new System.Collections.ArrayList();
            

            l.Add(e.X);
            l.Add(e.Y);

            objts.Flush();
            objts.Send(l);
            objts.Flush();
            
        }
        #endregion
                        
        #region Redraw
        private void Redraw()
        {
            BeginInvoke(new MethodInvoker(Invalidate));
        }
        #endregion

        #region Form1_Paint
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.Clear(Color.SeaShell);
            foreach (int i in telepointers.Keys)
            {
                
                telepointers[i].Draw(g);
            }
        }
        #endregion

        #region timerRepaint_Tick
        private void timerRepaint_Tick(object sender, EventArgs e)
        {
            // timer starts on creation; client created only on form load
            if (client != null) { client.Update(); }
            Redraw();
        }
        #endregion

        #region Form1_FormClosed
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            client.Stop();
            client.Dispose();
        }
        #endregion

        #region class Telepointer
        /// <summary>
        /// A record for a particular telepointer.
        /// </summary>
        private class Telepointer
        {
            float x = 0, y = 0;
            readonly Color color;

            public Telepointer(Color color)
            {
                this.color = color;
            }

            public void Draw(Graphics g)
            {
                g.DrawRectangle(new Pen(color), x, y, 5, 5);
            }

            public void Update(float newX, float newY)
            {
                this.x = newX;
                this.y = newY;
            }

            public Color getColor()
            {
                return color;
            }


        }
        #endregion


    }
}

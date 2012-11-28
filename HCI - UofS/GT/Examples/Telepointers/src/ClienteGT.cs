using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Collections;

using GT.Net;
using GT.UI;

namespace Telepointers
{
    // A classe que vai agir como cliente para o toolkit GT
    class ClienteGT
    {
        // Canais
        private const int SessionUpdatesChannelId = 0;
        // private const int TelepointersChannelId = 1;
        private const int ObjectChannelId = 1;

        // Instância cliente
        private Client client;

        // Variáveis de comunicação:
        // private IStreamedTuple<int, int> coords;
        private ISessionChannel updates;
        private IObjectChannel objts;

        // Apenas controle
        private int ContaEnviadas = 0;
        private int ContaRecebidas = 0;
        // public SmallKeyboard.frmKeyboard form;

        #region ClienteGT
        public ClienteGT(string host, string port)
        {
            // this.form = f;

            // Set up GT
            client = new Client(new DefaultClientConfiguration());
            client.ErrorEvent += es => Console.WriteLine(es);
            // client.ErrorEvent += es => MessageBox.Show(es.ToString());

            // Evento do client
            client.ConnexionRemoved += client_ConnexionRemoved;
            client.Start();

            // Evento do client
            client.MessageSent += new MessageHandler(this.MensagemEnviada);

            // updates: controle de acesso à sessão
            updates = client.OpenSessionChannel(host, port, SessionUpdatesChannelId, ChannelDeliveryRequirements.SessionLike);

            // Evento do updates
            updates.MessagesReceived += updates_SessionMessagesReceived;

            // Utilizar o OpenObjectChannel para enviar objetos genéricos
            objts = client.OpenObjectChannel(host, port, ObjectChannelId, ChannelDeliveryRequirements.CommandsLike);

            objts.MessagesReceived += new Action<IObjectChannel>(objts_MessagesReceived);

        }
        #endregion

        // Evento de Mensagem recebida para objetos genéricos
        #region objts_MessagesReceived
        void objts_MessagesReceived(IObjectChannel obj)
        {
            Object desserializado;
            Object o;
            String nomeEvento;

            while ((desserializado = objts.DequeueMessage(0)) != null)
            {
                ArrayList list = (ArrayList)desserializado;

                o = list[0];
                nomeEvento = (String)list[1];

                #region mouseMovedPointer
                if (nomeEvento.Equals("mouseMovedPointer"))
                {

                    // ((frmKeyboard)this.Form).ContaTelefingerRemovido++;

                    // Recebe os dados do objeto de rede transportado
                    ArrayList dados_rec = (ArrayList)o;

                    // int pos =  int.Parse(dados_rec[2].ToString());

                    // this.Form.aTelepointers[pos].Visible = true;
                    // this.Form.aTelepointers[pos].Location = (Point)dados_rec[0];

                    // this.Form.OnMouseMoveImpl((Point) dados_rec[0], (Color) dados_rec[1], pos, dados_rec[3].ToString());


                    // Precisa utilizar delegates, pois existem problemas quando uma Thread que não é
                    // o formulário atualiza a interface gráfica
                   //  this.form.Invoke(new SmallKeyboard.frmKeyboard.DelegateGenerico(this.form.OnMouseMoveImpl), new object[] { dados_rec });

                }
                #endregion



                ContaRecebidas++;
            }
        }
        #endregion

        // Evento de Mensagem enviada para objetos genéricos
        #region MensagemEnviada
        private void MensagemEnviada(GT.Net.Message a, IConnexion b, ITransport c)
        {

            if ((a.MessageType.Equals(MessageType.Object)) && (a.ChannelId.Equals(2)))
            {
                ContaEnviadas++;
                // this.label1.Text = "Enviadas:" + ContaEnviadas.ToString() + " de:" + coords.Identity.ToString();
            }
        }
        #endregion

        // Evento para receber conexões perdidas
        #region client_ConnexionRemoved
        private void client_ConnexionRemoved(Communicator c, IConnexion conn)
        {
            MessageBox.Show("Disconnected from server");

            /* if (!IsDisposed && client.Connexions.Count == 0)
            {
                MessageBox.Show(this, "Disconnected from server", Text);
                Close();
            } */
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
                // Console.WriteLine("Session: " + m);
                if (m.Action == SessionAction.Left)
                {
                    //  telepointers.Remove(m.ClientId);
                    // Redraw();
                }
            }

        }
        #endregion

        // Envia o objeto
        #region EnviaObjGT
        public void EnviaObjGT(object o)
        {
            objts.Flush();
            objts.Send(o);
            objts.Flush();
        }
        #endregion

    }
}

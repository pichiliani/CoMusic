using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using SmallKeyboard;
using GT.Net;
using GT.UI;

namespace CoKeyboard.Collab
{
    // A classe que vai agir como cliente para o toolkit GT
    public class ClienteGT
    {
        // Canais
        private const int SessionUpdatesChannelId = 0;
        // private const int TelepointersChannelId = 1;
        private const int ObjectChannelId = 1;

        // Instância cliente
        public Client client;

        // Variáveis de comunicação:
        // private IStreamedTuple<int, int> coords;
        private ISessionChannel updates;
        private IObjectChannel objts;

        // Apenas controle
        private int ContaEnviadas = 0;
        private int ContaRecebidas = 0;
        public SmallKeyboard.frmKeyboard form;
        private string login;
        public string sessao;

        
        #region ClienteGT 
        public ClienteGT(string host, string port, SmallKeyboard.frmKeyboard f,string l)
        {
            this.form = f;
            this.login = l;

            // Set up GT
            client = new Client(new DefaultClientConfiguration());
            // client.ErrorEvent += es => Console.WriteLine(es);
            client.ErrorEvent += es => MessageBox.Show(es.ToString());

            // Evento do client
            client.ConnexionRemoved += client_ConnexionRemoved;
            client.Start();

            // Evento do client
            client.MessageSent += new MessageHandler(this.MensagemEnviada);

            // updates: controle de acesso à sessão
            updates = client.OpenSessionChannel(host, port, SessionUpdatesChannelId,ChannelDeliveryRequirements.SessionLike);

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
            String login_recebido;
            String sessao_recebida;

            while ((desserializado = objts.DequeueMessage(0)) != null)
            {
                ArrayList list = (ArrayList)desserializado;
   
                o = list[0];
                nomeEvento = (String)list[1];
                ArrayList credencial = (ArrayList)list[2];

                #region VerificaUserSessao
                // Aqui vou verificar se a mensagem recebida não foi mandanda
                // por mim mesmo e se a mensagem faz parte da mesma sessão
                login_recebido = (string)credencial[0];
                sessao_recebida = (string)credencial[1];

                if (login_recebido.Equals(this.login))
                    continue;
                if(!sessao_recebida.Equals(this.sessao))
                    continue;
                #endregion

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
                    this.form.Invoke(new SmallKeyboard.frmKeyboard.DelegateGenerico(this.form.OnMouseMoveImpl), new object[] { dados_rec });

                }
                #endregion

                #region fingerCreatePointer, fingerMovePointer ou fingerRemovePointer
                if (nomeEvento.Equals("fingerCreatePointer")
                    || nomeEvento.Equals("fingerMovePointer")
                    || nomeEvento.Equals("fingerRemovePointer"))
                {
                    ArrayList dados_rec = (ArrayList)o;
                    dados_rec.Add(nomeEvento);

                    // Precisa utilizar delegates, pois existem problemas quando uma Thread que não é
                    // o formulário atualiza a interface gráfica
                    this.form.Invoke(new frmKeyboard.DelegateGenerico(this.form.OnTelefingerImpl), new object[] { dados_rec });



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

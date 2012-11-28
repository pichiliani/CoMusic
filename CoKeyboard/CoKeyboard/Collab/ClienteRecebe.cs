using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections;
using System.Reflection;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using System.Windows.Forms;

namespace SmallKeyboard
{
	public class ClienteRecebe
	{
	    private Socket socket;
        ClienteRecebe cr;
        public frmKeyboard Form;
        public bool OKEnvio = false;

        #region ClienteRecebe()
        public ClienteRecebe(Socket s) 
        {
		    this.socket = s;
        }
        #endregion

        #region run()
        public void run ()  
        {
            
            System.Threading.Thread.CurrentThread.Priority = ThreadPriority.Highest;
    	    try 
            {

                bool clientTalking = true;
                
                //a loop that reads from and writes to the socket
                while (clientTalking) 
                {
            	    //get what client wants to say...
                    byte[] bytes = new byte[8192]; // 8 KB de dados
				    int bytesRec = this.socket.Receive(bytes);

                    String[] dados = new String[100];
                    String nomeEvento;
                    Object o = null;
                    
                    // System.Diagnostics.Debug.WriteLine("----- Recebido... -----" + DateTime.Now.ToString() );

                    String msg = Encoding.ASCII.GetString(bytes);

                    if( msg.IndexOf(";") >= 0)
                    {

                        dados = msg.Split(";".ToCharArray());
                        
                        // Retirando os bytes desnecessários do último elemento
                        nomeEvento = dados[0];
                        dados[dados.Length - 1] = dados[dados.Length - 1].Substring(0, dados[dados.Length - 1].IndexOf("\0"));
                    }
                    else
                    {
                        object desserializado = getObjectWithByteArray(bytes);          

                        if(desserializado == null)
                            continue;

                        ArrayList list = (ArrayList) desserializado;          
                        
                        o = list[0];
                        nomeEvento = (String) list[1];
                    }

                    OKEnvio = true;

                    #region PROT_atualiza_modelo_cliente
                    // EVENTOS DO ARGO
               	    if (nomeEvento.Equals("PROT_atualiza_modelo_cliente"))
            	    {
                        OKEnvio = true;
                    }
                    #endregion

                    #region PROT_atualiza_modelo_cliente_inicial
                    if (nomeEvento.Equals("PROT_atualiza_modelo_cliente_inicial"))
            	    {
                        ArrayList Users = (ArrayList) o;	
                        
                        // Cara elemento da ArrayList é um vetor com a sessão e os usuários

                        foreach(String[] x in Users)
                        {
                            
                            string sess = x[0]; // nome da seção
                            string[] us = x[1].Split("+".ToCharArray()); // lista de usuários e o nome de suas cores

                            foreach (String s in us)
                            {

                                string[] info = s.Split(",".ToCharArray());

                                // Mandando o nome do usuário para criar o seu teclado.
                                // Não preciso testar o nome da sessão, pois se entrou aqui já está na mesma sessão!
                                ArrayList aNome = new ArrayList();

                                aNome.Add(info[0]);  // Login
                                aNome.Add(info[1]);  // Nome da cor

                                this.Form.Invoke(new frmKeyboard.DelegateGenerico(this.Form.AddRemoteKeyboard), new object[] { aNome });
                                
                            }


                        }
                            
                        OKEnvio = true;

                    }
                    #endregion
                    
                    #region PROT_inicio_sessao
                    //  Recebeu a notificação que algum cliente entrou na da sessão!
               	    if (nomeEvento.Equals("PROT_inicio_sessao"))
            	    {
                        OKEnvio = true;
                        // Em o Tenho o nome do usuário que entrou!
                        // Aqui vou colocar o teclado do usuário remoto
                        ArrayList aNome = new ArrayList();

                        string login_cor = (String)o;
                        string[] info = login_cor.Split(",".ToCharArray());

                        aNome.Add(info[0]);
                        aNome.Add(info[1]);

                        this.Form.Invoke(new frmKeyboard.DelegateGenerico(this.Form.AddRemoteKeyboard), new object[] { aNome });

                        // MessageBox.Show("Entrou!");


                    }
                    #endregion

                    #region PROT_fim_sessao
                    //  Recebeu a notificação que algum cliente saiu da sessão!
               	    if (nomeEvento.Equals("PROT_fim_sessao"))
            	    {
                    }
                    #endregion

                    // Toca a nota no teclado 'fantasma'
                    #region PlayNote
                    if (nomeEvento.Equals("PlayNote"))
                    {
                        ArrayList dados_rec = new ArrayList();

                        // posição 0-> Nome do eevento
                        // posição 1-> Arquivo a ser tocado
                        // posição 2-> instrumento
                        // posição 3-> Login que enviou

                        dados_rec.Add(dados[1]);
                        dados_rec.Add(dados[2]);
                        dados_rec.Add(dados[3]);

                        this.Form.Invoke(new frmKeyboard.DelegateGenerico(this.Form.PlayImpl), new object[] { dados_rec });

                    }
                    #endregion

                    #region keyboardLocation
                    //  Recebeu a notificação que a localização do teclado remoto foi modificada!
                    if (nomeEvento.Equals("keyboardLocation"))
                    {
                        // Em o tenho primeiro o nome do login que mandou a posição do teclado 
                        //  e depois a posição do teclado
                        ArrayList dados_rec = (ArrayList)o;

                        this.Form.Invoke(new frmKeyboard.DelegateGenerico(this.Form.keyboardLocationImpl), new object[] { dados_rec });

                    }
                    #endregion

                    #region keyboardSize
                    //  Recebeu a notificação que o tamanho do teclado foi modificado!
                    if (nomeEvento.Equals("keyboardSize"))
                    {
                        // Em o tenho primeiro o nome do login que mandou a posição do teclado 
                        //  e depois a posição do teclado
                        ArrayList dados_rec = (ArrayList)o;

                        this.Form.Invoke(new frmKeyboard.DelegateGenerico(this.Form.keyboardSizeImpl), new object[] { dados_rec });

                    }
                    #endregion

                    #region mouseMovedPointer
                    if (nomeEvento.Equals("mouseMovedPointer"))
                    {

                        ((frmKeyboard)this.Form).ContaTelefingerRemovido++;

                        // Recebe os dados do objeto de rede transportado
                        ArrayList dados_rec = (ArrayList) o;
                     
                        // int pos =  int.Parse(dados_rec[2].ToString());
                        
                        // this.Form.aTelepointers[pos].Visible = true;
                        // this.Form.aTelepointers[pos].Location = (Point)dados_rec[0];

                        // this.Form.OnMouseMoveImpl((Point) dados_rec[0], (Color) dados_rec[1], pos, dados_rec[3].ToString());


                        // Precisa utilizar delegates, pois existem problemas quando uma Thread que não é
                        // o formulário atualiza a interface gráfica
                        this.Form.Invoke(new frmKeyboard.DelegateGenerico(this.Form.OnMouseMoveImpl), new object[] {dados_rec});

                    }
                    #endregion

                    #region fingerCreatePointer, fingerMovePointer ou fingerRemovePointer
                    if (   nomeEvento.Equals("fingerCreatePointer")
                        || nomeEvento.Equals("fingerMovePointer")
                        || nomeEvento.Equals("fingerRemovePointer"))
                    {
                        ArrayList dados_rec = (ArrayList)o;
                        dados_rec.Add(nomeEvento);

                        // Precisa utilizar delegates, pois existem problemas quando uma Thread que não é
                        // o formulário atualiza a interface gráfica
                        this.Form.Invoke(new frmKeyboard.DelegateGenerico(this.Form.OnTelefingerImpl), new object[] { dados_rec });



                    }
                    #endregion

                }
               	
            } 
            catch (ThreadAbortException  e) 
            {
                MessageBox.Show("ThreadAbortException in ClienteRecebe:" + e.Message  , 
			    Application.ProductName, 
			    MessageBoxButtons.OK, 
			    MessageBoxIcon.Warning);
                return;
            }
            catch (Exception e) 
            {
                MessageBox.Show("Exception in ClienteRecebe:" + e.Message  , 
			    Application.ProductName, 
			    MessageBoxButtons.OK, 
			    MessageBoxIcon.Warning);
            }
        }
        #endregion

        // Método que vai empacotar os dados antes deles serem enviados        
        #region getByteArrayWithObject()
        
        public static byte[] getByteArrayWithObject(Object o)
        {
            MemoryStream ms = new MemoryStream();
            BinaryFormatter bf1 = new BinaryFormatter();
            bf1.Serialize(ms, o);
            return ms.ToArray();
        }
        #endregion

        // Método que vai desempacotar os dados antes deles serem enviados        
        #region getObjectWithByteArray()
        public static object getObjectWithByteArray(byte[] theByteArray)
        {
            
            object ret = null;
            try
            {
                MemoryStream ms = new MemoryStream(theByteArray);
                BinaryFormatter bf1 = new BinaryFormatter();
                ms.Position = 0;

                ret = bf1.Deserialize(ms);

                return ret;
            }
            catch (Exception e)
            {
                return ret;
            }

        }
        #endregion



    }
}

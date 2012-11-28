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
using CoKeyboard.Collab;

namespace SmallKeyboard
{
	public class ClienteConecta
	{
        private String serverName;
        private int serverPort;
        private String lo; //login
        private String pass; //senhs
        private String sess; // sessão atual
    
        private Socket socket;

        public bool connected;
        private ClienteRecebe cr;
        public Thread tClienteRecebe; // Precisa, pois esta treadhchama a instânci do clienteReceve


        private ClienteGT cGT;


        public ArrayList objAenviar;         // vector of currently connected clients
        public ArrayList objFila = new ArrayList();
    
        public ArrayList listaSessoes;  
	    public String AcaoAnterior = "";
        private Color cor_telepointer= Color.Yellow; // Inicialização para ter uma cor. Será trocada pelo que vier da rede
	    private String id_telepointer = "";
        public frmKeyboard Form;
        // public byte[] x = SmallKeyboard.AudioGuitar.;

        #region ClienteConecta() 
        public ClienteConecta() 
        {
            this.connected = false;
            this.objAenviar = new ArrayList();
        }
        #endregion

        #region getLogin()
        public String getLogin()
	    {
		    return this.lo;
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
            MemoryStream ms = new MemoryStream(theByteArray);
            BinaryFormatter bf1 = new BinaryFormatter();
            ms.Position = 0;

            return bf1.Deserialize(ms);
        }
        #endregion

        // monta o que vai ser enviado
        #region EnviaEvento()
        public void EnviaEvento(Object me,String evento)
        {
    	        if(connected) 
    	        {
			        ArrayList list = new ArrayList();
                    list.Add(me); // objeto 'genérico' (pode ser um mouse event ou outro)
        	        list.Add(evento); // o nome do evento
                
        	        this.objAenviar.Add(list);
    	        }
            }
        #endregion


        // monta o que vai ser com outra assinatura (string para o caso do TuxGuitar
        #region EnviaEvento()
        public void EnviaEvento(String me, String evento)
        {
            if (connected)
            {
                String msg = evento + ";" + me;
                this.objAenviar.Add(msg);
            }
        }
        #endregion


        #region EnviaEventoGT()
        public void EnviaEventoGT(Object me, String evento)
        {
            if (connected)
            {
                // cGT.client.Update();

                ArrayList list = new ArrayList();
                list.Add(me); // objeto 'genérico' (pode ser um mouse event ou outro)
                list.Add(evento); // o nome do evento

                ArrayList credencial = new ArrayList();

                credencial.Add(this.lo);
                credencial.Add(this.sess); 
                
                list.Add(credencial); // Informações sobre o login e a sessão

                this.cGT.EnviaObjGT(list);
            }
        }
        #endregion


        #region EnviaEventoAtraso()
        public void EnviaEventoAtraso(Object me,String evento)
        {
    	    if(connected) 
    	    {
        		
    		    ArrayList list = new ArrayList();
	    	    list.Add(me); // objeto 'genérico' (pode ser um mouse event ou outro)
        	    list.Add(evento); // o nome do evento
            
        	    this.objFila.Add(list);
            	
    	    }
        }
        #endregion

        #region EnviaAtraso()
        public void EnviaAtraso()
        {
    	    // Neste método são enviados todos os
    	    // eventos pendentes .

            System.Collections.IEnumerator myEnumerator = objFila.GetEnumerator();
          
            while ( myEnumerator.MoveNext() )
            {
                this.objAenviar.Add(myEnumerator.Current);
            }
        	 
    	     objFila.Clear();
         }

        #endregion

        #region SetaConecta()
        public bool SetaConecta(String s,int port) 
	    {
            
		    if(!this.connected)
		    {
			    this.serverName = s;
	            this.serverPort = port;
    	        
	            return  this.connect();
		    }
		    else
			    return true;
        }

        #endregion

        #region connect()
        protected bool connect() 
         {
             try
			{

                if(socket != null) 
                {
                    try 
                    {
                        socket.Shutdown(SocketShutdown.Both);
                        socket.Close();
                    } catch (Exception e) {}
                }
    
				IPEndPoint remoteEP = new IPEndPoint( IPAddress.Parse(serverName),serverPort);

				socket = new Socket(AddressFamily.InterNetwork,
					SocketType.Stream ,
					ProtocolType.Tcp); 

				socket.Connect(remoteEP);

				if (!socket.Connected)
				{
					// Connection failed, try next IPaddress.
                    MessageBox.Show("Não conectado!" , 
				    Application.ProductName, 
				    MessageBoxButtons.OK, 
				    MessageBoxIcon.Warning);



					socket = null;
                    connected = false;
        	        return false;
				}
                
                connected = true;
                return true;
            }
			catch(Exception e)
			{
                MessageBox.Show(
				"Exception on ClienteConecta:" + e.Message , 
				Application.ProductName, 
				MessageBoxButtons.OK, 
				MessageBoxIcon.Warning);

                connected = false;
        	    return false;
			}
        }

        #endregion

        #region run()  
        public void run()  
        {
            try {
                Object envia;
                
                while(true) 
                {
                    for (int i = 0; i < objAenviar.Count; i++) 
                    {
                	    envia = (Object) objAenviar[i];

                    	// Aqui efetivamente envia os dados!
                        Byte[] ByteGet;

                        ByteGet = getByteArrayWithObject(envia);
                        
                        // System.Diagnostics.Debug.WriteLine("----- Enviado... -----" + DateTime.Now.ToString());
                        // MessageBox.Show("----- Enviado... -----" + DateTime.Now.ToString());

                        socket.Send(ByteGet, ByteGet.Length, 0); 
                    }
                    
                    // Limpando o vetor de itens enviados
                    objAenviar.Clear();

                    if(this.cGT.client.Active) 
                        this.cGT.client.Update(); 

                    // System.Diagnostics.Debug.WriteLine("----- Testando... -----");

                    // Esperando alguns milisegundos
                    // System.Threading.Thread.Sleep(150); 
                    System.Threading.Thread.Sleep(50); 
                }
            
            } catch (ThreadAbortException  e) 
            {
                this.disconnect();
                return;
            }
            catch (Exception e) 
            {

                MessageBox.Show(
				"Exception on ClienteConecta:" + e.Message , 
				Application.ProductName, 
				MessageBoxButtons.OK, 
				MessageBoxIcon.Warning);
            }    	

            this.disconnect();
        }

        #endregion

        #region disconnect() 

        public void disconnect () 
        {
            try 
            {
        	    // System.Threading.Thread.Sleep(3000); 

                this.socket.Shutdown(SocketShutdown.Both);
                this.socket.Close();

                connected = false;

                if (tClienteRecebe.IsAlive)
                    tClienteRecebe.Abort();
                

            } catch (Exception e) 
            {

                /* MessageBox.Show(
				"Exception on ClienteConecta:" + e.Message , 
				Application.ProductName, 
				MessageBoxButtons.OK, 
				MessageBoxIcon.Warning); */
            }
        }

        #endregion

        #region SetaUser()
        public bool SetaUser(String login,String senha) 
	   {
            try 
		    {
			    this.lo = login;
	            this.pass = senha;
    	    
    	        
	            // O primeiro objeto a ser enviado é uma string que vai indicar
	            // ao servidor se eh uma conexão para trocas de objetos no nível do 
	            // GEF ou nível do ArgoUML. Devo enviar também o login e a senha
	    	    ArrayList l = new ArrayList();
	      	    l.Add(this.lo); // 
	            l.Add(this.pass); // 
    	        
	            this.EnviaEvento(l,"ARGO");

                // Aqui efetivamente envia os dados!
                Byte[] ByteGet;

                ByteGet = getByteArrayWithObject((Object) objAenviar[0]);

                socket.Send(ByteGet, ByteGet.Length, 0); 

                objAenviar.Clear();
    			
                // Recebendo a resposta
                byte[] bytes = new byte[1024];
				int bytesRec = socket.Receive(bytes);
                	
                ArrayList list = (ArrayList) getObjectWithByteArray(bytes);          
                
    	
                Object o = list[0];
                String nomeEvento = (String) list[1];
    	
	    	    if (nomeEvento.Equals("ERRO"))
	    	    {
                    
                    MessageBox.Show("Login ou senha incorretos!" , 
				    Application.ProductName, 
				    MessageBoxButtons.OK, 
				    MessageBoxIcon.Warning);

    	    	
	    		    return false;
	    	    }
	    	    else
	    	    {
	    		    // Aqui vou armazenar as informações que vão ser colocadas na tabela!

            	    if (nomeEvento.Equals("PROT_lista_sessoes"))
            	    {
            		    ArrayList se = (ArrayList) o;
            		    // Colocando os nomes das sessões colaborativas
                		
            		    this.listaSessoes = se; 

            		    // Agora colocando a cor do telepointer
            		    String id = (String) list[2];
            		    Color c = (Color) list[3];
                		
                		this.setaCorTelepointer(c);
                        this.setaIdTelepointer(id);

                        // Aqui adiciono o usuário e sua cor no ArrayList de usuários!
                        // O armazenamento será feito em um ArrayList que contém arrays de strings (string[])
                        string[] info = new string[2];
                        info[0] = this.lo;
                        info[1] = c.Name;

                        this.Form.aUsers.Add(info);

                        // Adicionando o usuário no listview
                        System.Windows.Forms.ListViewItem listViewItem = new System.Windows.Forms.ListViewItem(new string[] {
            this.lo.Trim()}, -1, System.Drawing.SystemColors.WindowText, c , new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0))));
                        listViewItem.StateImageIndex = 0;
                        listViewItem.Checked = true;
                        listViewItem.Tag = this.lo.Trim();
                        this.Form.listView1.Items.Add(listViewItem);

                        this.Form.listView1.Refresh();
                        
        	        	
            	    } 

            	    // Iniciando a Thread que vai receber os dados
                    this.cr = new ClienteRecebe(socket);
                    this.cr.Form = this.Form;

                    ThreadStart threadDelegate = new ThreadStart(this.cr.run);
                    tClienteRecebe = new Thread(threadDelegate);
                    tClienteRecebe.Start();

                    // Iniciando a Thread...

                    // Aqui vou colocar a chamada para o cliente GT!
                    cGT = new ClienteGT(this.serverName, "9999",this.Form, this.lo);
                    cGT.client.Update(); 

	    		    return true;
	    	    }
		    }
            catch (Exception e) 
		    {

                    MessageBox.Show("Exception in ClienteConecta " + e.Message  , 
				    Application.ProductName, 
				    MessageBoxButtons.OK, 
				    MessageBoxIcon.Warning);

        	    return false;
            }

        }

        #endregion

        #region setaCorTelepointer()
        public void setaCorTelepointer(Color c)
        {
            cor_telepointer = c;
        }
        #endregion

        #region getCorTelepointer()
        public Color getCorTelepointer()
        {
                
            return cor_telepointer;
        }
        #endregion

        #region setaIdTelepointer()
        public void setaIdTelepointer(String c)
        {
            id_telepointer = c;
        }
        #endregion

        #region getIdTelepointer()
        public String getIdTelepointer()
        {
            return id_telepointer;
        }
        #endregion


        #region setaSessao()
        public void setaSessao(string s)
        {
            this.sess = s;
            this.cGT.sessao = s;
        }
        #endregion

        #region getSessao()
        public string getSessao()
        {
            return this.sess;
        }
        #endregion

        #region getEnvioOK()
        public bool getEnvioOK()
        {
            if( this.cr != null)
                return this.cr.OKEnvio;
            else
                return false;
        }
        #endregion

        #region getClienteGT
        public CoKeyboard.Collab.ClienteGT getClienteGT()
        {
            return cGT;
        }
        #endregion


    }

}

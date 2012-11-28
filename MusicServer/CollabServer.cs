using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Data.SqlClient;
using System.Threading;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Xml;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Web;
using System.Runtime.Serialization.Formatters.Binary;

namespace MusicServer
{
    #region class MusicServer
    class MusicServer
	{

        public ArrayList clientVector;         // vector of currently connected clients
	    private int id_telepointer = 1;
	    private ArrayList telepointers = new ArrayList();
	    private ArrayList userList = new ArrayList(); // Lista contendo os usuários e senhas

        private Thread tClienteConectado;

        #region MusicServer()
        public  MusicServer()
        {

        }
        #endregion

        #region Main(string[] args)
        [STAThread]
		static void Main(string[] args)
		{
			
            // TODO: Inserir endereço IP do servidor e IP como parâmetro
            MusicServer s = new MusicServer();


            int pp;

            if(args.Length.Equals(0))
                pp = 100;
            else
                pp = int.Parse(args[0]);

           s.IniciaServico(pp);
           // s.IniciaServico(100);
        }
        #endregion

        #region IniciaServico
        public void IniciaServico(int port)
        {
            // Lista de usuários
            String []x = {"A" ,"A"};
    	    userList.Add (x);

            String []y = {"B" ,"B"};
		    userList.Add (y);

            String[] z = { "C", "C" };
            userList.Add(z);

            String[] w = { "D", "D" };
            userList.Add(w); 

            
            // IPHostEntry ipHostInfo = Dns.GetHostByName("localhost"); 
            // IPHostEntry ipHostInfo = Dns.GetHostByAddress(end_ip);

            IPHostEntry ipHostInfo = Dns.GetHostByName(Dns.GetHostName()) ;

			IPEndPoint localEP = new IPEndPoint(ipHostInfo.AddressList[0],port);

			Console.WriteLine("Local address and port: " + localEP.ToString());

			Socket listener = new Socket( localEP.Address.AddressFamily,
				SocketType.Stream, ProtocolType.Tcp );

			Thread t;
			ArrayList ListThread = new ArrayList();

			int i;
			
			try 
			{
				listener.Bind(localEP);
				listener.Listen(10);
		
				i = 0;

                clientVector = new ArrayList();
                MusicServerT c;

			    // A cada nova requisão uma nova Thread é criada
			    while (true)
			    {
				    c = new MusicServerT (listener.Accept(),this);
				    
                    // Iniciando a Thread do cliente!

                    ThreadStart threadDelegate = new ThreadStart(c.run);
                    tClienteConectado = new Thread(threadDelegate);
                    tClienteConectado.Start();
			    }


			} 

			catch (Exception e) 
			{
				Console.WriteLine("Error:" + e.ToString());

			}


        }
        #endregion

        #region RemoveClient()
        public void RemoveClient (MusicServerT singleThread) 
        {
            clientVector.Remove(singleThread);
        }
        #endregion

        #region BroadCastToAll()
        public void BroadCastToAll (Object objrecebido, MusicServerT tEnviada, bool skipItself) 	
        {
    	    this.BroadCastToAll(objrecebido,tEnviada,skipItself,tEnviada.tipocon);
        }
        #endregion

        #region BroadCastToAll()

        public void BroadCastToAll (Object objrecebido, MusicServerT tEnviada, bool skipItself,String tipocon) 	
        {
            
            MusicServerT aSingleThread = null;
            
            for (int i = 0; i < clientVector.Count ; i++) 
            {
                aSingleThread = (MusicServerT) clientVector[i];

                
                // Console.WriteLine("T:" + aSingleThread.tipocon + " S: " + aSingleThread.nome_sessao);
                
               //   NOVA MODIFICACAO - olho
                if(skipItself)
                {
            	    
                    // Retirando temporariamente a verificação do tipo de conexão
                    //if( (!aSingleThread.Equals(tEnviada)) && (aSingleThread.tipocon.Equals(tipocon))   )
                    if (!aSingleThread.Equals(tEnviada) )
            	    {
                        // Console.WriteLine("Evento enviado!");
                        aSingleThread.BroadCastToClient(objrecebido);
            	    }
                }
                else
                {
                    if( (aSingleThread.tipocon.Equals(tipocon))   )
                        aSingleThread.BroadCastToClient(objrecebido);
                	
                }                  
            }
        }
        #endregion

        #region ExisteSessao
        public bool ExisteSessao (String nomesessao) 
        {
        
            MusicServerT aSingleThread = null;
        
            for (int i = 0; i < clientVector.Count ; i++) 
            {
                aSingleThread = (MusicServerT) clientVector[i];

                if( aSingleThread.nome_sessao.Equals(nomesessao))
            	    return true;
            }
            
            return false;
        }

        #endregion

        // Obtem o modelo atual da sessão de algum dos participantes
        #region ModeloSessao
        public Object ModeloSessao (String nomesessao) 
        {
            MusicServerT aSingleThread = null;
        
            for (int i = 0; i < clientVector.Count; i++) 
            {
                aSingleThread = (MusicServerT) clientVector[i];

                if( aSingleThread.nome_sessao.Equals(nomesessao))
                	return  aSingleThread.modelo_atual;
            }
        
            return null;
        }

        #endregion

        // Obtem o modelo atual da sessão de algum dos participantes
        #region ModeloSessaoInicial()

        public ArrayList ModeloSessaoInicial (String nomesessao) 
        {
            MusicServerT aSingleThread = null;
            
            for (int i = 0; i < clientVector.Count; i++) 
            {
                aSingleThread = (MusicServerT) clientVector[i];

                if( aSingleThread.nome_sessao.Equals (nomesessao))
            	    return  aSingleThread.modelo_inicial;
            }
            return null;
        }
        #endregion

        #region IdsSessaoInicial 
        public ArrayList IdsSessaoInicial (String nomesessao) 
        {
            MusicServerT aSingleThread = null;
            
            for (int i = 0; i < clientVector.Count; i++) {
                aSingleThread = (MusicServerT) clientVector[i];

                if( aSingleThread.nome_sessao.Equals (nomesessao))
            	    return  aSingleThread.ids_inicial;
            }
            return null;
        }
        #endregion

        // retorna todas os nomes da sesões colaborativas atuais
        #region NomeSessoes 

        public ArrayList NomeSessoes () 
        {
            MusicServerT aSingleThread = null;

            ArrayList ret_aux = new ArrayList();
            ArrayList retorno  = new ArrayList();
            String nome_sessao = "";
            String user_sessao = "";
            
            // Neste primeiro loop eu objenho todos os nomes das sessoes
            for (int i = 0; i < clientVector.Count; i++) 
            {
                aSingleThread = (MusicServerT) clientVector[i];

                if(  !ret_aux.Contains(aSingleThread.nome_sessao) && !aSingleThread.nome_sessao.Equals("")  )
                {
            	    ret_aux.Add(aSingleThread.nome_sessao);
                }
            }
            
            // Aqui objeto os usuarios de cada sessao
            for (int i = 0; i < ret_aux.Count; i++) 
            {
        	        nome_sessao = (String) ret_aux[i];
				    user_sessao = "";  
    				
                    for (int j = 0; j < clientVector.Count; j++) 
                    {
                        aSingleThread = (MusicServerT) clientVector[j];
                        
                        if(aSingleThread.nome_sessao.Equals(nome_sessao) && user_sessao.IndexOf(aSingleThread.login) == -1 )
                        {
                    	    user_sessao = user_sessao  + "+" + aSingleThread.login + "," + this.getColor(aSingleThread.id_pointer).Name;     	
                        }
                    }
        	        String []inserir = { nome_sessao, user_sessao.Substring(1) };
            	    
        	        retorno.Add(inserir);    
            }
            
            return retorno;
        }

        #endregion

        // CheckLock verifica se existe um lock na fig e pode remover este lock
        #region CheckLock (String fig) 
        public bool CheckLock (String fig) 
        {
    	    return this.CheckLock(fig,false);
        }

        #endregion

        #region CheckLock (String fig,boolean remove) 
        public bool CheckLock (String fig,bool remove) 
        {
            MusicServerT aSingleThread = null;

            for (int i = 0; i < clientVector.Count; i++) {
                aSingleThread = (MusicServerT) clientVector[i];

                if( aSingleThread.resources_locked.Contains(fig) )
                {
            	    if(remove)
            		    aSingleThread.resources_locked.Remove(fig);	

            	    return true;
                }
            }
            return false;
        }

        #endregion

        #region getLockStatus() 

        public ArrayList getLockStatus () 
        {
        
    	    ArrayList ret = new ArrayList();
    	    MusicServerT aSingleThread = null;
            ArrayList r;
            
            String logaJanela = "";

            for (int i = 0; i < clientVector.Count; i++) 
            {
                aSingleThread = (MusicServerT) clientVector[i];

                r = aSingleThread.resources_locked;
                
                for (int j = 0; j < r.Count; j++) 
                {
                	
            	    logaJanela = logaJanela + " usuario:" + aSingleThread.login + " - objeto: "  + (String)  r[j] + ","; 
                	
            	    String []x = {aSingleThread.login ,(String)  r[j]};
                	
            	    ret.Add(x);
                	
                }
            }
            
            // Toda a vez que solicitarem o status global dos locks vou 
            // logar em um arquivo separado
            
            // this.LogaMsg(logaJanela,2);
            
            return ret;
        }

        #endregion

        // Limita a apenas 4 telepointers
        #region getPointerID()
        public String getPointerID()
        {

    	    String x = "1";
        	
    	    for (int i = 0; i < 4; i++) 
    	    {
    		    id_telepointer = i+1;
    		    if(!telepointers.Contains(id_telepointer.ToString()) )
    		    {
    			    x = id_telepointer.ToString();
        			
    			    telepointers.Add(x); 
        			
    			    return x;
    		    }
    	    }
        	
    	    return x;
        }
        #endregion

        #region getColor
        public Color getColor(String id)
        {
    	    // Aqui vou especificar as colores e os ID's dos telepointers
    	    Color fColor = Color.Cyan;
		    
            if (id.Equals("1")) 
			    fColor = Color.Red; 
		    if (id.Equals("2"))
                fColor = Color.CornflowerBlue;
		    if (id.Equals("3")) 
			    fColor = Color.Cyan;
		    if (id.Equals("4")) 
			    fColor = Color.Orange;
    		
		    return fColor;
        }
        #endregion

        // Procurar a cor do telepointer do usuário
        #region getEyeColor()
        public Color getEyeColor(String user)
        {
    	    Color fColor = Color.Red;

            MusicServerT aSingleThread = null;
            
            for (int i = 0; i < clientVector.Count; i++) 
            {
                aSingleThread = (MusicServerT) clientVector[i];
                
                if( user.Equals(aSingleThread.login) && aSingleThread.tipocon.Equals("ARGO") )
            	    fColor = this.getColor(aSingleThread.id_pointer); 
            }
                
    	    return fColor;
        }
        #endregion

        #region RemovePointer
        public void RemovePointer(String id)
        {
		        if(telepointers.Contains(id) )
			        telepointers.Remove(id);
        		
		        return;
        }
        #endregion

        // Verifica se o usuário/senha existem
        #region checkUser
        public bool checkUser(String l,String s)
        {
            for (int i = 0; i < userList.Count; i++) 
            {
                String []user = (String []) userList[i];
                
                if(user[0].Equals(l) && user[1].Equals(s))
            	    return true;
            }
            
            return false;
        }

        #endregion

        #region LogaMsg()
        public void LogaMsg(String msg,int tipo) // 0 -> Chat, 1->Lock, 2->Janela de lock
        {

        }
        #endregion


    }
    #endregion

    #region MusicServerT
    class MusicServerT
	{

        Socket client;
    
        private String CON = "OBJ";
    
        private MusicServer servidor;
        public String id_pointer;
        public String tipocon; 
        public String login;
        public String senha;
        public String nome_sessao = "";
        public Object modelo_atual;
        public ArrayList modelo_inicial = new ArrayList();
        public ArrayList ids_inicial = new ArrayList();
    
        public ArrayList resources_locked = new ArrayList();

        #region MusicServerT
        public  MusicServerT(Socket client, MusicServer s)
        {
           	// Preenche os campos e seta a priopridade desta Thread
    		this.client = client;
	    	this.servidor = s;
    		this.tipocon = "";
        }

        #endregion

        #region run() 
        public void run() 
	    {
            try 
            {
                this.CON = "OBJ";

                bool clientTalking = true;
                
                Object clientObject = null;

                //a loop that reads from and writes to the socket
                while (clientTalking) 
                {

                    byte[] bytes = new byte[8192];

                    int bytesRec = this.client.Receive(bytes);

                    if (bytesRec == 0)
                    {
                        clientTalking = false;
                        break;
                    }

                    string returnData = Encoding.ASCII.GetString(bytes);

                    // // Tratamento para o caso da informação vier pelo TuxGuitar
                    if ((returnData.IndexOf("TUX") >= 0) || (this.tipocon.Equals("TUXGUITAR")))
                        clientObject = QuebraProtocoloTux(returnData);
                    else
                    {
                        // get what client wants to say...
                        clientObject = getObjectWithByteArray(bytes);
                        
                        // Verificação caso ocorra algum problema com 
                        // a des-serialização!

                        if (clientObject == null)
                            continue;
                        
                    }

            
                    // System.out.println("Servidor recebeu algum objeto!");
                    
                    if(!this.trataProtocolo(clientObject))
                    {
	                    // if client dissappeared.. 
	                    if (clientObject == null) 
	                        clientTalking = false;
	                     else 
	                     {
	                 	    // Preciso mandar o objeto somente para os clientes
	                 	    // que participarem da mesma sessão
	                 	    this.servidor.BroadCastToAll(clientObject, this, true);
	                     }
               	    }
                }
            } 			
            catch (Exception e) 
			{
				Console.WriteLine("Error:" + e.ToString());

			}

            this.disconnect();
    	}
        #endregion

        #region  parseEye(String text)

        // Tranforma em objeto o que foi enviado em texto
	    private Object parseEye(String text)
	    {
		    Object clientObject;
		    String msg_protocolo = "";
    		
		    // Podem haver dois tipos de mensagem: EYE e PROT
    		
		    ArrayList l = new ArrayList();
		    ArrayList dados = new ArrayList();
    		
		     
            String [] qtd = text.Split(",".ToCharArray() ,StringSplitOptions.None);
            
            for(int i =0;i<qtd.Length;i++)
            {
                l.Add( (String) qtd[i] );
            }
    	 
	        // Adequacao ao protocolo! 
	        msg_protocolo = (String) l[0]; 
		    l.RemoveAt(0);
    	    
	        dados.Add(l);
		    dados.Add(msg_protocolo);
    		
		    clientObject = (Object) dados;
		    return clientObject;
        }
        #endregion

        // Trata as mensagens do protocolo
        #region trataProtocolo
        private bool trataProtocolo (Object clientObject) 
	    {

            // Se for string já cai logo fora POR CAUSA DO TUX GUITTAR! 
            // TODO: Pensar melhor no controle de acesso pelo TuxGuitar
            if (clientObject.GetType().Equals("a".GetType()))
                 return false;
            
            // Aqui verifica qual é a mensagem enviada. São tratadas apenas as mensagens que 
            // dizem respeito ao servidor (controle sessão, atulização modelo, etc
            ArrayList list = (ArrayList) clientObject;

            Object objDados = list[0];
            String nomeEvento = (String)list[1];

            // Aqui são verificadas as mensagens iniciais, para identificar qual é a conexão
            #region GEF, ARGO, E, TUXGUITAR
            if ((nomeEvento.Equals("GEF")) || (nomeEvento.Equals("ARGO")) || (nomeEvento.Equals("E")) || (nomeEvento.Equals("TUXGUITAR")))
            {
        	    ArrayList l = (ArrayList) objDados;
            	
            	
        	    if ( this.servidor.checkUser((String) l[0],(String) l[1] )  )
        	    {
        		    this.tipocon = nomeEvento;
        		    this.login = (String) l[0];
        		    this.senha = (String) l[1];
        		     // System.out.println("Conexão autorizada!");
            		
        		    // Adicionando ao array de clientes
        		    this.servidor.clientVector.Add(this);
        			
        		    if(nomeEvento.Equals("E"))
	        	    {
        			    this.id_pointer = this.servidor.getPointerID(); 
	        	    }
            		
        		    // Daqui para baixo o envio de dados eh apenas 
        		    // para conexoes do ARGO e do GEF
                    if ((nomeEvento.Equals("GEF")) || (nomeEvento.Equals("ARGO")) || (nomeEvento.Equals("TUXGUITAR")))
        		    {		
	        		    // Enviar para o cliente o nome das sessões!
	        		    l = new ArrayList();
		        	    l.Add(this.servidor.NomeSessoes()); 
		        	    l.Add("PROT_lista_sessoes");

                        // Aqui vou colocar uma cor para o telepointer do cliente
                        this.id_pointer = this.servidor.getPointerID(); 
    	
		        	    if(nomeEvento.Equals("ARGO"))
		        	    {
			        	    
			        	    l.Add(this.id_pointer);
			        	    l.Add(this.servidor.getColor(this.id_pointer));
		        	    }

                        this.BroadCastToClient(this.AcertaFormatoEnvio(nomeEvento,l));
        		    }

        	    }
        	    else // Erro porque o login e a senha estão incorretos
        	    {
                    l = new ArrayList();
        		    l.Add("ERRO");
	        	    l.Add("ERRO");

            	    this.BroadCastToClient(this.AcertaFormatoEnvio(nomeEvento,l));
    	        	
	        	    // servidor.RemovePointer(id_pointer);
	                // servidor.RemoveClient(this);
            		
        		    // this.disconnect();
        	    }
        	    return true;
            }
            #endregion


            // Aqui são verificadas as mensagens de 'protocolo'
            #region PROT
            if (  nomeEvento.StartsWith("PROT") )
            {
                #region PROT_nova_sessao
                // O cliente quer abrir uma noca sessão
    		    if (nomeEvento.Equals("PROT_nova_sessao"))
    		    {
    			    ArrayList li = (ArrayList) objDados;
        			
    			    // A verificação da possível criação de uma sessão que
    			    // já exista deve ser feita no ArgoUML e não no servidor!
        			
				    this.nome_sessao = ((String) li[0]);
    				
				    // Colocando o modelo do diagrama atual no servidor
				    // this.modelo_atual = ((String) li.get(1));

                    if (this.tipocon.Equals("TUXGUITAR"))
                    {
                        this.modelo_inicial = null;
                        this.ids_inicial = null;
                    }
                    else
                    {
                        this.modelo_inicial = ((ArrayList)li[1]);

                        // Este terceiro elemento eh mais um arraylist contendo os ID's das Figs
                        this.ids_inicial = ((ArrayList)li[2]);
                    }
        				
				    // Logando o horário que o usuário entrou na sessao
				    this.servidor.LogaMsg( login + " entrou na sessao:" + this.nome_sessao,0);
                }
                #endregion

                #region PROT_sessao_existente
                // O cliente quer participar de uma sessão existente
    		    if (nomeEvento.Equals("PROT_sessao_existente"))
    		    {

                    String session_name;

                    ArrayList l_aux = (ArrayList) objDados;
                    session_name = (String)l_aux[0];

    			    if(this.servidor.ExisteSessao(session_name))
    			    {
        				
    				    // Aqui são feitas as devidas inicializações da sessão colaborativa
                        this.nome_sessao = session_name;

    				    // Obtem o modelo desta sessão de algum dos participantes
    				    this.modelo_inicial = this.servidor.ModeloSessaoInicial(this.nome_sessao);
    				    this.ids_inicial = this.servidor.IdsSessaoInicial(this.nome_sessao);
    				    this.modelo_atual = this.servidor.ModeloSessao(this.nome_sessao);
        				
        				
    				    // Envia este modelo para o cliente, para que ele atualize o seu modelo
                        // Também preciso enviar os logins que já existem para poder criar 
                        // a lista de usuários e o teclado 'fantasma'
	    			    ArrayList l = new ArrayList();
                        l.Add(this.servidor.NomeSessoes());
                        l.Add("PROT_atualiza_modelo_cliente_inicial");
	        		    // l.Add(this.ids_inicial);

    				    // this.BroadCastToClient(l);

                        this.BroadCastToClient(l);
        				
    				    if(this.tipocon.Equals("ARGO"))
    				    {
            				
    					    // Logando o horário que o usuário entrou na sessao
    					    this.servidor.LogaMsg( login + " entrou na sessao:" + this.nome_sessao,0);

    					    // Vou enviar a notificão de novo usuário para os clientes!
    					    l = new ArrayList();
    					    l.Add(login + "," + this.servidor.getColor(this.id_pointer).Name); 
    					    l.Add("PROT_inicio_sessao");

                            this.servidor.BroadCastToAll(l, this, true, "ARGO");
    				    }
        				
        				
    			    }
                }
                #endregion

                #region PROT_chat_msg
                //    		 O cliente quer participar de uma sessão existente
    		    if (nomeEvento.Equals("PROT_chat_msg"))
    		    {
    			    // Montando o objeto que será replicado
    			    ArrayList l = new ArrayList();
	        	    l.Add(login + ":" + ((String) objDados)); 
	        	    l.Add("PROT_chat_msg");
        			
	        	    //	Aqui vou colocar uma cor para a mensagem do cliente
	        	    l.Add(this.servidor.getColor( this.id_pointer ));
    	        	
    			    this.servidor.BroadCastToAll(l, this, false,"ARGO");
    			
    			    // 	Logando a conversa em um arquivo
    			    this.servidor.LogaMsg( login + ":" + ((String) objDados),0 );

                }
                #endregion

                #region PROT_fim_sessao
                // Algum cliente resolveu sair da sessão colaborativa
    		    if (nomeEvento.Equals("PROT_fim_sessao"))
    		    {
    //				 Logando o horário que o usuário entrou na sessao
				    this.servidor.LogaMsg( login + " saiu na sessao:" + this.nome_sessao,0);
    				
				    // Enviando para os clientes o nome do usuário que saiu da sessão
    			    // Montando o objeto que será replicado
    			    ArrayList l = new ArrayList();
	        	    l.Add(login); 
	        	    l.Add("PROT_fim_sessao");
    	        	
    			    this.servidor.BroadCastToAll(l, this, true,"ARGO");

    			    //Removendo as dependências deste cliente (Thread & afins)
    			    this.servidor.RemovePointer(this.id_pointer);
                    this.servidor.RemoveClient(this);
                    
                    // TODO: Descobrir como interrompter a Thread!
                    // this.interrupt();
                }
                #endregion

                #region PROT_fim_sessao
                ////////////////////////////////////LOCK////////////////////////////////////////////
    		    // Aqui sera verificado se o lock solicitado pode ser liberado
    		    if (nomeEvento.Equals("PROT_lock_request"))
    		    {
	    			    ArrayList l = new ArrayList();
	    			    l.Add(((String) objDados));
    	    		
	    			    // System.out.println("Lock request on:" + ((String) objDados));
	    			    this.servidor.LogaMsg( login + ":" + " Lock request on:" + ((String) objDados)  ,1 );
        			
    				    if (!this.servidor.CheckLock(  ((String) objDados) ))
    				    {
    					    this.resources_locked.Add( ((String) objDados) );

    					    this.servidor.LogaMsg( login + ":" + " Lock granted on:" + ((String) objDados)  ,1 );
						    l.Add("PROT_lock_granted"); 
						    // System.out.println("Lock granted on:" + ((String) objDados));
    						
	    				    // Manda o status para os clientes
	    				    this.BroadLockStatusToAll();
    				    }
    				    else
    				    {
    					    // Aqui vou fazer uma verificação. Se for esta conexão
    					    // que esta com o lock, retornar OK
        					
    					    if(this.resources_locked.Contains((String) objDados) )
    					    {
        					
    						    this.servidor.LogaMsg( login + ":" + " Lock granted on:" + ((String) objDados)  ,1 );
    						    l.Add("PROT_lock_granted"); 
    						    // System.out.println("Lock alredy granted on:" + ((String) objDados));
        						
    	    				    // Manda o status para os clientes
    	    				    this.BroadLockStatusToAll();
    					    }
    					    else
    					    {
    						    this.servidor.LogaMsg( login + ":" + " Lock deny on:" + ((String) objDados)  ,1 );
    						    l.Add("PROT_lock_deny");
    						    // System.out.println("Lock deny on:" + ((String) objDados));
    					    }
    				    }
        	        	
    				    // Manda a resposta da requisitção de lock
    				    this.BroadCastToClient(l);

                }
                #endregion

                #region PROT_lock_request_group
                // Aqui sera verificado se os locks solicitados pode ser liberado
    		    // Lock multiplo
    		    if (nomeEvento.Equals("PROT_lock_request_group"))
    		    {
    				    // O ArrayList abaixo vai montar a mensagem de lock_ok ou lock_deny
    				    ArrayList l = new ArrayList();
        				
    				    ArrayList liberados = new ArrayList();
        			
    				    // Este ArrayList vai conter os objetos que querem lock 
    				    ArrayList objs = (ArrayList) objDados;
        				
    				    l.Add(objs);
        				
    				    bool lock_ok = true;
        				
    				    // Varrendo a ArrayList dos objetos que querem lock
    				    for(int i = 0;i<objs.Count;i++)
        			    {
						    if (!this.servidor.CheckLock(  (String) objs[i]  ))
						    {
							    liberados.Add((String) objs[i]);
						    }
						    else
						    {
							    //	Aqui vou fazer uma verificação. Se for esta conexão
	    					    // que esta com o lock, OK, senão retorna erro! 
	    					    if(!this.resources_locked.Contains( (String) objs[i]  ))
	    						    lock_ok = false;
						    }
        			    }
        				
    				    String elementos ="";
        				
    				    // Vendo se todos os locks estao ok
    				    if(lock_ok)
    				    {
    					    // Agora vou colocar efetivamente o lock em todo mundo
    					    for(int i = 0;i<liberados.Count;i++)
            			    {
    						    this.resources_locked.Add( (String) liberados[i] );
        						
    						    elementos = elementos + "," + (String) liberados[i];
            			    }
        					
    					    // Mandando a mensagem
    					    l.Add("PROT_lock_granted");
        					
    					    this.servidor.LogaMsg( login + ":" + " Lock request group granted:" + elementos,1 );
    				    }
    				    else
    				    {
    					    l.Add("PROT_lock_deny");
    					    this.servidor.LogaMsg( login + ":" + " Lock request group denyed:" + elementos,1 );	
    				    }
        				
    				    // Enviando a notificação para os outros clientes
    				    this.BroadLockStatusToAll();
        				
    				    // Enviado a mensagem para o cliente
    				    this.BroadCastToClient(l);

                }
                #endregion

                #region PROT_lock_release
                // Aqui libera os locks que existem nas figuras
    		    if (nomeEvento.Equals("PROT_lock_release"))
    		    {
				    // System.out.println("Lock release on:" + ((String) objDados));
    			    this.servidor.LogaMsg( login + ":" + " Lock release on:" + ((String) objDados)  ,1 );
    			    this.resources_locked.Remove(((String) objDados));
        			
				    // Manda o status para os clientes
				    this.BroadLockStatusToAll();

                }
                #endregion

                #region PROT_lock_clear
                if (nomeEvento.Equals("PROT_lock_clear"))
    		    {
    			    // Antes de fazer o lock clear eu preciso montar um array 
    			    // contendo os id's dos elementos que perderam o lock
    			    // e que podem ter um novo nome
    			    ArrayList modificados = new ArrayList();
        			
    			    String compara = "";

    			    if(objDados != null)
    			    {
	    			    ArrayList nomesElementos = (ArrayList) objDados;
    	    			
	    			    for(int i=0;i<nomesElementos.Count;i++)
	    			    {
	    				    String [] dados = (String []) nomesElementos[i];
    	    				
	    	                String []x = {dados[0],dados[1],dados[2],dados[3],dados[4],dados[5],dados[6]};
    	    	            	
	    	                modificados.Add(x);
	    			    }
    			    }
        			
    			    // Agora posso apagar todos os elementos
    			    this.servidor.LogaMsg( login + ":" + " Lock clear",1 );
    			    this.resources_locked.Clear(); 
    			    // System.out.println("Lock Clear" );
        			
				    // Manda o status para os clientes
				    this.BroadLockStatusToAll(modificados);

                }
                #endregion

                #region PROT_lock_clear_almost
                if (nomeEvento.Equals("PROT_lock_clear_almost"))
    		    {
        		
    			    ArrayList modificados = new ArrayList();
        			
    			    ArrayList selecionados = (ArrayList) objDados;

    //   			 Aqui vou montar o array com apenas os elementos
    			    // que perderam o lock e naum com todos
    			    if(list.Count > 2)
    			    {
    				    ArrayList nomesElementos = (ArrayList) list[2];
    	    			
	    			    for(int i=0;i<nomesElementos.Count;i++)
	    			    {
	    				    String []dados = (String []) nomesElementos[i];
    	    				
	        			    for(int j = 0;j<selecionados.Count;j++)
	        			    {
	        				    if(!selecionados[j].Equals(dados[0]))
	        				    {
	    	    				    String []x = {dados[0],dados[1],dados[2],dados[3],dados[4],dados[5],dados[6]};
	    	    	                modificados.Add(x);
	        				    }
	        			    }
	    			    }
    			    }
        			
        			
    			    // Vou limpar os locks em todos, menos os que ja estão selecionados
    			    this.resources_locked.Clear(); 
        			
        			
    			    String elementos ="";
        			
    			    for(int i = 0;i<selecionados.Count;i++)
    			    {
    				    // Preciso retirar o lock da Thread que a possui!
        				
    				    this.resources_locked.Add( selecionados[i] );
    				    elementos = elementos + "," + selecionados[i];
    			    }
        			
        			
        			
        			
    			    this.servidor.LogaMsg( login + ":" + " Lock clear almost:" + elementos,1 );
        			
				    // 	Manda o status para os clientes
				    this.BroadLockStatusToAll(modificados);
        			
    			    // System.out.println("Lock Released - liberando todos os locks menos os selecioandos" );
    		    }
                ////////////////////////////////////LOCK////////////////////////////////////////////

                #endregion

                #region PROT_remove_elemento
                if (nomeEvento.Equals("PROT_remove_elemento"))
    		    {
    			    ArrayList selecionados = (ArrayList) objDados;

    			    for(int i = 0;i<selecionados.Count;i++)
    			    {
        			    this.servidor.LogaMsg( login + ":" + " Lock release on:" + ((String) selecionados[i])  ,1 );
        			    this.servidor.CheckLock( (String) selecionados[i], true);
    			    }
        			
    			    // Manda o status para os clientes
				    this.BroadLockStatusToAll();
    				
				    // Vou encaminhar a mensagem PROT_remove_elemento para os demais usuários
				    return false;
                }

                #endregion

                #region PROT_atualiza_modelo_servidor
                //  O cliente mandou um modelo para ser atualizado nos clientes
    		    // Porem somente os clientes que são da mesma sessão!
    		    if (nomeEvento.Equals("PROT_atualiza_modelo_servidor"))
    		    {
    			    // Atualizando o modelo desta thread
    			    this.modelo_atual = (Object) objDados;
        			
    			    // System.out.println("Cliente mandou o modelo:" + this.modelo_atual);
        			
    			    // Montando o objeto que será replicado
    			    ArrayList l = new ArrayList();
	        	    l.Add(this.modelo_atual); 
	        	    l.Add("PROT_atualiza_modelo_cliente");
        			
    			    this.servidor.BroadCastToAll(l, this, true);

                }

                #endregion

                #region PROT_EYE
                // O cliente quer abrir uma noca sessão
    		    if (nomeEvento.Equals("PROT_EYE"))
    		    {
    			    // Enviar pela conexão do GEF
    			    // a movimentação ocular para todos os usuários
        			
        			 
    			    ArrayList l_olho = (ArrayList) objDados;
        			 
    			    // System.out.println("Posicao do olho do usuario:" + (String) l_olho.get(0));
    			    // System.out.println("Pos X:" + (String) l_olho.get(1));
    			    // System.out.println("Pos Y:" + (String) l_olho.get(2));
        				
        			
    			    ArrayList manda = new ArrayList();
    			    ArrayList p = new ArrayList();
        			
    			    p.Add((String) l_olho[1]); // x
    			    p.Add((String) l_olho[2]); // y
    			    manda.Add(p); // objeto 'genérico' (pode ser um mouse event ou outro)
                	
    			    manda.Add("eyeMovedPointer");
                	
            	    // Mandando a cor atual do telepointer
            	    // manda.add(this.servidor.getColor(this.id_pointer));
    			    manda.Add(this.servidor.getEyeColor( (String) l_olho[0]) );
        			
            	    // 	Mandando id do telepointer
            	    manda.Add(this.id_pointer);
                	
            	    // Mandando o nome do proprietario do telepointer
            	    manda.Add((String) l_olho[0]);
                	
    			    this.servidor.BroadCastToAll(manda, this, true,"GEF");
        			
    			    return true;
                }

                #endregion

                return true;
            }
            #endregion

            return false;
	    }

        #endregion

        #region BroadLockStatusToAll()
        private void BroadLockStatusToAll () 	
       {
            // ArrayLista que vai conter as notificações dos clientes (cores)
            ArrayList avisa;
            ArrayList manda = new ArrayList();

            avisa = this.servidor.getLockStatus(); 
    		
            manda.Add(avisa);
            manda.Add("PROT_notify_lock");

		    this.servidor.BroadCastToAll(manda, this, true,"GEF");

        }
        #endregion

        #region AcertaFormatoEnvio
        private Object AcertaFormatoEnvio(string evento, object lista)
        {
            Object o;
            if(evento.Equals("TUXGUITAR"))
            {
                // Aqui vou transferir tudo que está na lista para uma string separada por ;

                ArrayList l = (ArrayList)lista;
                String msg = "";

                for (int i = 0; i < l.Count; i++)
                {
                    if( l[i].ToString().IndexOf("Array") >=0 )
                    {
                        // O elemento é um arrayllist. É preciso então quebrar os elementos...
                        ArrayList dados = (ArrayList)l[i];

                        for (int j = 0; j < dados.Count; j++)
                        {
                            // Se for um array de strings...
                            if (dados[j].ToString().IndexOf("[]") >= 0)
                            {
                                string[] a = (String []) dados[j];
                                
                                msg = msg + a[0] + ";";
                            }
                            else
                                msg = msg + dados[j].ToString() + ";";
                        }
                    }
                    else
                        msg = msg + l[i].ToString() + ";";
                }

                o = (object)msg.Substring(0, msg.Length - 1);
            }
            else
                o = lista;

            return o;
        }
        #endregion

        #region QuebraProtocoloTux
        private Object QuebraProtocoloTux(string returnData)
        {
            string[] dados = returnData.Split(";".ToCharArray());

            ArrayList d = new ArrayList();

            for (int i = 1; i < dados.Length; i++)
            {
                if (dados[i].IndexOf("\0") >= 0)
                    d.Add(dados[i].Substring(0, dados[i].IndexOf("\0")));
                else
                    d.Add(dados[i]);

            }

            ArrayList l = new ArrayList();

            l.Add(d); //dados
            l.Add(dados[0]);     // Nome do evento

            return (Object) l;

        }
        #endregion

        #region BroadLockStatusToAll()
        private void BroadLockStatusToAll (ArrayList nomes) 	
        {
            // ArrayLista que vai conter as notificações dos clientes (cores)
            ArrayList avisa;
            ArrayList manda = new ArrayList();

            avisa = this.servidor.getLockStatus(); 
    		
            manda.Add(avisa);
            manda.Add("PROT_notify_lock");
            manda.Add(nomes);

		    this.servidor.BroadCastToAll(manda, this, true,"GEF");

        }
        #endregion

        //  when connection ends - closes streams, stops this thread and notifies
        // server about the disconnection of this client.
        #region disconnect() 
        private void disconnect () 
        {
            try {
                
                this.client.Shutdown(SocketShutdown.Both);
                this.client.Close();

                // Liberando todos os locks que este cara tenha
                this.resources_locked.Clear(); 
    			
			    // Manda o status para os clientes
			    this.BroadLockStatusToAll();


                // Aqui vou remover o Id do Telepointer
                servidor.RemovePointer(this.id_pointer);
                servidor.RemoveClient(this);
                
                // TODO: Preciso de um jeito para fechar esta Thread!
                 
            } 
            catch (IOException e)
            {
                 Console.WriteLine("Error:" + e.ToString());
            } 
        }           // disconnect
        #endregion

        #region BroadCastToClient()
        public void BroadCastToClient (Object obj) 
        {
            try 
            {

                System.Threading.Thread.BeginCriticalRegion();

            	// Aqui efetivamente envia os dados!
                Byte[] ByteGet;

                // Aqui preciso verificar qual é o tipo do objeto

        
                // Se enviar apenas o string, é o caso do Tux
                if (obj.GetType().Equals("a".GetType()))
                {
                    System.Text.ASCIIEncoding  encoding=new System.Text.ASCIIEncoding();
                    ByteGet = encoding.GetBytes((string) obj);

                }
                else
                    ByteGet = getByteArrayWithObject(obj);

                client.Send(ByteGet, ByteGet.Length, 0); 

                System.Threading.Thread.EndCriticalRegion();

            } 
    		catch (Exception e) 
			{
				Console.WriteLine("Error:" + e.ToString());
                this.disconnect();

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
        // Colocando a exception para evitar problemas de des-serialização
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

#endregion 


    #region Classe LogWriter
    public class LogWriter
	{
		private static LogWriter p;

		private String LogErrorFileName;
		private String LogOpFileName;
		// Propriedades do Destino

		
		public String LogErrorName
		{
			get {return this.LogErrorFileName;}
			set {this.LogErrorFileName= value;}
		}

		public String LogOpName
		{
			get {return this.LogOpFileName;}
			set {this.LogOpFileName= value;}
		}

		
		private LogWriter()
		{

			string caminho = @System.Environment.CommandLine;
			caminho = 	caminho.Substring(1,caminho.LastIndexOf(@"\")-1);
		
			this.LogErrorFileName=caminho + @"\LogError" + DateTime.Now.ToString("yyyy-MM-dd-HH_mm_ss")+  ".log";
			this.LogOpFileName=caminho + @"\LogOp" + DateTime.Now.ToString("yyyy-MM-dd-HH_mm_ss")+  ".log";;

		}

		public static LogWriter getLogWriter()
		{
			if (p == null)
				p = new LogWriter();

			return p;
		}

		
		public void GravaLog(String msg,String filename)
		{
			// Gravando os logs de erro
			StreamWriter w =  File.AppendText(@filename);
			
			w.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " - " +  msg + "\n\n");	
			w.Flush();
			w.Close();
		}
    }
    #endregion


}

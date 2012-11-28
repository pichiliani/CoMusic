// Copyright (c) 1996-99 The Regents of the University of California. All
// Rights Reserved. Permission to use, copy, modify, and distribute this
// software and its documentation without fee, and without a written
// agreement is hereby granted, provided that the above copyright notice
// and this paragraph appear in all copies.  This software program and
// documentation are copyrighted by The Regents of the University of
// California. The software program and documentation are supplied "AS
// IS", without any accompanying services from The Regents. The Regents
// does not warrant that the operation of the program will be
// uninterrupted or error-free. The end-user understands that the program
// was developed for research purposes and is advised not to rely
// exclusively on the program for any reason.  IN NO EVENT SHALL THE
// UNIVERSITY OF CALIFORNIA BE LIABLE TO ANY PARTY FOR DIRECT, INDIRECT,
// SPECIAL, INCIDENTAL, OR CONSEQUENTIAL DAMAGES, INCLUDING LOST PROFITS,
// ARISING OUT OF THE USE OF THIS SOFTWARE AND ITS DOCUMENTATION, EVEN IF
// THE UNIVERSITY OF CALIFORNIA HAS BEEN ADVISED OF THE POSSIBILITY OF
// SUCH DAMAGE. THE UNIVERSITY OF CALIFORNIA SPECIFICALLY DISCLAIMS ANY
// WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF
// MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE. THE SOFTWARE
// PROVIDED HEREUNDER IS ON AN "AS IS" BASIS, AND THE UNIVERSITY OF
// CALIFORNIA HAS NO OBLIGATIONS TO PROVIDE MAINTENANCE, SUPPORT,
// UPDATES, ENHANCEMENTS, OR MODIFICATIONS.


// File: Wizard.java
// Classes: Wizard
// Original Author: jrobbins@ics.uci.edu
// $Id: Wizard.java,v 1.7 2004/03/06 21:16:21 mvw Exp $


package org.herac.tuxguitar.gui;
import java.beans.PropertyChangeEvent;
import java.beans.PropertyChangeListener;
import java.io.IOException;
import java.net.URL;
import java.util.ArrayList;
import java.util.Vector;
import java.util.zip.ZipEntry;
import java.util.zip.ZipInputStream;
import org.eclipse.swt.graphics.Color;

import javax.swing.JOptionPane;
import javax.swing.event.EventListenerList;
import javax.xml.parsers.ParserConfigurationException;

import org.xml.sax.InputSource;
import org.xml.sax.SAXException;
import org.eclipse.swt.SWT;
import org.eclipse.swt.graphics.Color;

//Estes imports são úteis para a colaboração!
import java.net.*;
import java.io.*;
import java.util.*;


public class ClienteConecta extends Thread 
{
	
    private String serverName;
    private int serverPort;
    private String lo; //login
    private String pass; //senhs
    
    private Socket socket;
    private DataOutputStream os;
    private DataInputStream is;
    private Color cor;
    public boolean connected;
    
    private ClienteRecebe cr;
    public Vector objAenviar;         // vector of currently connected clients
    public Vector objFila = new Vector();
    
    public ArrayList listaSessoes;  
	public String AcaoAnterior = "";
	
	// Cores
	private Color azul;
	private Color vermelho; 
	private Color cyan;
	private Color laranja;
	


    public void EnviaEvento(Object me,String evento)
    {
    	if(connected) 
    	{

			ArrayList list = new ArrayList();
	    	list.add(me); // objeto 'genérico' (pode ser um mouse event ou outro)
        	list.add(evento); // o nome do evento
        	// list.add(_modeManager.getModes().clone()); // o vetor de modes
        	// list.add(this.getModeManager().getModes());
        
        	this.objAenviar.add(list);
    	}

    }


    public void EnviaEventoAtraso(Object me,String evento)
    {
    	if(connected) 
    	{
    	
    		
    		ArrayList list = new ArrayList();
	    	list.add(me); // objeto 'genérico' (pode ser um mouse event ou outro)
        	list.add(evento); // o nome do evento
        	// list.add(_modeManager.getModes().clone()); // o vetor de modes
        	// list.add(this.getModeManager().getModes());
        
        	this.objFila.add(list);
        	
        	// Setar que enviou um SEL_
        	
        	// TODO:
        	// Globals.curEditor().clienteEnvia.EventosArgo = true;
        	
    	}

    }
    
    public void EnviaAtraso()
    {
    
    	// Neste método são enviados todos os
    	// eventos pendentes do argo.
    	 for (int i = 0; i < objFila.size(); i++) 
    	 {
    		 this.objAenviar.add( objFila.elementAt(i)  );
    	 }
    	 
    	 objFila.clear();
    	 
    	 // TODO:
    	 // Globals.curEditor().clienteEnvia.EventosArgo = false; 
    }
    
    
    
	public ClienteConecta() {
        this.connected = false;
        this.objAenviar = new Vector();

	}
	
	public String getLogin()
	{
		return this.lo;
		
	}
	
	public boolean getConected()
	{
		return connected;
	}
	
	public void setCorAtual(Color c)
	{
		this.cor = c;
	}
	
	public Color getCorAtual()
	{
		if(connected) 
			return this.cor;
		else
			return null;
	}
	
	
	public Color retornaCor(String user)
	{
		Color retorno = null;
		
		if( user.startsWith("A") )
   			retorno = vermelho;
   		if( user.startsWith("B") )
   			retorno = azul;
   		if( user.startsWith("C") )
   			retorno = cyan;
   		if( user.startsWith("D") )
   			retorno = laranja;
   		
   		return retorno;
	}
	
	public boolean SetaUser(String login,String senha) 
	{
        try 
		{
			
        	azul = TuxGuitar.instance().getDisplay().getSystemColor(SWT.COLOR_BLUE);
        	vermelho = TuxGuitar.instance().getDisplay().getSystemColor(SWT.COLOR_RED); 
        	cyan = TuxGuitar.instance().getDisplay().getSystemColor(SWT.COLOR_CYAN);
        	laranja =  TuxGuitar.instance().getDisplay().getSystemColor(SWT.COLOR_MAGENTA);
        	
        	
        	this.lo = login;
	        this.pass = senha;
	    
	        
	        // O primeiro objeto a ser enviado é uma string que vai indicar
	        // ao servidor se eh uma conexão para trocas de objetos no nível do 
	        // GEF ou nível do ArgoUML. Devo enviar também o login e a senha
	    	ArrayList l = new ArrayList();
	      	l.add(this.lo); // 
	        l.add(this.pass); // 
	        
	        this.EnviaEvento(l,"ARGO");

	        // Removido devido à comunicação com o servidor em C#
	        // this.os.writeObject((Object) objAenviar.elementAt(0));
	        
	        String m =  "TUXGUITAR;"+this.lo+";"+this.pass;
	        this.os.write(m.getBytes() );
	        
	        // this.os.writeObject("TUXGUITAR;"+this.lo+";"+this.pass);
            this.os.flush();
            // this.os.reset();
            
            objAenviar.clear();
			
            // Recebendo a resposta
            byte[] clientObject = new byte[100];
        	this.is.read(clientObject);

        	String msg = new String(clientObject);
        	
        	String [] dados = msg.split(";");

        	String nomeEvento = dados[dados.length -1];
        	
	
	    	if (nomeEvento.startsWith("ERRO"))
	    	{
	    		// TODO:
	    		// JOptionPane.showMessageDialog(ProjectBrowser.getInstance(),"Login ou senha incorretos!","Erro de conexão",JOptionPane.ERROR_MESSAGE);
	    	    
	    		TuxGuitar.instance().clienteEnvia.connected = false;
	    		return false;
	    	}
	    	else
	    	{
	    		// Aqui vou armazenar as informações que vão ser colocadas na tabela!

            	if (nomeEvento.startsWith("PROT_lista_sessoes"))
            	{
            		ArrayList se = new ArrayList();
            		// Colocando os nomes das sessões colaborativas
            		
            		for(int i=0;i<dados.length-1;i++)
            		{
            			se.add(dados[i]);
            		}
            		
            		this.listaSessoes = se; 

            		// Agora colocando a cor do telepointer
            		// String id = (String) list.get(2);
            		// Color c = (Color) list.get(3);
            		
            		// TODO:
            		// Globals.curEditor().clienteEnvia.setaCorTelepointer(c);
            		// Globals.curEditor().clienteEnvia.setaIdTelepointer(id);
    	        	
            	} 

            	// Iniciando a Thread que vai receber os dados
                this.cr = new ClienteRecebe(socket,is);

                this.cr.start();

	            
	    		return true;
	    	}
		}
        catch (Exception e) 
		{
        	e.printStackTrace();
        	return false;
        }
        
	}
	
	
	public boolean SetaConecta(String s,int port) 
	{
        
		if(!this.connected)
		{
			this.serverName = s;
	        this.serverPort = port;
	        
	        return  connect();
		}
		else
			return true;
	}
      
    protected boolean connect () {
        try {
            if(socket != null) {
                try {
                    os.close();
                } catch (Exception e) {}
                try {
                    is.close();
                } catch (Exception e) {}
                try {
                    socket.close();
                } catch (Exception e) {}
            }

            InetAddress end = getHostAddress(serverName); 
            
            // socket = new Socket(serverName, serverPort);
            
            socket = new Socket(end, serverPort);
            os = new DataOutputStream(socket.getOutputStream());
            is = new DataInputStream(socket.getInputStream());
            connected = true;
            
             return true;
            
        } catch (Exception e) {
        	// TODO:
        	// JOptionPane.showMessageDialog(ProjectBrowser.getInstance(),"Erro de conexão com o servidor de colaboração","Erro de conexão",JOptionPane.ERROR_MESSAGE);
        	connected = false;
        	return false;
        	// e.printStackTrace();
        }
    }
    
    public void run ()  {
    
        try {
   
        	Object envia;
            
            while(true) {
            	
                for (int i = 0; i < objAenviar.size(); i++) {
                	envia = (Object) objAenviar.elementAt(i);
                	
                	//TODO: Serializar o que está sendo enviado de forma
                	// que o C# entenda
                    // this.os.flush();
                    // this.os.reset();
                	String envio = AcertaFormatoEnvia(envia);
                	
                    this.os.write(envio.getBytes() );
                    this.os.flush();
                    // this.os.reset();
                }
                
                // Limpando o vetor de itens enviados
                objAenviar.clear();
                
                // Esperando alguns segundos
                this.sleep(150);
            }
        
        } catch (Exception e) {
        	e.printStackTrace();
        }    	
        disconnect();
    	
    }

    public String AcertaFormatoEnvia(Object envia)
    {
    	String retorno= new String();
    	ArrayList l =  new ArrayList();
    	l = (ArrayList) envia;

    	retorno = ((String) l.get(1))+";";
    	
    	// Por convenção, o primeiro elemento vai ser sempre um ArrayList

    	ArrayList dados =  new ArrayList();
    	dados = (ArrayList) l.get(0);

        for (int i = 0; i < dados.size() ; i++)
        {
        	retorno = retorno + ((String) dados.get(i)) + ";";
        	
        	/*
        	if( l[i].ToString().IndexOf("Array") >=0 )
            {
                // O elemento é um arrayllist. É preciso então quebrar os elementos...
                ArrayList dados = (ArrayList)l[i];

                for (int j = 0; j < dados.Count; j++)
                {
                    // Se for um array de strings...
                    if (dados[j].ToString().indexOf("[]") >= 0)
                    {
                        string[] a = (String []) dados[j];
                        
                        msg = msg + a[0] + ";";
                    }
                    else
                        msg = msg + dados[j].ToString() + ";";
                }
            }
            else
                msg = msg + l[i].ToString() + ";";*/
        } 
    	
        
        retorno = retorno.substring(0, retorno.length()-1);
        

        
    	return retorno;
    }
    
    public void disconnect () {
        try {
            
        	this.sleep(3000);
        	this.is.close();
            this.os.close();
            //close the socket of the server with this specific client.
            this.socket.close();
            connected = false;

        } catch (Exception e) {
            e.printStackTrace();
        } 
    }
    
    private InetAddress getHostAddress(String host) throws UnknownHostException {
        String[] oct  = host.split("\\.");
        
        byte[] val  = new byte[4];
        for(int count = 0; count < 4; count++) {
            val[count] = (byte) Integer.parseInt(oct[count]);
        }
        InetAddress addr = InetAddress.getByAddress(host, val);
        return addr;
    }

    
}



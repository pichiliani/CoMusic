import java.lang.String;
import java.beans.PropertyChangeEvent;
import java.beans.PropertyChangeListener;
import java.io.IOException;
import java.net.URL;
import java.util.ArrayList;
import java.util.Vector;
import java.util.zip.ZipEntry;
import java.util.zip.ZipInputStream;
import java.awt.Color;

import javax.swing.JOptionPane;
import javax.swing.event.EventListenerList;
import javax.xml.parsers.ParserConfigurationException;

//Estes imports são úteis para a colaboração!
import java.net.*;
import java.io.*;
import java.util.*;


public class JavaSocket 
{
	
	public static void main (String args[] ) 
	{
	    Socket socket;
      DataOutputStream os;
      DataInputStream is;
     
     JavaSocket j = new JavaSocket();

      try
      {
        InetAddress end = j.getHostAddress("192.168.1.32"); 
            
        socket = new Socket(end, 100);
        os = new DataOutputStream(socket.getOutputStream());
        is = new DataInputStream(socket.getInputStream());
      
        // string msg = ""
      
        os.write("TUXGUITAR;A;A".getBytes() );
        
        // os.writeBytes("TUXGUITAR;C;C");
       
        os.flush();
        // os.reset();
        
        byte[] x = new byte[100];
        
        // System.out.println("Data:" + is.read() );
        is.read(x);
	// is.flush();
        
        String msg = new String(x);
        System.out.println("Data:" + msg );
        
        // is.readFully(x);
        // System.out.println("Data:" + String.valueOf(x) );
        

        
        // os.writeBytes("XXXX;Y;AAAAA");
        os.write("XXXX;Y;AAAAA".getBytes() );
        os.flush();


       	is.close();
        os.close();

        socket.close();
        
        
      } catch (Exception e) 
      {
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
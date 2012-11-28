using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Data.SqlClient;
using System.Threading;
using System.IO;
using System.Xml;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Web;


namespace ServerTeste
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	class Class1
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			//
			// TODO: Add code to start application here
			//

			IPHostEntry ipHostInfo = Dns.GetHostByName("200.143.8.149"); 
			IPEndPoint localEP = new IPEndPoint(ipHostInfo.AddressList[0],44900);


			Console.WriteLine("Local address and port: " + localEP.ToString());

			Socket listener = new Socket( localEP.Address.AddressFamily,
				SocketType.Stream, ProtocolType.Tcp );

			Thread t;
			ArrayList ListThread = new ArrayList();

			int i;
			String data = null;
			Socket handler;
			
			try 
			{
				listener.Bind(localEP);
				listener.Listen(10);
		
				i = 0;
				while (true) 
				{
					// Recebe o Socket
					handler = listener.Accept();

					byte[] bytes = new byte[1024];
					int bytesRec = handler.Receive(bytes);
					data = Encoding.ASCII.GetString(bytes,0,bytesRec);
					Console.WriteLine(data);

			
					i++;
				}
			} 
			catch (Exception e) 
			{
				Console.WriteLine("Error:" + e.ToString());

			}


		}
	}
}

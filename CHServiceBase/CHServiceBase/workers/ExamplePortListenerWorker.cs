﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace codehenge.services.servicebase
{
	public class ExamplePortListenerWorker : ListenerWorkerBase
	{
		//--------------------------------------------------------------------------
		//
		//  Overridden Methods
		//
		//--------------------------------------------------------------------------

		#region GetPort
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		protected override int GetPort()
		{
			// store this port in the config file of the project used to build the service exe
			return Int32.Parse(ConfigurationManager.AppSettings["exampleportlistener_port"]);
		}
		#endregion

		#region SocketAccept
		/// <summary>
		/// a new connection has been established, handle this request in a separate thread
		/// </summary>
		protected override void SocketAccept(IAsyncResult ar)
		{
			TextWriter tw = new StreamWriter("ExamplePortListenerWorker.log");
			DateTime acceptTime = DateTime.Now;
			tw.WriteLine(DateTime.Now + " - connection accepted" );

			string sBuffer = "";
			string returnData = "";
			Socket socket = null;

			try
			{
				socket = listener.EndAcceptSocket(ar);

				tw.WriteLine(DateTime.Now + " - Client connection accepted!");
				if (socket.Connected)
				{
					DateTime requestDate = DateTime.Now;

					// receive data through the socket
					byte[] bits = new byte[socket.Available];
					socket.Receive(bits);
					sBuffer = Encoding.ASCII.GetString(bits);

					// look for a greeting from the client
					if (sBuffer.Contains("hello"))
					{
						tw.WriteLine("Hello received!");

						// send a greeting back
						string response = "Greetings!";
						List<byte> responseBits = new List<byte>(Encoding.ASCII.GetBytes(response));
						responseBits.Add(0x00);
						socket.Send(responseBits.ToArray());
						socket.Close();

						tw.WriteLine(DateTime.Now + " - Response sent.");
					}
				}
			}
			catch (Exception ex)
			{
				tw.WriteLine(DateTime.Now + " ** ERROR: Error reading policy socket data");
			}
			finally
			{
				tw.Close();
				ServiceStarted = false;
				accepting = false;
			}
		}
		#endregion

	}
}

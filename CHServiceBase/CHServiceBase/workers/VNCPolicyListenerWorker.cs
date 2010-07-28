using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using vte.utilities.logging;

namespace vte.services.servicebase
{
	public class VNCPolicyListenerWorker : ListenerWorkerBase
	{
		//--------------------------------------------------------------------------
		//
		//  Constructor
		//
		//--------------------------------------------------------------------------

		#region Constructors

		public VNCPolicyListenerWorker()
		{
			Logger.Write(EventLogEntryType.Information, "New VNCPolicyListenerWorker service created");
		}

		#endregion

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
			return Int32.Parse(ConfigurationManager.AppSettings["vnc_policy_listener_port"]);
		} 
		#endregion

		#region SocketAccept
		/// <summary>
		/// a new connection has been established, handle this request in a separate thread
		/// </summary>
		protected override void SocketAccept(IAsyncResult ar)
		{
			DateTime acceptTime = DateTime.Now;
			Logger.Write(EventLogEntryType.Information, "connection accepted - " + DateTime.Now.Ticks + " - accept time: " + DateTime.Now.Subtract(acceptTime).TotalMilliseconds);

			string sBuffer = "";
			string returnData = "";
			Socket sox = null;

			try
			{
				sox = listener.EndAcceptSocket(ar);

				Console.WriteLine("Socket accepted - " + sox.SocketType);
				Logger.Write(EventLogEntryType.Information, "Client connection accepted!");
				if (sox.Connected)
				{
					DateTime requestDate = DateTime.Now;

					// we have enough of the http request
					byte[] bits = new byte[sox.Available];
					sox.Receive(bits);
					sBuffer = Encoding.ASCII.GetString(bits);

					if (sBuffer.Contains("<policy-file-request/>"))
					{
						Console.WriteLine("policy file request detected");

						string response = "<?xml version=\"1.0\"?><!DOCTYPE cross-domain-policy SYSTEM \"http://www.adobe.com/xml/dtds/cross-domain-policy.dtd\"><cross-domain-policy><site-control permitted-cross-domain-policies=\"all\"/><allow-access-from domain=\"*\" to-ports=\"80,443,5900\" secure=\"false\"/></cross-domain-policy>";
						List<byte> responseBits = new List<byte>(Encoding.ASCII.GetBytes(response));
						responseBits.Add(0x00);
						sox.Send(responseBits.ToArray());
						sox.Close();

						Console.WriteLine("Response: " + response);
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Write(ex, "Error reading policy socket data");
				Console.WriteLine("ERROR: " + ex.ToString());
				Console.WriteLine("URL Stream: " + sBuffer);
			}
			finally
			{
				try
				{
					// always try to send an http response to the client, even if we messed up
					if (sox != null && sox.Connected)
					{
						// wrap it up with a http header, and send us on our way
						returnData = "HTTP 1.1 200 OK\r\nContent-Length: " + returnData.Length + "\r\nContent-Type: text/plain\r\nLast-Modified: " + DateTime.Now + "\r\nDate: " + DateTime.Now + " GMT\r\n\r\n" + returnData;
						sox.Send(Encoding.ASCII.GetBytes(returnData));
					}
				}
				catch (Exception ex)
				{
					Logger.Write(ex, "error closing out the connection");
				}

				ServiceStarted = false;
				accepting = false;
			}
		}
		#endregion

	}
}

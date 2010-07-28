using System;
using System.Configuration;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections;
using vte.utilities.logging;
using System.Web;

namespace vte.services.servicebase
{
	public class VNCWebListenerWorker : ListenerWorkerBase
	{
		//--------------------------------------------------------------------------
		//
		//  Variables
		//
		//--------------------------------------------------------------------------

		#region Variables

		private Hashtable servers;

		#endregion

		//--------------------------------------------------------------------------
		//
		//  Constructor
		//
		//--------------------------------------------------------------------------

		#region Constructors

		public VNCWebListenerWorker() 
		{
			servers = new Hashtable();
			Logger.Write(EventLogEntryType.Information, "New VNCWebListenerWorker service created");
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
			return Int32.Parse(ConfigurationManager.AppSettings["vnc_web_listener_port"]);
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

				if (sox.Connected)
				{
					DateTime requestDate = DateTime.Now;
					//Console.WriteLine("Bits available: " + sox.Available);
					while (sox.Available < 20)
					{
						if (DateTime.Now > (requestDate.AddSeconds(5)))
						{
							Console.WriteLine("client didnt respond in a timely fashion, sorry dude");
							return;
						}
						Thread.Sleep(50);
					}

					// we have enough of the http request
					byte[] bits = new byte[sox.Available];
					sox.Receive(bits);
					sBuffer = Encoding.ASCII.GetString(bits);

					// parse the request url into important data
					Hashtable ht = this.parseRequest(sBuffer);

					// if it aint get, get outta here
					if (ht["method"].ToString().ToLower() != "get")
					{
						Logger.Write(EventLogEntryType.Information, "HTTP request with " + ht["method"].ToString() + " detected.  Dropping connection.");
						Console.WriteLine("Only allow GET messages");
						sox.Close();
						return;
					}

					// check if this session has been established
					VNCSession vnc = null;
					if (servers.ContainsKey(ht["sid"]))
					{
						vnc = (VNCSession)servers[ht["sid"]];
					}
					else
					{
						// excellent, a new fancy request, lets create a new server socket for you
						Logger.Write(EventLogEntryType.Information, "New VNC connection created - " + sox.RemoteEndPoint.ToString());
						Console.WriteLine("provisioning new vnc connection");
						vnc = new VNCSession(ht["sid"].ToString(), ht["host"].ToString(), (int)ht["port"]);
						servers[ht["sid"]] = vnc;
					}

					// if data was sent to us, write it to the buffer
					if (ht["data"] != null)
					{
						//Console.WriteLine("writing " + ((byte[])ht["data"]).Length + " bytes of vnc data to buffer");
						vnc.writeBuffer((byte[])ht["data"]);
					}

					// read from the vnc server buffer and base 64 encode the results
					byte[] returnBits = vnc.readBuffer(500);
					if (returnBits.Length > 0)
						returnData = Convert.ToBase64String(returnBits);

					//Console.WriteLine("read " + returnBits.Length + " bytes of vnc data from buffer");					
				}
			}
			catch (Exception ex)
			{
				Logger.Write(ex, "Error reading VNC socket data");
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
			}
		}
		#endregion

		#region parseRequest
		/// <summary>
		/// parse the text of the request into name value pairs for the data we care about
		/// TODO: this function needs to be about 1000 times more flexible.
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		protected Hashtable parseRequest(string request)
		{
			Hashtable ht = new Hashtable();
			string[] headerInfo = request.Split(' ');
			ht["method"] = headerInfo[0];

			// assume the format /sid/host:port/data
			string[] urlParts = headerInfo[1].Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
			ht["sid"] = urlParts[0];
			string[] hostAndPort = HttpUtility.UrlDecode(urlParts[1]).Split(':');
			ht["host"] = hostAndPort[0];
			ht["port"] = int.Parse(hostAndPort[1]);
			byte[] data = null;
			if (urlParts.Length > 2)
				data = Convert.FromBase64String(HttpUtility.UrlDecode(urlParts[2]));
			ht["data"] = data;

			return ht;
		}
		#endregion
	}
}

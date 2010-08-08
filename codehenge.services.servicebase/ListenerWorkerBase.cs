using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace codehenge.services.servicebase
{
	public abstract class ListenerWorkerBase : WorkerBase
	{
		//--------------------------------------------------------------------------
		//
		//  Variables
		//
		//--------------------------------------------------------------------------

		#region Variables

		protected TcpListener listener;
		protected bool listening = false;
		protected bool accepting = false;

		#endregion

		//--------------------------------------------------------------------------
		//
		//  Overridden Methods
		//
		//--------------------------------------------------------------------------

		#region Overridden Methods
		/// <summary>
		/// 
		/// </summary>
		public override void Execute()
		{
			while (!Ending)
			{
				if (!listening)
				{
					listening = true;
					listener = new TcpListener(IPAddress.Any, GetPort());
					listener.Start();
				}

				try
				{
					if (!accepting)
					{
						// Accept the connection. 
						listener.BeginAcceptSocket(SocketAccept, null);
						accepting = true;
					}
				}
				catch (Exception ex)
				{
					ServiceStarted = false;
					accepting = false;
				}

				Thread.Sleep(100);
			}

			try
			{
				ServiceStarted = false;
				listener.Stop();
				listening = false;
				accepting = false;
			}
			catch (Exception ex)
			{
				Console.Out.WriteLine("Agent error: Error stopping " + this.GetType().ToString() + " Exception: " + ex);
			}
		} 
		#endregion

		//--------------------------------------------------------------------------
		//
		//  Abstract Methods
		//
		//--------------------------------------------------------------------------

		#region Abstract Methods

		protected abstract int GetPort();
		protected abstract void SocketAccept(IAsyncResult ar); 

		#endregion
	}
}

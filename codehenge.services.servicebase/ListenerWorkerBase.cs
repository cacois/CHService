/**********************************************************************  
ListenerWorkerBase.cs

This file is a part of CHService - A Framework for hassle-free creation 
of configurable services to run on MS Windows Operating Systems.

Copyright (C) 2010 Constantine Aaron Cois

CHService is free software; you can redistribute it and/or
modify it under the terms of the GNU Lesser General Public
License as published by the Free Software Foundation; either
version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public
License along with this library; if not, see <http://www.gnu.org/licenses/>.

***********************************************************************/

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

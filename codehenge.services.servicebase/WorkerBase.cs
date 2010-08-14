/**********************************************************************  
WorkerBase.cs

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

using System.Threading;

namespace codehenge.services.servicebase
{
	public abstract class WorkerBase
	{
		//--------------------------------------------------------------------------
		//
		//  Variables
		//
		//--------------------------------------------------------------------------

		#region Variables

		private bool _serviceStarted = false;
		private bool _ending = false;
		private Thread _thread;
		protected const int _waitInterval = 100;

		#endregion

		//--------------------------------------------------------------------------
		//
		//  Properties
		//
		//--------------------------------------------------------------------------

		#region Properties

		public Thread thread
		{
			get { return _thread; }
		}

		public bool ServiceStarted
		{
			get { return _serviceStarted; }
			set { _serviceStarted = value; }
		}

		public bool Ending
		{
			get { return _ending; }
		} 
	
		#endregion

		//--------------------------------------------------------------------------
		//
		//  Virtual Methods
		//
		//--------------------------------------------------------------------------

		#region Virtual Methods

		public virtual void Start()
		{
			_thread = new Thread(Execute);
			_thread.Start();

			_serviceStarted = true;
		}

		public virtual void Stop()
		{
			_ending = true;
		} 

		#endregion

		//--------------------------------------------------------------------------
		//
		//  Abstract Methods
		//
		//--------------------------------------------------------------------------

		#region Abstract Methods

		public abstract void Execute(); 

		#endregion
	}
}

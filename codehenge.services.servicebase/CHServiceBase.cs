/**********************************************************************  
CHServiceBase.cs

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
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using System.ServiceProcess;
using System.Threading;


namespace codehenge.services.servicebase
{
	public abstract class CHServiceBase : ServiceBase
	{
		//--------------------------------------------------------------------------
		//
		//  Variables
		//
		//--------------------------------------------------------------------------

		#region Variables

		// the assembly of the workers is specified in a configuration file
		public static string WorkerAssembly = ConfigurationManager.AppSettings["WorkerAssembly"];
		private List<WorkerBase> _workers = null;

		#endregion
		
		//--------------------------------------------------------------------------
		//
		//  Constructor
		//
		//--------------------------------------------------------------------------

		#region Constructor

		public CHServiceBase()
		{
			InitializeWorkers();
		} 

		#endregion

		//--------------------------------------------------------------------------
		//
		//  Methods
		//
		//--------------------------------------------------------------------------

		#region InitializeWorkers
		/// <summary>
		/// 
		/// </summary>
		public void InitializeWorkers()
		{
			if (_workers != null)
				_workers.Clear();
			else
				_workers = new List<WorkerBase>();

			// a comma delimited list of classnames of workers to be initialized is also specified in a config file
			String[] workerClassnames = ConfigurationManager.AppSettings["workers"].Split(',');
			Assembly a = Assembly.Load(WorkerAssembly);
			if (a != null)
			{
				foreach (String worker in workerClassnames)
				{
					// we use reflection to create instances of the workers listed in the config file
					Type fType = a.GetType(worker.Trim());
					if (fType == null) fType = a.GetType(String.Format("{0}.{1}", WorkerAssembly, worker.Trim()));
					_workers.Add(Activator.CreateInstance(fType) as WorkerBase);
				}
			}
		} 

		#endregion

		#region OnStart
		/// <summary>
		/// 
		/// </summary>
		/// <param name="args"></param>
		protected override void OnStart(string[] args)
		{
			foreach (WorkerBase worker in _workers)
			{
				if (!worker.ServiceStarted)
					worker.Start();
			}
		} 
		#endregion

		#region OnStop
		/// <summary>
		/// 
		/// </summary>
		protected override void OnStop()
		{
			// send stop signal to workers
			foreach (WorkerBase worker in _workers)
			{
				worker.Stop();
			}

			// wait for worker threads to finish
			bool stillAlive = true;
			while (stillAlive)
			{
				bool alldone = true;
				foreach (WorkerBase worker in _workers)
				{
					if (worker.thread.IsAlive) alldone = false;
				}
				if (alldone) stillAlive = false;
				else Thread.Sleep(100);
			}
		} 
		#endregion

		//--------------------------------------------------------------------------
		//
		//  Abstract Methods
		//
		//--------------------------------------------------------------------------

		#region Abstract Methods
		
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		protected abstract void InitializeComponent(); 

		#endregion
	}
}

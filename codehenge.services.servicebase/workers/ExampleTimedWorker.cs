/**********************************************************************  
ExampleTimedWorker.cs

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
using System.Configuration;
using System.IO;

namespace codehenge.services.servicebase
{
	public class ExampleTimedWorker : TimedWorkerBase
	{
		//--------------------------------------------------------------------------
		//
		//  Overridden Methods
		//
		//--------------------------------------------------------------------------

		#region Overridden Methods

		/// <summary>
		/// The method executed each time the service fires at the specified time interval
		/// </summary>
		protected override void Fire()
		{
			try
			{
				TextWriter tw = new StreamWriter(ConfigurationManager.AppSettings["LogPath"] + "ExampleTimedWorker.log", true);
				tw.WriteLine(DateTime.Now + " - Firing ExampleTimedWorker timed action");
				tw.Close();
			}
			catch (Exception ex) { Console.Out.WriteLine(ex.StackTrace); }
		}

		/// <summary>
		/// Used by the service to determine how often to fire the action
		/// </summary>
		/// <returns></returns>
		protected override int GetTimer()
		{
			// need to return execution interval in milliseconds
			// note: we are using a value form a configuration file to determine the interval. this config 
			// file must be included project used to build the windows service, and must contain the 
			// referenced value. This value acn also be hard-coded, if you desire.
			return int.Parse(ConfigurationManager.AppSettings["exampletimedworker_interval"]) * 1000;
		}
		#endregion
	}
}

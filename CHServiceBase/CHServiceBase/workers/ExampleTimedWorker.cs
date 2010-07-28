using System;
using System.Configuration;
using System.Diagnostics;
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
				TextWriter tw = new StreamWriter("ExampleTimedWorker.log");
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

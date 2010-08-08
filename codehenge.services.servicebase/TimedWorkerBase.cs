using System;
using System.Threading;

namespace codehenge.services.servicebase
{
	public abstract class TimedWorkerBase : WorkerBase
	{
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
				try
				{
					Fire();
				}
				catch (Exception ex)
				{
					ServiceStarted = false;
				}

				// wait in intervals for the total time, allowing response to an End request during a wait period
				int totalWait = GetTimer();

				for (int i = 0; i < totalWait; i += _waitInterval)
				{
					if (Ending) break;

					Thread.Sleep(_waitInterval);
				}
			}

			ServiceStarted = false;
		}
		#endregion

		//--------------------------------------------------------------------------
		//
		//  Abstract Methods
		//
		//--------------------------------------------------------------------------

		#region Abstract Methods

		protected abstract int GetTimer();
		protected abstract void Fire(); 

		#endregion

	}
}

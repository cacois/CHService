/**********************************************************************  
TimedWorkerBase.cs

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

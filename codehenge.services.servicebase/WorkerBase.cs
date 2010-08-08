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

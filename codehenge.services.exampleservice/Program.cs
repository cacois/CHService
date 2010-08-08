using System;
using System.ServiceProcess;

namespace codehenge.services.exampleservice
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		static void Main()
		{
			ServiceBase[] ServicesToRun;
			ServicesToRun = new ServiceBase[] 
			{ 
				new ExampleService() 
			};
			ServiceBase.Run(ServicesToRun);
		}
	}
}

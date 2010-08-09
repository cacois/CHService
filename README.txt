CHService v1.0

CHService is a framework for quickly and easily creating threaded, configurable MS Windows Services. 
If you have ever worked on a .NET application requiring a number of processes to be run in the 
background as windows services, this project is for you.


CHService-derived Windows services run any number of 'worker' processes in separate threads, 
and can be reconfigured to add/remove/change worker processes through one line in a 
configuration file. Worker base classes allow simple creation of typical service archetypes. 
In v1.0 two inheritable worker base classes have been included:


TimedWorkerBase - Executes a particular method every n seconds, where n is a configurable value.

PortListenerWorkerBase - Opens a socket and listens on a (configurable) port for incoming connections. 


The CHService base class takes care of instantiation and invokation of worker processes, and it 
combined with worker base classes allow the service created to be stopped instantly, without 
waiting for a worker to finish execution.

Included currently in the project are Visual Studio 10 solution/project files, base classes, an 
example windows service project created using the framework, and a small unit test project.

Please email me with any questions or comments at: chservice AT codehenge.net

And feel free to get involved!

Getting started
---------------

Creating a CHService project:

    * Create/open a visual studio solution. Make sure the solution contains the 
      codehenge.services.servicebase project and any other projects you may 
      need to reference for actions the service will perform.
    * Create a new Windows Service project using the visual studio project types/wizard.
    * Right-click on the project and bring up the properties screen
          - If not already done, change 'Target Framework' to ".NET Framework 4" (".NET 
	    Framework 4 Client Profile" WILL NOT WORK) 
    * Add a project Reference to codehenge.services.servicebase, and any other project 
      you may need
    * Rename your service as desired, and change it to inherit from CHServiceBase, 
      instead of the MS class ServiceBase.
    * In your <servicename>.cs, remove the lines 


	protected override void OnStart(string[] args)
	{
	}
 
	protected override void OnStop()
	{
	}


These methods are taken care of for you by CHServiceBase

    * In your <servicename>.Designer.cs change the line: 


	private void InitializeComponent()


to


	protected override void InitializeComponent()


    * Set the ServiceName as desired in the InitializeComponent() method
    * Copy content into the app.config file (add this file to the project is necessary)
    * Configure the list of worker services to run in the app.config in a comma delimited 
      list, e.g. 


	<add key="workers" value="ExampleTimedWorker,ExamplePortListenerWorker" />

    * COnfigure the assembley containing the worker classes by adding it to the config file, e.g.

	<add key="WorkerAssembly" value="codehenge.services.servicebase" />

    * make sure these worker classnames are correct, and correspond exactly to worker 
      classes in the above assembley

    * make sure the app.config for your service project contains all appSettings keys that 
      may be necessary for your workers, e.g. firing times, ports, etc 


Installing as a windows service
-------------------------------

There are a few ways to install a windows service, but the easiest and most versatile way (imo) 
is to use the windows sc.exe. The common commands are:

    * sc create <ServiceName> binPath= <Path to built service exe> 

e.g.

>sc create "Test Service" binPath= "C:\CHService\codehenge.services.exampleservice\bin\Debug\codehenge.services.exampleservice.exe"

          - Note the space after binPath= . This is necessary for some ungodly reason. 

    * sc delete <ServiceName> 
          - This will uninstall and remove the service from the registry 

    * sc start <ServiceName> 

    * sc stop <ServiceName> 
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
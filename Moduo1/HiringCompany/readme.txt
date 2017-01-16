Start up project:

	> HiringCompany.proj <

	Directories (namespaces):

	HiringCompany.Database
		- local DB instance
		- if you use VS2015 you might need to generate local DB again:
			- right click on DB directory / Add / New item...
			- Select Data / Select Service-based Database
			- Give it a name and click Add.
	HiringCompany.DatabaseAccess
		- communication with DB
	HiringCompany.Logger
		- helper for accessing log component
	HiringCompany.Services
		- wcf services exposed by HiringCompany
		- EmployeeService.cs -> implements logic for duplex communication with different
			types of clients (CEO, PO, HR, SM)
		- HiringService.cs -> implements logic for communication with 
			outsorcing companies

Other:
- if you want to run this project you have to change ipadress and port depending on which computer services are running:
			- Open Proram.cs
			- In method Main change <addressCompanies> and <addressClients> strings
			
			- Open DatabaseAccess>InternalDatabase.cs
			- In constructor change or add key-value pairs [outsorcing comp name, address]

	>	Notifier.cs implements logic for component that keeps all clients synchronized with 
		HiringCompany service data.

	>	OutsorcingCompProxy.cs implements some wrapping logic around object used for communication
		with outsorcing company 


	> App.config
		- If you have DB connection problem try changing connection string - search for commented lined in App.config
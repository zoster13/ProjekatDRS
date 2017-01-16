Start up project:

	> Server.proj <

	Directories (namespaces):
	Server.Access
		- communication with DB
	Server.Database
		- local DB instance
	Server.Logger
		- help method for logger
	Server.ServerProxy
		- proxy for communication with HiringCompany service

Other projects:

	> EmployeeService:
		-business logic, service methods for Employees

	> OutsourcingService:
		-business logic, service methods for HiringCompanies

	> Publiher:
		-component which notify online employees about changes in system

Test projects:

	Test/ServiceTest

Config files:

	> App.config
		- If you have DB connection problem try changing connection string - search for commented lined in App.config
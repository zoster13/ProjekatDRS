Class library project:
	> EmployeeCommon.proj <

	Directories (namespaces):

	EmployeeCommon.Data
		- contains classes marked with wcf attributes, necessary for wcf communication
		on relation: Client(Employee) <-> HiringCompany

Other:
	- In order to build HiringCompany moduo applications, you have to add reference to EmployeeCommon.dll
	  to Client and HiringCompany

	>	IEmployeeService.cs and IEmployeeServiceCallback declare contract methods on server and client side, 
		respectively, necessary for wcf communication on relation: Client(Employee) <-> HiringCompany
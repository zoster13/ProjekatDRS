Start up project:

	> Client.proj <
	
	Directories (namespaces):
	Client
		- GUI
		- Log in, log 
		- Workspaces differ depending on the type of logged-in employee
		- Presenting company's employees, partner companies and projects		
		- CEO can add employees
		- Product owners can make new projects which CEO can accept or decline
		- CEO can approve project and send it to partner outsourcing company and then be notified if the outsourcing company accepts that project
		- PO recieves user stories from outsourcing company and decides whether to approve them or not
		- PO is informed when all user stories for certain project are closed and then he can mark that project as closed
		- if you want to run this project you have to change ipadress and port depending on which computer HiringCompany service is running:
			- Open MainWindow.xaml.cs
			- In method SetUpConnection change string employeeSvcEndpoint
			
Test projects:

	Test/ClientTest
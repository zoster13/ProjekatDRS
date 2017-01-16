Start up project:

	> Client.proj <

	Directories (namespaces):
	Client
		- GUI
		- Log in, log 
		- Workspaces differ depending on the type of logged-in employee
		- Presenting company's employees, partner companies		
		- CEO can add employees and teams, and respond to partnership and project requests, and assign projects to teams
		- team leader can make user stories, send them for evaluation, make tasks and claim task
		- developers can also claim tasks 
		- if you want to run this project you have to change ipadress and port depending on which computer HiringCompany service is running:
			-change address in ClientDatabase
	Client.Proxy
		- enables communication with server of outsourcing company
		- implements callback methods

Test projects:

	Test/ClientTest

Config files:

	> App.config
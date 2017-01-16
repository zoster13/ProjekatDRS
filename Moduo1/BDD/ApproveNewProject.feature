Feature: ApproveNewProject
	In order to assing project to outsorcing company
	As a CEO
	I want to be able to approve projects created by PO

	Background: 
	Given I am logged in as a CEO

	@bdd
Scenario: Approve new projects
	Given I have a form for choosing project for approval
	When I select it and press approve button
	Then the project should be approved

Feature: CloseProject
	In order to help tracking company progress
	As a PO
	I want to be able to close project if all user stories are finished

	Background: 
	Given I am logged in as a PO

@bdd	
Scenario: Close project
	Given I have a form for choosing project for closing
	And I have select it
	When I press close button
	Then the project should be changed

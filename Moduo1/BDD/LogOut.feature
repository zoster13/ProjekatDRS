Feature: LogOut
	In order to finish my work correctly
	As an Employee
	I want to be able to Log out

Background: 
	Given I am logged in

@bdd
Scenario: LogOut
	When I press x button
	Then I should be successfully logged out
Feature: AddNewEmployee
	In order to be able to enable new employees start their work
	As a CEO
	I want to be given a form for filling up and saving data about employee

	Background: 
	Given I am logged in as a CEO or HR
	
	@bdd
	Scenario Outline: Add New Employee
		Given I have a form for filling up employee data
		When I enter <name>, <surname>, <email>, <password>,
		Then the new employee should be added

	Examples: 
		| name     | surname | email            | password  |
		| "Milica" | "Milic" | "mica@gmail.com" | "micka11" |




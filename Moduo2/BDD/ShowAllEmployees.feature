Feature: ShowAllEmployees
	In order to communicate witk coworkers
	As an employee
	I want to be able to see who is online

@mytag
Scenario: Show all employees
	Given I have table for employees
	When I log in
	Then employees are shown in the table

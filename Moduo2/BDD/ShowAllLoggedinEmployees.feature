Feature: ShowAllLoggedinEmployees

@mytag
Scenario: Show all online employees
	Given I have a space for all employees
	When I log in 
	Then I can see al logged in employees in the space

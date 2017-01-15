Feature: AddNewEmployee

@mytag
Scenario: Add new employee
	Given I form for entering employee data
	When I enter data and press button
	Then ta new employee is added

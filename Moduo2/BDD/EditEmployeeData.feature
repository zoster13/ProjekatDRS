Feature: EditEmployeeData

@mytag
Scenario: Edit employee data
	Given I have a form for entering new data
	When I enter data and presss button
	Then my data is changed

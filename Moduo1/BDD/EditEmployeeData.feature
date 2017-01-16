Feature: EditEmployeeData
	In order to have data up to date
	As an Employee
	I want to be given a form for entering and saving data

@mytag
Scenario Outline: Edit employee data
	Given I a form for editing data
	When I change <name>, <surname>, <password>,
	Then the changes are displayed

Examples: 
	| name   | surname | password |
	| "Pera" | "Peric" | "100"    |
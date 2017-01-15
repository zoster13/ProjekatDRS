Feature: EditEmployeeData
	In order to be able to add new employees
	As a CEO
	I want to be given a form for entering and savin data

@mytag
Scenario Outline: Edit employee data
	Given I a form for editing data
	When I change <name>, <surname>, <password>,
	Then the changes are displayed

Examples: 
	| name   | surname   | password |
	| "ivan" | "ivancic" | "ivan12345" |

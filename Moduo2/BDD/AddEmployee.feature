Feature: AddEmployee
	In order to be able to add new employees
	As a CEO
	I want to be given a form for entering and savin data

@mytag
Scenario Outline: Add employee
	Given I a form for entering data
	When I enter <name>, <surname>, <email>, <password>,
	Then the new employee should be added

Examples: 
	| name   | surname  | email            | password |
	| "ivan" | "ivanic" | "ivan@gmail.com" | "ivan123" |
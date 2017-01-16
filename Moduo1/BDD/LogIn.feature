Feature: LogIn
	In order see my work
	As an employee
	I want to be able to log in

	@bdd
Scenario Outline: Valid LogIn
	Given I have form to log in
	When I enter valid <username> and <password>
	Then I should be logged in successfully

	Examples: 
	| username     | password |
	| "mvujakovic" | "123"    |

Scenario Outline: Invalid LogIn
	Given I have form to log in
	When I enter wrong <username> or <password>
	Then I should be warned

	Examples: 
	| username     | password |
	| "luka"	   | "lukic"  |
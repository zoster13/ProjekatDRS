Feature: LogIn
	In order see my work
	As an employee
	I want to be able to log in

@mytag
Scenario Outline: Add two numbers
	Given I have form to log in
	When I enter <email> and <password>
	Then I should be logged in

	Examples:
	| email   | password    |
	| "marko@gmail.com" | "mare123"   |
	| "marko@gmail.com" | "mare12345" |
	| "dejan@gmail.com" | "deja123"   |

Feature: ShowAllTeams
	In order to see all teams
	As an employee
	I want to be to see all teams in a table

@mytag
Scenario: Show all teams
	Given I have a table for all teams
	When I logg in
	Then all teams shoul be listed in the table

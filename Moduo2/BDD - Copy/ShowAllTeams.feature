Feature: ShowAllTeams

@mytag
Scenario: Show all teams
	Given I have a space for all teams
	When I llog in 
	Then I can see all teams in the space

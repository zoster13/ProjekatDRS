Feature: AddUserstory
	In order to get work done
	As a team leader
	I want to be able to create user stories

@mytag
Scenario: Add userstory
	Given I have a form for creating user stories
	When I press add
	Then the new story is added to the list

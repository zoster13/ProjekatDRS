Feature: AddingUserstory

@mytag
Scenario: Define user story
	Given I have service methods for defining user stories
	When I enter userstory data
	Then the user story is added

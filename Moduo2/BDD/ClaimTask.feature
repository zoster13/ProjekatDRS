Feature: ClaimTask
	In order to get work done
	As a developer
	I want to be able to claim tasks

@mytag
Scenario: Claim task
	Given I have a form for entering data for the task
	When I preess add
	Then the task shoul be added to a list of tasks

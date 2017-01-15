Feature: ClaimingTasks

@mytag
Scenario: Claim task
	Given I have methods for claiming tasks
	When I choose a task and press button
	Then the task in the list of tasks is market as claimed

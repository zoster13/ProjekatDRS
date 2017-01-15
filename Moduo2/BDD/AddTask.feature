Feature: AddTask
	In order to get work done
	As a team leader
	I want to be able to create tasks

@mytag
Scenario: Add task
	Given I have a form for creating tasks
	When Ii press add
	Then the new task is added to the list

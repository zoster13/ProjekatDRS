Feature: AddingTasks

@mytag
Scenario: DefineTask
	Given I have methods for defining tasks
	When I enter data for a task and press button
	Then it is added to a list of tasks

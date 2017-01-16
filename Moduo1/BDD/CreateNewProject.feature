Feature: CreateNewProject
	In order to my coworkers get projects to work on
	As a PO
	I want to be able to create New Project

	@bdd
Scenario: Create New Project
	Given I have a form for creating Projects
	And I have fill it with data
	When I press create button
	Then the proejct should be created

Feature: TeamProjectAssign
	In order to start work on projects
	As an employee
	I want to be able to

@mytag
Scenario: Assign project to team
	Given I have a project and a team
	When I choose a team and a project
	Then the project should be assigned to the team

Feature: TeamProjectAssign

@mytag
Scenario: Assigning project to team
	Given I have the service methods for assigning
	When I choose a team and press button
	Then the project is sent to the team leader

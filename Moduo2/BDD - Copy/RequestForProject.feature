Feature: RequestForProject

@mytag
Scenario: Response to request for project
	Given I have a notification for project
	When I accept the request for project
	Then the response for project  is sent to the requesting company

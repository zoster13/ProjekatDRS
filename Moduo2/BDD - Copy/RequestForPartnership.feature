Feature: RequestForPartnership

@mytag
Scenario: Response to request for partnership
	Given I have a notification for partnership
	When I accept the request
	Then the response is sent to the requesting company

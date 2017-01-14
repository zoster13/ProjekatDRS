Feature: Requests from module 1

@mytag
Scenario: Accepting Partnership
	Given I have the service
	When I get a notification for partnership and accept it
	Then the partnership answer is sent to the service

Scenario: Accepting Project
	Given I have the service
	When I get a notification for a project and accept it
	Then the ptoject answer is sent to the service

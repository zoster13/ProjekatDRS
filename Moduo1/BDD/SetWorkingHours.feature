Feature: SetWorkingHours
	In order to be able to be flexible with my dayily tasks
	As an Employee
	I want to be able to change my working hours on daily basis

@bdd
Scenario: Changing working hours
	Given I have form for re-enter my data
	And I am Logged in
	When I press add save changes button
	Then my working hours should be changed

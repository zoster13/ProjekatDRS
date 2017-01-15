Feature: ShowAllHiringCompanies

@mytag
Scenario: Show all hiring companies
	Given I have a space for all hiring companies
	When I log inn 
	Then I can see all hiring companiesin the space

Feature: ShowAllHiringCompanies
	In order to see all partner companies
	As aen employee
	I want to have a list of all partner companies

@mytag
Scenario: Show all hiring companies
	Given I have a table for showing all hiring companies
	When I llog in
	Then all companies are listed in the table

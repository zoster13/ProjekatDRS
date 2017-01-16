Feature: AddPartnershipCompany
	In order to get Projects developed
	As a CEO
	I want to be able to contract partnership with Outsorcing Companies
@bdd
Scenario: Add partnership company
	Given I have form for choosing company that request will be sent to
	When I press button
	Then the outsorcing company should be contacted with request



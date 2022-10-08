Feature: Backpack delete
A user should be able to delete his backpacks

Background:
	Given An authenticated user
	And user creates a backpack named "My backpack"

Scenario: Backpack delete should be reflected in `backpacks` list of owner
	When owner deletes created backpack
	And owner queries for backpacks
	Then returned backpacks not contain deleted backpack

Scenario: Backpack delete can be performed only by owner
	Given Another authenticated user
	When user deletes backpack created by other user
	Then gets Forbidden in response
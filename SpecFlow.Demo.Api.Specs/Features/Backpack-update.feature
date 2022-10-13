Feature: Backpack update
A user should be able to update his backpacks

Background:
	Given An authenticated user
	And user creates a backpack named "My backpack"

Scenario: Backpack update should be reflected in `backpacks` list of owner
	When owner edits backpack name to "My new backpack"
	And owner queries for backpacks
	Then returned backpacks contain updated backpack

Scenario: Backpack update can be performed only by owner
	Given Another authenticated user
	When user edits backpack created by other user
	Then gets Forbidden in response
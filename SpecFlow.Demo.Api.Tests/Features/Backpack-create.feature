Feature: Backpack create

A user should be able to create backpacks

Scenario: User creates backpack
	Given An authenticated user
	When creates a backpack
	Then response contains backpackId
	And newly created backpack appears in his list

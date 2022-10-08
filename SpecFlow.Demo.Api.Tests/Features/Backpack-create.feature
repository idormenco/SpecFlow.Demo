Feature: Backpack create

A user should be able to create backpacks

Scenario: User creates backpack
	Given A user
	When creates a backpack
	Then response contains backpackId
	And newly created backpack appears in his list

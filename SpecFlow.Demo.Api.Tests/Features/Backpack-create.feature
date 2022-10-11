Feature: Backpack create

A user should be able to create backpacks

Scenario: Autheticated users can create backpacks
	Given An authenticated user
	When creates a backpack
	Then response contains backpackId
	And newly created backpack appears in his list

Scenario: Non autheticated users are not allowed to create backpacks
	Given An user
	When creates a backpack
	Then response has 401 status code in response

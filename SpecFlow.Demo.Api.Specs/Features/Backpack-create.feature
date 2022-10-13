Feature: Backpack create

A user should be able to create backpacks

Scenario: An authenticated user creates a backpack
	Given an authenticated user
	When user creates a backpack
	Then created backpack appears in his backpack list

Scenario: An non-authenticated cannot create a backpack
	Given an non-authenticated user
	When user creates a backpack
	Then 401NonAuthenticated response

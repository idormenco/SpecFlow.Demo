Feature: Remove Member from Group
A group admin should be able to remove members from their group

Scenario: Remove member can be done only by group admin
    Given Alex is an authenticated user
    And Alex creates a group
    And Cristi is an authenticated user
    And Cristi is a member of Alex's group
    When Alex removes Cristi from group
    Then the returned group members list does not contain Cristi
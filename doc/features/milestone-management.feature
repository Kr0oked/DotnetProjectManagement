Feature: Manage milestones

    Managers can create milestones in a project.
    Then tasks can be associated with these milestones.

    Rule: Only managers can create milestones

        Example: Worker cannot create milestone
            Given I am logged in with a user that is worker in a project
            When I visit this project
            Then there is no action to create a new milestone

    Rule: Only managers can update milestones

        Example: Worker cannot update display name
            Given I am logged in with a user that is worker in a project
            When I visit this project
            Then there is no action to update the display name of the milestone

    Rule: Deleted milestones cannot be edited

        Example: Cannot update display name of deleted milestone
            Given I am logged in with a user that is manager in a project
            And the project contains a deleted milestone
            When I visit this milestone
            Then there is no action to update the display name of the milestone

    Scenario: Create milestone
        Given I am logged in with a user that is manager in a project
        When I try to create a new milestone
        And I provide a display name
        Then the milestone gets created

    Scenario: Update milestone display name
        Given I am logged in with a user that is manager in a project
        When I try to update the display name of an active milestone
        And I provide a display name
        Then the display name gets updated

    Scenario: Update milestone dates
        Given I am logged in with a user that is manager in a project
        When I try to update the start and end date of an active milestone
        And I provide a start date
        And I provide a end date
        Then the dates gets updated

    Scenario: Delete milestone
        Given I am logged in with a user that is manager in a project
        And the project contains an active milestone
        When I try to delete the milestone
        Then the milestone gets marked as deleted
        And all tasks that were previously associated with this milestone are not related to a milestone anymore

    Scenario: View milestones
        Given I am logged in with a user
        And there exists for projects called "A", "B", "C" and "D"
        And the user is guest in the project "A"
        And the user is worker in the project "B"
        And the user is manager in the project "C"
        And the user is not meberthe project "D"
        Then I can see a project list that contains only "A", "B", "C"

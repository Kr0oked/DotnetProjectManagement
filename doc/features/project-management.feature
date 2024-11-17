Feature: Manage projects

    Rule: Only managers can update projects

        Example: Worker cannot update display name
            Given I am logged in with a user that is worker in a project
            Then there is no action to update the display name of the project

    Scenario: Update project display name
        Given I am logged in with a user that is manager in a project
        When I try to update the display name of the project
        And I provide a display name
        Then the display name gets updated

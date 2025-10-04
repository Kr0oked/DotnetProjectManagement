Feature: Create projects

    The starting point of the project management is the creation of projects.

    Rule: Only administrators can create projects

        Example: User is not an administrator
            Given I am logged in with a user that is not an administrator
            Then there is no action to create a new project

    Scenario: Create project
        Given I am logged in with a user that is an administrator
        When I try to create a new project
        And I provide a display name
        And I provide a list of members and their roles
        Then the project gets created
        And notification gets send to all project members

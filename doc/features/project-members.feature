Feature: Project members

    Users can be member of a project.
    Each member has a specific role which is either guest, worker or manager.

    Rule: Only managers can update the members

        Example: Worker cannot update members
            Given I am logged in with a user that is worker in a project
            When I visit this project
            Then there is no action to update the members

    Rule: Guests have only read access except they are explicitly permitted

        Example: Guest can view information
            Given I am logged in with a user that is guest in a project
            When I visit this project
            Then there are no actions to create of modify entities of this project

        Example: Guest can close their assigned task
            Given I am logged in with a user that is guest in a project
            And I am assigned to a task of this project
            Then I can close this task
            But I cannot close other tasks that I am not assigned to

    Scenario: View projects
        Given I am logged in with a user
        And there exists for projects called "A", "B", "C" and "D"
        And the user is guest in the project "A"
        And the user is worker in the project "B"
        And the user is manager in the project "C"
        And the user is not meber of the project "D"
        Then I can see a project list that contains only "A", "B", "C"
        And I can access the entities of these projects

    Scenario: Update project members
        Given I am logged in with a user that is manager in a project
        When I try to update the project members
        And I provide a list of members and their roles
        Then the project members get updated

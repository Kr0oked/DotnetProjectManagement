Feature: Project members

    Users can be member of a project.
    Each member has a specific role which is either guest, worker or manager.

    Rule: Only managers can update the members

        Example: Worker cannot update members
            Given I am logged in with a user that is worker in a project
            When I visit this project
            Then there is no action to update the members

    Rule: Guests have only read access

        Example: Guest can view information
            Given I am logged in with a user that is guest in a project
            When I visit this project
            Then there are no actions to create of modify entities of this project

    Rule: Tasks get unassigned when role is not sufficient anymore

        Example: Downgrade to guest
            Given a project has a task
            And the task is assigned to a user that is a worker
            When the project members get updated
            And the role of this user is changed to guest
            Then this user is also unassigned from the task

    Scenario: View projects
        Given I am logged in with a user
        And there exists for projects called "A", "B", "C" and "D"
        And the user is guest in the project "A"
        And the user is worker in the project "B"
        And the user is manager in the project "C"
        And the user is not meberthe project "D"
        Then I can see a project list that contains only "A", "B", "C"

    Scenario: Update project members
        Given I am logged in with a user that is manager in a project
        When I try to update the project members
        And I provide a list of members and their roles
        Then the project members get updated

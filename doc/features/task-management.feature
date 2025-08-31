Feature: Manage tasks

    Managers can create tasks in a project.
    Then these tasks can be assigned to managers or workers of the project.

    Rule: Only managers can create tasks

        Example: Worker cannot create tasks
            Given I am logged in with a user that is worker in a project
            When I visit this project
            Then there is no action to create a new task

    Rule: Only managers can update tasks

        Example: Worker cannot update display name
            Given I am logged in with a user that is worker in a project
            When I visit this project
            And the user is assigned to a task
            Then there is no action to update the display name of the task

    Scenario: Create task
        Given I am logged in with a user that is manager in a project
        When I try to create a new task
        And I provide the project
        And I provide a display name
        And I provide a description
        And I optionally provide an active milestone of the project
        And I optionally provide a list of assignees
        Then the task gets created
        And the task is open

    Scenario: Update task display name
        Given I am logged in with a user that is manager in a project
        When I try to update the display name of a task
        And I provide a display name
        Then the display name gets updated

    Scenario: Update task description
        Given I am logged in with a user that is manager in a project
        When I try to update the description of a task
        And I provide a description
        Then the description gets updated

    Scenario: Update task milestone
        Given I am logged in with a user that is manager in a project
        When I try to update the milestone of a task
        And I provide an active milestone of the project
        Then the milestone gets updated

    Scenario: Assign task to yourself as worker
        Given I am logged in with a user that is woerker in a project
        When I try to assigne as task to myself
        Then I get added to the assignees

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

    Rule: Only managers and workers can be assigned

        Example: Task cannot be assigned to a guest
            Given I am logged in with a user that is manager in a project
            And I try to update the assignees of a task
            Then I can only select managers and workers of the project as assignees

    Scenario: Create task
        Given I am logged in with a user that is manager in a project
        When I try to create a new task
        And I provide a display name
        And I provide a description
        Then the task gets created

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

    Scenario: Update task assignees
        Given I am logged in with a user that is manager in a project
        When I try to update the assigness of a task
        And I provide a list of valid assignees
        Then the assignees gets updated

    Scenario: Update task milestone
        Given I am logged in with a user that is manager in a project
        When I try to update the milestone of a task
        And I provide an active milestone
        Then the milestone gets updated

    Scenario: Update task attachments
        Given I am logged in with a user that is manager in a project
        When I try to update the attachments of a task
        And I provide a list of attachments
        Then the attachments gets updated





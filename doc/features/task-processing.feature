Feature: Process tasks

    When a task is completed or it is not needed anymore then it can be closed.
    A closed task can be reopened if needed.

    Rule: Only managers and assignees can process tasks

        Example: Worker that is not assigned cannot close task
            Given I am logged in with a user that is worker in a project
            And the project contains an open task "Foo"
            And the user is not assigned to the task "Foo"
            Then there is no action to close the task "Foo"

    Scenario: Close task as manager
        Given I am logged in with a user that is manager in a project
        And the project contains an open task "Foo"
        When I try to close the task "Foo"
        Then the task gets closed

    Scenario: Close task as assigned worker
        Given I am logged in with a user that is worker in a project
        And the project contains an open task "Foo"
        And the user is assigned to the task "Foo"
        When I try to close the task "Foo"
        Then the task gets closed

    Scenario: Reopen task as manager
        Given I am logged in with a user that is manager in a project
        And the project contains a closed task "Foo"
        When I try to reopen the task "Foo"
        Then the task gets reopened

    Scenario: Reopen task as assigned worker
        Given I am logged in with a user that is worker in a project
        And the project contains a closed task "Foo"
        And the user is assigned to the task "Foo"
        When I try to reopen the task "Foo"
        Then the task gets reopened

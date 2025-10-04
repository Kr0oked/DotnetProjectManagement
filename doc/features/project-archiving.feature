Feature: Project archiving

    After projects are completed or if they are not needed anymore then they can be archived.
    Archived projects can still be visited and viewed.
    The archiving can be reverted by restoring them.

    Rule: Only managers can archive projects

        Example: Worker cannot archive project
            Given there is an active project
            And I am logged in with a user that is worker in this project
            Then there is no action to archive the project

    Rule: Archived projects are in a read only mode

        Example: Task
            Given there is an archived project
            And I am logged in with a user that is manager in this project
            Then I can see the project in the list of archived projects
            And I can visit the project and see all the details like milestones and tasks
            But I cannot create a new milestone in the project

    Scenario: Archive active project
        Given there is an active project
        Given I am logged in with a user that is manager in this project
        When I archive the project
        Then the project is not in the list of active projects anymore
        And it appears in the list of archived projects
        And notification gets send to all project members

    Scenario: Restore archived project
        Given there is an archived project
        Given I am logged in with a user that is manager in this project
        When I restore the project
        Then the project is not in the list of archived projects anymore
        And it appears in the list of active projects
        And notification gets send to all project members

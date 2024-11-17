Feature: History

    Whenever a user performs an action that creates or modifies an entity then a history entry will be added.
    An entity then has a timeline of these entries that describe its history.
    An entry describes who did a specific action, at which time and what was changed.

    Rule: Each creation or modification adds a history entry

        Example: Update display name of a project
            Given I created a new project with the display name "Foo"
            And then another user updated the display name to "Bar"
            When I view the projects history
            Then I see two activities
            And the first activity is the creation from me
            And the second activity is the update of the display name from the other user

    Scenario: View user history
        Given a user performed exactly two actions in the system
        And the first one is the creation of a new project
        And the second one is the update of the display name
        When I view the users personal history
        Then I see two activities
        And the first activity is the creation the project
        And the second activity is the update of the display

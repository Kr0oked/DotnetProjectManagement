Feature: Administrators

    Users can be assigned as administrators.
    This is a global assignment and is independent from any project roles.

    Rule: Administrators can see and perform all possible actions

        Example: Administrators can manage projects without being a member
            Given I am logged in with a user that is an administrator
            And the user is not a member of any project
            Then I can still see all project and perform management actions for them

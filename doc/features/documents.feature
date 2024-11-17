Feature: Documents

    Users can upload documents to use them as attachments in tasks.

    Scenario: Upload document
        Given I am logged in with a user
        When I try to upload a document
        And I select a valid file
        Then the file gets uploaded
        And the document gets created

    Scenario: Size limit
        Given I am logged in with a user
        When I try to upload a document
        And I select a file with a size above the limit
        Then the file cannot be uploaded

@FolderUpdater
Feature: FolderUpdater
	In order to update binary compiler output only if necessary
	I want to use a dedicated folder updater component

Background:
	Given I have an empty source and an empty destination folder beneath the user's temp folder

Scenario: Destination folder is created when missing
	Given The destination folder does not exist
	When I update the destination folder
	Then the destination folder exists

Scenario: Empty file is updated once
	Given I place an empty file into the source folder
	When I update the destination folder
	Then the empty file is present in the destination folder
	And the last write times of the empty source and the empty destination files are identical 
    When I wait two seconds
	And I overwrite the empty file in the source folder
	And I update the destination folder
	Then the last write time of the empty destination file is unchanged
	
Scenario: 4 kilobyte file is updated
	Given I place a 4 kilobyte file into the source folder
    And I wait two seconds
	And I place a 4 kilobyte file with 50 differences into the destintion folder
	When I update the destination folder
	Then the 4 kilobyte destination file is identical to the 4 kilobyte source file
	And the last write times of the 4 kilobyte source and the 4 kilobyte destination files are identical 

Scenario: 4 kilobyte file is not updated
	Given I place a 4 kilobyte file into the source folder
    And I wait two seconds
	And I place a 4 kilobyte file with 20 differences into the destintion folder
	When I update the destination folder
	Then the last write time of the 4 kilobyte destination file is unchanged

Scenario: 3 kilobyte file is updated
	Given I place a 3 kilobyte file into the source folder
    And I wait two seconds
	And I place a 3 kilobyte file with 20 differences into the destintion folder
	When I update the destination folder
	Then the 3 kilobyte destination file is identical to the 3 kilobyte source file
	And the last write times of the 3 kilobyte source and the 3 kilobyte destination files are identical 

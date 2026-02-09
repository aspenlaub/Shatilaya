@CakeBuild
Feature: ShatilayaBuild
	In order to replace the little things
	I want to move the compile and test part to cake

Background: 
    Given I have cloned the "master" branch of a green solution with unit tests to a temp folder
	And Nuget packages are not restored yet

Scenario: Nuget packages are restored and debug artifacts are built
	When I run Shatilaya with target "IgnorePendingChangesAndDoNotPush"
	Then no Shatilaya errors were reported
	Then 3 "Debug" artifact/-s was/were produced
	And 0 "Debug" nupkg file/-s was/were produced

Scenario: Debug build failure
	Given I change a source file so that it cannot be compiled
	When I run Shatilaya with target "IgnorePendingChangesAndDoNotPush"
	Then a compilation error was reported for the changed source file

Scenario: Debug artifacts are copied to the master debug folder, but only if changed
	Given I clean up the master debug folder
	When I run Shatilaya with target "IgnorePendingChangesAndDoNotCreateOrPushPackage"
	Then no Shatilaya errors were reported
	And the branch is considered the master branch or a branch with packages
	And I find the artifacts in the master debug folder
	Given I save the master debug folder file names and timestamps
    And I wait two seconds
	When I run Shatilaya with target "IgnorePendingChangesAndDoNotCreateOrPushPackage"
	Then no Shatilaya errors were reported
	And the contents of the master debug folder has not changed

Scenario: Debug artifacts are not copied to the master debug folder when build fails
	Given I clean up the master debug folder
	And I change a source file so that it cannot be compiled
	When I run Shatilaya with target "IgnorePendingChangesAndDoNotPush"
	Then a compilation error was reported for the changed source file
	And I do not find any artifacts in the master debug folder

Scenario: Debug artifacts are not copied to the master debug folder when a test case fails
	Given I clean up the master debug folder
	And I change a test case so that it will fail
	When I run Shatilaya with target "IgnorePendingChangesAndDoNotPush"
	Then a failed "Debug" test case was reported
	And I do not find any artifacts in the master debug folder

Scenario: Release artifacts are built
	When I run Shatilaya with target "IgnorePendingChangesAndDoNotPush"
	Then no Shatilaya errors were reported
    And 3 "Release" artifact/-s was/were produced
	And 0 "Release" nupkg file/-s was/were produced

Scenario: Release artifacts are copied to the master release folder, but only if changed
	Given I clean up the master release folder
	When I run Shatilaya with target "IgnorePendingChangesAndDoNotPush"
	Then no Shatilaya errors were reported
	And I find the artifacts in the master release folder
	Given I save the master release folder file names and timestamps
    And I save the contents of the master release json dependencies file
    And I wait two seconds
    And I wait for a minute change on the clock
	When I run Shatilaya with target "IgnorePendingChangesAndDoNotPush"
	Then no Shatilaya errors were reported
    And the contents of the master release json dependencies file has not changed
	And the contents of the master release folder has not changed

Scenario: Release artifacts are copied to the master release candidate folder
	Given I clean up the master release candidate folder
	When I run Shatilaya with target "IgnorePendingChangesAndDoNotPush"
	Then no Shatilaya errors were reported
	And I find the artifacts in the master release candidate folder
	Given I save the master release candidate folder file names and timestamps
    And I save the contents of the master release candidate json dependencies file
    And I wait two seconds
    And I wait for a minute change on the clock
	When I run Shatilaya with target "IgnorePendingChangesAndDoNotPush"
	Then no Shatilaya errors were reported
	And the contents of the master release candidate json dependencies file has changed
	And the contents of the master release candidate folder has changed

Scenario: Release artifacts are not copied to the master release folder when build fails
	Given I clean up the master release folder
	And I change a source file so that it cannot be compiled
	When I run Shatilaya with target "IgnorePendingChangesAndDoNotPush"
	Then a compilation error was reported for the changed source file
	And I do not find any artifacts in the master release folder

Scenario: Release artifacts are not copied to the master release folder when a test case fails
	Given I clean up the master debug folder
	And I clean up the master release folder
	And I change a test case so that it will fail in release
	When I run Shatilaya with target "IgnorePendingChangesAndDoNotCreateOrPushPackage"
	Then a failed "Release" test case was reported
	And I find the artifacts in the master debug folder
	And I do not find any artifacts in the master release folder

Scenario: Nuget package is created in the master release folder
	Given I clean up the master release folder
	When I run Shatilaya with target "IgnorePendingChangesAndDoNotPush"
	Then no Shatilaya errors were reported
    And the number of "nupkg" files in the master "Release" folder is 1
	And the newest file in the master "Release" folder is of type "nupkg"
	And the newest nuget package in the master "Release" folder is tagged with the head tip id sha
	And the newest nuget package in the master "Release" folder does not contain a test assembly
	And I remember the last write time of the newest file in the master "Release" folder
	When I run Shatilaya with target "IgnorePendingChangesAndDoNotPush"
	Then no Shatilaya errors were reported
    And the number of "nupkg" files in the master "Release" folder is 1
	And the newest file in the master "Release" folder is of type "nupkg"
	And the last write time of the newest file in the master "Release" folder is as remembered
    And the number of "nupkg" files in the master "Debug" folder is 0

Scenario: Uncommitted changes break the build
	Given I change a source file so that it still can be compiled
	When I run Shatilaya with target "DoNotPush"
	Then an uncommitted change error was reported for the changed source file
	And I do not find any artifacts in the master debug folder

Scenario: Nuspec file is recreated
	Given I empty the nuspec file
	When I run Shatilaya with target "IgnorePendingChangesAndDoNotPush"
	Then a non-empty nuspec file is there again

Scenario: Uncommitted changes can be verified
	Given I change a source file so that it still can be compiled
	When I run Shatilaya with target "ValidatePackageUpdate"
	Then no Shatilaya errors were reported
    And 3 "Release" artifact/-s was/were produced

Scenario: Debug artifacts are copied to the master debug folder for a branch with packages
    Given I have cloned the "pkg-branch-test" branch of a green solution with unit tests to a temp folder
	And I clean up the master debug folder
	When I run Shatilaya with target "IgnorePendingChangesAndDoNotCreateOrPushPackage"
	Then no Shatilaya errors were reported
	And the branch is considered the master branch or a branch with packages
	And I find the artifacts in the master debug folder

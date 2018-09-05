@CakeBuild
Feature: CakeBuild
	In order to replace the little things
	I want to move the compile and test part to build.cake

Background: 
    Given I have a green solution with unit tests in a temp folder
	And Nuget packages are not restored yet
	And I copy the latest build.cake script from my Shatilaya solution and reference the local assemblies

Scenario: Latest build.cake is copied from GitHub Shatilaya master branch
	When I run the build.cake script
	Then the build.cake file is identical to the latest found on the GitHub Shatilaya master branch
	And I get an error message saying that I need to rerun my cake script

Scenario: Output folders are cleaned up
	When I run the build.cake script with target "CleanRestorePull"
	Then no cake errors were reported
	And build step "DebugBuild" was not a target
	And build step "UpdateBuildCake" was not a target
	And no artifact exists
	And no intermediate build output exists

Scenario: Nuget packages are restored and debug artifacts are built
	When I run the build.cake script with target "IgnoreOutdatedBuildCakePendingChangesAndDoNotPush"
	Then no cake errors were reported
	Then the Nuget packages are restored
    And 3 "Debug" artifact/-s was/were produced
	And 0 "Debug" nupkg file/-s was/were produced

Scenario: Debug build failure
	Given I change a source file so that it cannot be compiled
	When I run the build.cake script with target "IgnoreOutdatedBuildCakePendingChangesAndDoNotPush"
	Then a compilation error was reported for the changed source file

Scenario: Debug artifacts are copied to the master debug folder, but only if changed
	Given I clean up the master debug folder
	When I run the build.cake script with target "IgnoreOutdatedBuildCakePendingChangesAndDoCreateOrPushPackage"
	Then no cake errors were reported
	And I find the artifacts in the master debug folder
	Given I save the master debug folder file names and timestamps
    And I wait two seconds
	When I run the build.cake script with target "IgnoreOutdatedBuildCakePendingChangesAndDoCreateOrPushPackage"
	Then no cake errors were reported
	And the contents of the master debug folder has not changed

Scenario: Debug artifacts are not copied to the master debug folder when build fails
	Given I clean up the master debug folder
	And I change a source file so that it cannot be compiled
	When I run the build.cake script with target "IgnoreOutdatedBuildCakePendingChangesAndDoNotPush"
	Then a compilation error was reported for the changed source file
	And I do not find any artifacts in the master debug folder

Scenario: Debug artifacts are not copied to the master debug folder when a test case fails
	Given I clean up the master debug folder
	And I change a test case so that it will fail
	When I run the build.cake script with target "IgnoreOutdatedBuildCakePendingChangesAndDoNotPush"
	Then a failed test case was reported
	And I do not find any artifacts in the master debug folder

Scenario: Release artifacts are built
	When I run the build.cake script with target "IgnoreOutdatedBuildCakePendingChangesAndDoNotPush"
	Then no cake errors were reported
    And 3 "Release" artifact/-s was/were produced
	And 0 "Release" nupkg file/-s was/were produced

Scenario: Release artifacts are copied to the master release folder, but only if changed
	Given I clean up the master release folder
	When I run the build.cake script with target "IgnoreOutdatedBuildCakePendingChangesAndDoNotPush"
	Then no cake errors were reported
	And I find the artifacts in the master release folder
	Given I save the master release folder file names and timestamps
    And I wait two seconds
	When I run the build.cake script with target "IgnoreOutdatedBuildCakePendingChangesAndDoNotPush"
	Then no cake errors were reported
	And the contents of the master release folder has not changed

Scenario: Release artifacts are not copied to the master release folder when build fails
	Given I clean up the master release folder
	And I change a source file so that it cannot be compiled
	When I run the build.cake script with target "IgnoreOutdatedBuildCakePendingChangesAndDoNotPush"
	Then a compilation error was reported for the changed source file
	And I do not find any artifacts in the master release folder

Scenario: Release artifacts are not copied to the master release folder when a test case fails
	Given I clean up the master debug folder
	And I clean up the master release folder
	And I change a test case so that it will fail in release
	When I run the build.cake script with target "IgnoreOutdatedBuildCakePendingChangesAndDoCreateOrPushPackage"
	Then a failed test case was reported
	And I find the artifacts in the master debug folder
	And I do not find any artifacts in the master release folder

Scenario: Nuget package is created in the master release folder
	Given I clean up the master release folder
	When I run the build.cake script with target "IgnoreOutdatedBuildCakePendingChangesAndDoNotPush"
	Then no cake errors were reported
    And the number of "nupkg" files in the master "Release" folder is 1
	And the newest file in the master "Release" folder is of type "nupkg"
	And the newest nuget package in the master "Release" folder is tagged with the head tip id sha
	And the newest nuget package in the master "Release" folder does not contain a test assembly
	And I remember the last write time of the newest file in the master "Release" folder
	When I run the build.cake script with target "IgnoreOutdatedBuildCakePendingChangesAndDoNotPush"
	Then no cake errors were reported
    And the number of "nupkg" files in the master "Release" folder is 1
	And the newest file in the master "Release" folder is of type "nupkg"
	And the last write time of the newest file in the master "Release" folder is as remembered
    And the number of "nupkg" files in the master "Debug" folder is 0

Scenario: Uncommitted changes break the build
	Given I change a source file so that it still can be compiled
	When I run the build.cake script with target "IgnoreOutdatedBuildCakeAndDoNotPush"
	Then an uncommitted change error was reported for the changed source file
	And I do not find any artifacts in the master debug folder

Scenario: Nuspec file is recreated
	Given I empty the nuspec file
	When I run the build.cake script with target "IgnoreOutdatedBuildCakePendingChangesAndDoNotPush"
	Then a non-empty nuspec file is there again

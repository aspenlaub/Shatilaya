@CakeBuild
Feature: CakeBuild
	In order to replace the little things
	I want to move the compile and test part to build.cake

Background: 
    Given I have a green solution with unit tests in a temp folder
	And Nuget packages are not restored yet

Scenario: Latest build.cake is copied from GitHub Shatilaya master branch
	When I run the build.cake script
	Then the build.cake file is identical to the latest found on the GitHub Shatilaya master branch
	And I get an error message saying that I need to rerun my cake script

Scenario: Output folders are cleaned up
	Given I copy the latest build.cake script from my Shatilaya solution
	And I change the cake script so that debug build is suppressed
	When I run the build.cake script
	Then no cake errors were reported
	And build step "DebugBuild" was skipped
	And build step "UpdateBuildCake" was skipped
	And no artifact exists
	And no intermediate build output exists

Scenario: Nuget packages are restored
	Given I copy the latest build.cake script from my Shatilaya solution
	When I run the build.cake script
	Then no cake errors were reported
	Then the Nuget packages are restored

Scenario: Debug artifacts are built
	Given I copy the latest build.cake script from my Shatilaya solution
	When I run the build.cake script
	Then no cake errors were reported
    And Debug artifacts were produced
	And no nupkg files were produced

Scenario: Debug build failure
	Given I copy the latest build.cake script from my Shatilaya solution
	And I change a source file so that it cannot be compiled
	When I run the build.cake script
	Then a compilation error was reported for the changed source file

Scenario: Debug artifacts are copied to the master debug folder
	Given I copy the latest build.cake script from my Shatilaya solution
	And I clean up the master debug folder
	When I run the build.cake script
	Then no cake errors were reported
	And I find the artifacts in the master debug folder

Scenario: Only changed artifacts are copied to the master debug folder
	Given I copy the latest build.cake script from my Shatilaya solution
	And I clean up the master debug folder
	And I run the build.cake script
	And no cake errors were reported
	And I save the master debug folder file names and timestamps
    And I wait two seconds
	When I run the build.cake script
	Then no cake errors were reported
	And the contents of the master debug folder has not changed

Scenario: Debug artifacts are not copied to the master debug folder when build fails
	Given I copy the latest build.cake script from my Shatilaya solution
	And I clean up the master debug folder
	And I change a source file so that it cannot be compiled
	When I run the build.cake script
	Then a compilation error was reported for the changed source file
	And I do not find any artifacts in the master debug folder


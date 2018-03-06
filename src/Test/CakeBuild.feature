@CakeBuild
Feature: CakeBuild
	In order to replace the little things
	I want to move the compile and test part to build.cake

Background: 
    Given I have a green solution with unit tests in a temp folder
	And Nuget packages are not restored yet

Scenario: Latest build.cake is copied from GitHub Shatilaya master branch
	When I run the build.cake script
	Then no cake errors were reported
	And the build.cake file is identical to the latest found on the GitHub Shatilaya master branch

Scenario: Output folders are cleaned up
	Given I copy the latest build.cake script from my Shatilaya solution
	When I run the build.cake script
	Then no cake errors were reported
	And no artifact exists
	And no intermediate build output exists

Scenario: Nuget packages are restored
	Given I copy the latest build.cake script from my Shatilaya solution
	When I run the build.cake script
	Then no cake errors were reported
	Then the Nuget packages are restored

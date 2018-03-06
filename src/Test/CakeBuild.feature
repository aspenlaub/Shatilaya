@CakeBuild
Feature: CakeBuild
	In order to replace the little things
	I want to move the compile and test part to build.cake

Background: 
    Given I have a green solution with unit tests in a temp folder
	And I have the latest build.cake script
	And Nuget packages are not restored yet

Scenario: Output folders are cleaned up
	When I run the build.cake script
	Then no cake errors were reported
	And no artifact exists
	And no intermediate build output exists

Scenario: Nuget packages are restored
	When I run the build.cake script
	Then no cake errors were reported
	Then the Nuget packages are restored

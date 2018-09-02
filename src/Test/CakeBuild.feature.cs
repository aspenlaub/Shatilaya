﻿// ------------------------------------------------------------------------------
//  <auto-generated>
//      This code was generated by SpecFlow (http://www.specflow.org/).
//      SpecFlow Version:2.4.0.0
//      SpecFlow Generator Version:2.4.0.0
// 
//      Changes to this file may cause incorrect behavior and will be lost if
//      the code is regenerated.
//  </auto-generated>
// ------------------------------------------------------------------------------
#region Designer generated code
#pragma warning disable
namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Test
{
    using TechTalk.SpecFlow;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "2.4.0.0")]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [Microsoft.VisualStudio.TestTools.UnitTesting.TestClassAttribute()]
    public partial class CakeBuildFeature
    {
        
        private static TechTalk.SpecFlow.ITestRunner testRunner;
        
        private Microsoft.VisualStudio.TestTools.UnitTesting.TestContext _testContext;
        
#line 1 "CakeBuild.feature"
#line hidden
        
        public virtual Microsoft.VisualStudio.TestTools.UnitTesting.TestContext TestContext
        {
            get
            {
                return this._testContext;
            }
            set
            {
                this._testContext = value;
            }
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.ClassInitializeAttribute()]
        public static void FeatureSetup(Microsoft.VisualStudio.TestTools.UnitTesting.TestContext testContext)
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner(null, 0);
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "CakeBuild", "\tIn order to replace the little things\r\n\tI want to move the compile and test part" +
                    " to build.cake", ProgrammingLanguage.CSharp, new string[] {
                        "CakeBuild"});
            testRunner.OnFeatureStart(featureInfo);
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.ClassCleanupAttribute()]
        public static void FeatureTearDown()
        {
            testRunner.OnFeatureEnd();
            testRunner = null;
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestInitializeAttribute()]
        public virtual void TestInitialize()
        {
            if (((testRunner.FeatureContext != null) 
                        && (testRunner.FeatureContext.FeatureInfo.Title != "CakeBuild")))
            {
                global::Aspenlaub.Net.GitHub.CSharp.Shatilaya.Test.CakeBuildFeature.FeatureSetup(null);
            }
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestCleanupAttribute()]
        public virtual void ScenarioTearDown()
        {
            testRunner.OnScenarioEnd();
        }
        
        public virtual void ScenarioInitialize(TechTalk.SpecFlow.ScenarioInfo scenarioInfo)
        {
            testRunner.OnScenarioInitialize(scenarioInfo);
            testRunner.ScenarioContext.ScenarioContainer.RegisterInstanceAs<Microsoft.VisualStudio.TestTools.UnitTesting.TestContext>(_testContext);
        }
        
        public virtual void ScenarioStart()
        {
            testRunner.OnScenarioStart();
        }
        
        public virtual void ScenarioCleanup()
        {
            testRunner.CollectScenarioErrors();
        }
        
        public virtual void FeatureBackground()
        {
#line 6
#line 7
    testRunner.Given("I have a green solution with unit tests in a temp folder", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 8
 testRunner.And("Nuget packages are not restored yet", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 9
 testRunner.And("I copy the latest build.cake script from my Shatilaya solution and reference the " +
                    "local assemblies", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("Latest build.cake is copied from GitHub Shatilaya master branch")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "CakeBuild")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestCategoryAttribute("CakeBuild")]
        public virtual void LatestBuild_CakeIsCopiedFromGitHubShatilayaMasterBranch()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Latest build.cake is copied from GitHub Shatilaya master branch", null, ((string[])(null)));
#line 11
this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 6
this.FeatureBackground();
#line 12
 testRunner.When("I run the build.cake script", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 13
 testRunner.Then("the build.cake file is identical to the latest found on the GitHub Shatilaya mast" +
                    "er branch", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 14
 testRunner.And("I get an error message saying that I need to rerun my cake script", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("Output folders are cleaned up")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "CakeBuild")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestCategoryAttribute("CakeBuild")]
        public virtual void OutputFoldersAreCleanedUp()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Output folders are cleaned up", null, ((string[])(null)));
#line 16
this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 6
this.FeatureBackground();
#line 17
 testRunner.When("I run the build.cake script with target \"CleanRestorePull\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 18
 testRunner.Then("no cake errors were reported", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 19
 testRunner.And("build step \"DebugBuild\" was not a target", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 20
 testRunner.And("build step \"UpdateBuildCake\" was not a target", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 21
 testRunner.And("no artifact exists", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 22
 testRunner.And("no intermediate build output exists", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("Nuget packages are restored and debug artifacts are built")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "CakeBuild")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestCategoryAttribute("CakeBuild")]
        public virtual void NugetPackagesAreRestoredAndDebugArtifactsAreBuilt()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Nuget packages are restored and debug artifacts are built", null, ((string[])(null)));
#line 24
this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 6
this.FeatureBackground();
#line 25
 testRunner.When("I run the build.cake script with target \"IgnoreOutdatedBuildCakePendingChangesAnd" +
                    "DoNotPush\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 26
 testRunner.Then("no cake errors were reported", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 27
 testRunner.Then("the Nuget packages are restored", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 28
    testRunner.And("3 \"Debug\" artifact/-s was/were produced", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 29
 testRunner.And("0 \"Debug\" nupkg file/-s was/were produced", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("Debug build failure")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "CakeBuild")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestCategoryAttribute("CakeBuild")]
        public virtual void DebugBuildFailure()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Debug build failure", null, ((string[])(null)));
#line 31
this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 6
this.FeatureBackground();
#line 32
 testRunner.Given("I change a source file so that it cannot be compiled", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 33
 testRunner.When("I run the build.cake script with target \"IgnoreOutdatedBuildCakePendingChangesAnd" +
                    "DoNotPush\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 34
 testRunner.Then("a compilation error was reported for the changed source file", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("Debug artifacts are copied to the master debug folder, but only if changed")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "CakeBuild")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestCategoryAttribute("CakeBuild")]
        public virtual void DebugArtifactsAreCopiedToTheMasterDebugFolderButOnlyIfChanged()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Debug artifacts are copied to the master debug folder, but only if changed", null, ((string[])(null)));
#line 36
this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 6
this.FeatureBackground();
#line 37
 testRunner.Given("I clean up the master debug folder", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 38
 testRunner.When("I run the build.cake script with target \"IgnoreOutdatedBuildCakePendingChangesAnd" +
                    "DoCreateOrPushPackage\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 39
 testRunner.Then("no cake errors were reported", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 40
 testRunner.And("I find the artifacts in the master debug folder", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 41
 testRunner.And("there is no obj folder in the src folder", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 42
 testRunner.Given("I save the master debug folder file names and timestamps", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 43
    testRunner.And("I wait two seconds", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 44
 testRunner.When("I run the build.cake script with target \"IgnoreOutdatedBuildCakePendingChangesAnd" +
                    "DoCreateOrPushPackage\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 45
 testRunner.Then("no cake errors were reported", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 46
 testRunner.And("the contents of the master debug folder has not changed", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("Debug artifacts are not copied to the master debug folder when build fails")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "CakeBuild")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestCategoryAttribute("CakeBuild")]
        public virtual void DebugArtifactsAreNotCopiedToTheMasterDebugFolderWhenBuildFails()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Debug artifacts are not copied to the master debug folder when build fails", null, ((string[])(null)));
#line 48
this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 6
this.FeatureBackground();
#line 49
 testRunner.Given("I clean up the master debug folder", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 50
 testRunner.And("I change a source file so that it cannot be compiled", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 51
 testRunner.When("I run the build.cake script with target \"IgnoreOutdatedBuildCakePendingChangesAnd" +
                    "DoNotPush\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 52
 testRunner.Then("a compilation error was reported for the changed source file", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 53
 testRunner.And("I do not find any artifacts in the master debug folder", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("Debug artifacts are not copied to the master debug folder when a test case fails")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "CakeBuild")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestCategoryAttribute("CakeBuild")]
        public virtual void DebugArtifactsAreNotCopiedToTheMasterDebugFolderWhenATestCaseFails()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Debug artifacts are not copied to the master debug folder when a test case fails", null, ((string[])(null)));
#line 55
this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 6
this.FeatureBackground();
#line 56
 testRunner.Given("I clean up the master debug folder", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 57
 testRunner.And("I change a test case so that it will fail", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 58
 testRunner.When("I run the build.cake script with target \"IgnoreOutdatedBuildCakePendingChangesAnd" +
                    "DoNotPush\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 59
 testRunner.Then("a failed test case was reported", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 60
 testRunner.And("I do not find any artifacts in the master debug folder", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("Release artifacts are built")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "CakeBuild")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestCategoryAttribute("CakeBuild")]
        public virtual void ReleaseArtifactsAreBuilt()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Release artifacts are built", null, ((string[])(null)));
#line 62
this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 6
this.FeatureBackground();
#line 63
 testRunner.When("I run the build.cake script with target \"IgnoreOutdatedBuildCakePendingChangesAnd" +
                    "DoNotPush\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 64
 testRunner.Then("no cake errors were reported", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 65
    testRunner.And("3 \"Release\" artifact/-s was/were produced", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 66
 testRunner.And("0 \"Release\" nupkg file/-s was/were produced", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("Release artifacts are copied to the master release folder, but only if changed")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "CakeBuild")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestCategoryAttribute("CakeBuild")]
        public virtual void ReleaseArtifactsAreCopiedToTheMasterReleaseFolderButOnlyIfChanged()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Release artifacts are copied to the master release folder, but only if changed", null, ((string[])(null)));
#line 68
this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 6
this.FeatureBackground();
#line 69
 testRunner.Given("I clean up the master release folder", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 70
 testRunner.When("I run the build.cake script with target \"IgnoreOutdatedBuildCakePendingChangesAnd" +
                    "DoNotPush\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 71
 testRunner.Then("no cake errors were reported", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 72
 testRunner.And("I find the artifacts in the master release folder", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 73
 testRunner.Given("I save the master release folder file names and timestamps", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 74
    testRunner.And("I wait two seconds", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 75
 testRunner.When("I run the build.cake script with target \"IgnoreOutdatedBuildCakePendingChangesAnd" +
                    "DoNotPush\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 76
 testRunner.Then("no cake errors were reported", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 77
 testRunner.And("the contents of the master release folder has not changed", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("Release artifacts are not copied to the master release folder when build fails")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "CakeBuild")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestCategoryAttribute("CakeBuild")]
        public virtual void ReleaseArtifactsAreNotCopiedToTheMasterReleaseFolderWhenBuildFails()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Release artifacts are not copied to the master release folder when build fails", null, ((string[])(null)));
#line 79
this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 6
this.FeatureBackground();
#line 80
 testRunner.Given("I clean up the master release folder", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 81
 testRunner.And("I change a source file so that it cannot be compiled", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 82
 testRunner.When("I run the build.cake script with target \"IgnoreOutdatedBuildCakePendingChangesAnd" +
                    "DoNotPush\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 83
 testRunner.Then("a compilation error was reported for the changed source file", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 84
 testRunner.And("I do not find any artifacts in the master release folder", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("Release artifacts are not copied to the master release folder when a test case fa" +
            "ils")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "CakeBuild")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestCategoryAttribute("CakeBuild")]
        public virtual void ReleaseArtifactsAreNotCopiedToTheMasterReleaseFolderWhenATestCaseFails()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Release artifacts are not copied to the master release folder when a test case fa" +
                    "ils", null, ((string[])(null)));
#line 86
this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 6
this.FeatureBackground();
#line 87
 testRunner.Given("I clean up the master debug folder", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 88
 testRunner.And("I clean up the master release folder", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 89
 testRunner.And("I change a test case so that it will fail in release", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 90
 testRunner.When("I run the build.cake script with target \"IgnoreOutdatedBuildCakePendingChangesAnd" +
                    "DoCreateOrPushPackage\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 91
 testRunner.Then("a failed test case was reported", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 92
 testRunner.And("I find the artifacts in the master debug folder", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 93
 testRunner.And("I do not find any artifacts in the master release folder", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("Nuget package is created in the master release folder")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "CakeBuild")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestCategoryAttribute("CakeBuild")]
        public virtual void NugetPackageIsCreatedInTheMasterReleaseFolder()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Nuget package is created in the master release folder", null, ((string[])(null)));
#line 95
this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 6
this.FeatureBackground();
#line 96
 testRunner.Given("I clean up the master release folder", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 97
 testRunner.When("I run the build.cake script with target \"IgnoreOutdatedBuildCakePendingChangesAnd" +
                    "DoNotPush\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 98
 testRunner.Then("no cake errors were reported", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 99
    testRunner.And("the number of \"nupkg\" files in the master \"Release\" folder is 1", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 100
 testRunner.And("the newest file in the master \"Release\" folder is of type \"nupkg\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 101
 testRunner.And("the newest nuget package in the master \"Release\" folder is tagged with the head t" +
                    "ip id sha", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 102
 testRunner.And("the newest nuget package in the master \"Release\" folder does not contain a test a" +
                    "ssembly", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 103
 testRunner.And("I remember the last write time of the newest file in the master \"Release\" folder", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 104
 testRunner.When("I run the build.cake script with target \"IgnoreOutdatedBuildCakePendingChangesAnd" +
                    "DoNotPush\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 105
 testRunner.Then("no cake errors were reported", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 106
    testRunner.And("the number of \"nupkg\" files in the master \"Release\" folder is 1", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 107
 testRunner.And("the newest file in the master \"Release\" folder is of type \"nupkg\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 108
 testRunner.And("the last write time of the newest file in the master \"Release\" folder is as remem" +
                    "bered", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 109
    testRunner.And("the number of \"nupkg\" files in the master \"Debug\" folder is 0", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("Uncommitted changes break the build")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "CakeBuild")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestCategoryAttribute("CakeBuild")]
        public virtual void UncommittedChangesBreakTheBuild()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Uncommitted changes break the build", null, ((string[])(null)));
#line 111
this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 6
this.FeatureBackground();
#line 112
 testRunner.Given("I change a source file so that it still can be compiled", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 113
 testRunner.When("I run the build.cake script with target \"IgnoreOutdatedBuildCakeAndDoNotPush\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 114
 testRunner.Then("an uncommitted change error was reported for the changed source file", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 115
 testRunner.And("I do not find any artifacts in the master debug folder", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("Nuspec file is recreated")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "CakeBuild")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestCategoryAttribute("CakeBuild")]
        public virtual void NuspecFileIsRecreated()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Nuspec file is recreated", null, ((string[])(null)));
#line 117
this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 6
this.FeatureBackground();
#line 118
 testRunner.Given("I empty the nuspec file", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 119
 testRunner.When("I run the build.cake script with target \"IgnoreOutdatedBuildCakePendingChangesAnd" +
                    "DoNotPush\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 120
 testRunner.Then("a non-empty nuspec file is there again", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
    }
}
#pragma warning restore
#endregion

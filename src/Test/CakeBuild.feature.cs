﻿// ------------------------------------------------------------------------------
//  <auto-generated>
//      This code was generated by SpecFlow (http://www.specflow.org/).
//      SpecFlow Version:2.3.0.0
//      SpecFlow Generator Version:2.3.0.0
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
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "2.3.0.0")]
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
        
        public virtual void ScenarioSetup(TechTalk.SpecFlow.ScenarioInfo scenarioInfo)
        {
            testRunner.OnScenarioStart(scenarioInfo);
            testRunner.ScenarioContext.ScenarioContainer.RegisterInstanceAs<Microsoft.VisualStudio.TestTools.UnitTesting.TestContext>(TestContext);
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
#line hidden
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("Latest build.cake is copied from GitHub Shatilaya master branch")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "CakeBuild")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestCategoryAttribute("CakeBuild")]
        public virtual void LatestBuild_CakeIsCopiedFromGitHubShatilayaMasterBranch()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Latest build.cake is copied from GitHub Shatilaya master branch", ((string[])(null)));
#line 10
this.ScenarioSetup(scenarioInfo);
#line 6
this.FeatureBackground();
#line 11
    testRunner.Given("I modify the build.cake script", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
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
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Output folders are cleaned up", ((string[])(null)));
#line 16
this.ScenarioSetup(scenarioInfo);
#line 6
this.FeatureBackground();
#line 17
 testRunner.Given("I copy the latest build.cake script from my Shatilaya solution and reference the " +
                    "local assemblies", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 18
 testRunner.When("I run the build.cake script with target \"CleanRestoreUpdateNuspec\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 19
 testRunner.Then("no cake errors were reported", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 20
 testRunner.And("build step \"DebugBuild\" was not a target", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 21
 testRunner.And("build step \"UpdateBuildCake\" was not a target", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 22
 testRunner.And("no artifact exists", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 23
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
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Nuget packages are restored and debug artifacts are built", ((string[])(null)));
#line 25
this.ScenarioSetup(scenarioInfo);
#line 6
this.FeatureBackground();
#line 26
 testRunner.Given("I copy the latest build.cake script from my Shatilaya solution and reference the " +
                    "local assemblies", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 27
 testRunner.When("I run the build.cake script with target \"IgnoreOutdatedBuildCakePendingChangesAnd" +
                    "DoNotPush\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 28
 testRunner.Then("no cake errors were reported", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 29
 testRunner.Then("the Nuget packages are restored", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 30
    testRunner.And("2 \"Debug\" artifact/-s was/were produced", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 31
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
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Debug build failure", ((string[])(null)));
#line 33
this.ScenarioSetup(scenarioInfo);
#line 6
this.FeatureBackground();
#line 34
 testRunner.Given("I copy the latest build.cake script from my Shatilaya solution and reference the " +
                    "local assemblies", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 35
 testRunner.And("I change a source file so that it cannot be compiled", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 36
 testRunner.When("I run the build.cake script with target \"IgnoreOutdatedBuildCakePendingChangesAnd" +
                    "DoNotPush\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 37
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
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Debug artifacts are copied to the master debug folder, but only if changed", ((string[])(null)));
#line 39
this.ScenarioSetup(scenarioInfo);
#line 6
this.FeatureBackground();
#line 40
 testRunner.Given("I copy the latest build.cake script from my Shatilaya solution and reference the " +
                    "local assemblies", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 41
 testRunner.And("I clean up the master debug folder", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 42
 testRunner.When("I run the build.cake script with target \"IgnoreOutdatedBuildCakePendingChangesAnd" +
                    "DoNotPush\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 43
 testRunner.Then("no cake errors were reported", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 44
 testRunner.And("I find the artifacts in the master debug folder", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 45
 testRunner.Given("I save the master debug folder file names and timestamps", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 46
    testRunner.And("I wait two seconds", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 47
 testRunner.When("I run the build.cake script with target \"IgnoreOutdatedBuildCakePendingChangesAnd" +
                    "DoNotPush\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 48
 testRunner.Then("no cake errors were reported", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 49
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
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Debug artifacts are not copied to the master debug folder when build fails", ((string[])(null)));
#line 51
this.ScenarioSetup(scenarioInfo);
#line 6
this.FeatureBackground();
#line 52
 testRunner.Given("I copy the latest build.cake script from my Shatilaya solution and reference the " +
                    "local assemblies", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 53
 testRunner.And("I clean up the master debug folder", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 54
 testRunner.And("I change a source file so that it cannot be compiled", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 55
 testRunner.When("I run the build.cake script with target \"IgnoreOutdatedBuildCakePendingChangesAnd" +
                    "DoNotPush\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 56
 testRunner.Then("a compilation error was reported for the changed source file", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 57
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
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Debug artifacts are not copied to the master debug folder when a test case fails", ((string[])(null)));
#line 59
this.ScenarioSetup(scenarioInfo);
#line 6
this.FeatureBackground();
#line 60
 testRunner.Given("I copy the latest build.cake script from my Shatilaya solution and reference the " +
                    "local assemblies", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 61
 testRunner.And("I clean up the master debug folder", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 62
 testRunner.And("I change a test case so that it will fail", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 63
 testRunner.When("I run the build.cake script with target \"IgnoreOutdatedBuildCakePendingChangesAnd" +
                    "DoNotPush\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 64
 testRunner.Then("a failed test case was reported", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 65
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
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Release artifacts are built", ((string[])(null)));
#line 67
this.ScenarioSetup(scenarioInfo);
#line 6
this.FeatureBackground();
#line 68
 testRunner.Given("I copy the latest build.cake script from my Shatilaya solution and reference the " +
                    "local assemblies", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 69
 testRunner.When("I run the build.cake script with target \"IgnoreOutdatedBuildCakePendingChangesAnd" +
                    "DoNotPush\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 70
 testRunner.Then("no cake errors were reported", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 71
    testRunner.And("2 \"Release\" artifact/-s was/were produced", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 72
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
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Release artifacts are copied to the master release folder, but only if changed", ((string[])(null)));
#line 74
this.ScenarioSetup(scenarioInfo);
#line 6
this.FeatureBackground();
#line 75
 testRunner.Given("I copy the latest build.cake script from my Shatilaya solution and reference the " +
                    "local assemblies", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 76
 testRunner.And("I clean up the master release folder", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 77
 testRunner.When("I run the build.cake script with target \"IgnoreOutdatedBuildCakePendingChangesAnd" +
                    "DoNotPush\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 78
 testRunner.Then("no cake errors were reported", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 79
 testRunner.And("I find the artifacts in the master release folder", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 80
 testRunner.Given("I save the master release folder file names and timestamps", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 81
    testRunner.And("I wait two seconds", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 82
 testRunner.When("I run the build.cake script with target \"IgnoreOutdatedBuildCakePendingChangesAnd" +
                    "DoNotPush\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 83
 testRunner.Then("no cake errors were reported", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 84
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
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Release artifacts are not copied to the master release folder when build fails", ((string[])(null)));
#line 86
this.ScenarioSetup(scenarioInfo);
#line 6
this.FeatureBackground();
#line 87
 testRunner.Given("I copy the latest build.cake script from my Shatilaya solution and reference the " +
                    "local assemblies", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 88
 testRunner.And("I clean up the master release folder", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 89
 testRunner.And("I change a source file so that it cannot be compiled", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 90
 testRunner.When("I run the build.cake script with target \"IgnoreOutdatedBuildCakePendingChangesAnd" +
                    "DoNotPush\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 91
 testRunner.Then("a compilation error was reported for the changed source file", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 92
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
                    "ils", ((string[])(null)));
#line 94
this.ScenarioSetup(scenarioInfo);
#line 6
this.FeatureBackground();
#line 95
 testRunner.Given("I copy the latest build.cake script from my Shatilaya solution and reference the " +
                    "local assemblies", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 96
    testRunner.And("I clean up the master debug folder", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 97
 testRunner.And("I clean up the master release folder", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 98
 testRunner.And("I change a test case so that it will fail in release", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 99
 testRunner.When("I run the build.cake script with target \"IgnoreOutdatedBuildCakePendingChangesAnd" +
                    "DoNotPush\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 100
 testRunner.Then("a failed test case was reported", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 101
 testRunner.And("I find the artifacts in the master debug folder", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 102
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
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Nuget package is created in the master release folder", ((string[])(null)));
#line 104
this.ScenarioSetup(scenarioInfo);
#line 6
this.FeatureBackground();
#line 105
 testRunner.Given("I copy the latest build.cake script from my Shatilaya solution and reference the " +
                    "local assemblies", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 106
 testRunner.And("I clean up the master release folder", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 107
 testRunner.When("I run the build.cake script with target \"IgnoreOutdatedBuildCakePendingChangesAnd" +
                    "DoNotPush\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 108
 testRunner.Then("no cake errors were reported", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 109
    testRunner.And("the number of \"nupkg\" files in the master \"Release\" folder is 1", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 110
 testRunner.And("the newest file in the master \"Release\" folder is of type \"nupkg\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 111
 testRunner.And("the newest nuget package in the master \"Release\" folder is tagged with the head t" +
                    "ip id sha", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 112
 testRunner.And("the newest nuget package in the master \"Release\" folder does not contain a test a" +
                    "ssembly", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 113
 testRunner.And("I remember the last write time of the newest file in the master \"Release\" folder", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 114
 testRunner.When("I run the build.cake script with target \"IgnoreOutdatedBuildCakePendingChangesAnd" +
                    "DoNotPush\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 115
 testRunner.Then("no cake errors were reported", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 116
    testRunner.And("the number of \"nupkg\" files in the master \"Release\" folder is 1", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 117
 testRunner.And("the newest file in the master \"Release\" folder is of type \"nupkg\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 118
 testRunner.And("the last write time of the newest file in the master \"Release\" folder is as remem" +
                    "bered", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 119
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
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Uncommitted changes break the build", ((string[])(null)));
#line 121
this.ScenarioSetup(scenarioInfo);
#line 6
this.FeatureBackground();
#line 122
 testRunner.Given("I copy the latest build.cake script from my Shatilaya solution and reference the " +
                    "local assemblies", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 123
 testRunner.And("I change a source file so that it still can be compiled", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 124
 testRunner.When("I run the build.cake script with target \"IgnoreOutdatedBuildCakeAndDoNotPush\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 125
 testRunner.Then("an uncommitted change error was reported for the changed source file", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 126
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
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Nuspec file is recreated", ((string[])(null)));
#line 128
this.ScenarioSetup(scenarioInfo);
#line 6
this.FeatureBackground();
#line 129
 testRunner.Given("I copy the latest build.cake script from my Shatilaya solution and reference the " +
                    "local assemblies", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 130
 testRunner.And("I empty the nuspec file", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 131
 testRunner.When("I run the build.cake script with target \"IgnoreOutdatedBuildCakePendingChangesAnd" +
                    "DoNotPush\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 132
 testRunner.Then("a non-empty nuspec file is there again", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
    }
}
#pragma warning restore
#endregion

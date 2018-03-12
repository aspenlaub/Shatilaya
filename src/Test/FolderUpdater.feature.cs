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
    public partial class FolderUpdaterFeature
    {
        
        private static TechTalk.SpecFlow.ITestRunner testRunner;
        
        private Microsoft.VisualStudio.TestTools.UnitTesting.TestContext _testContext;
        
#line 1 "FolderUpdater.feature"
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
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "FolderUpdater", "\tIn order to update binary compiler output only if necessary\r\n\tI want to use a de" +
                    "dicated folder updater component", ProgrammingLanguage.CSharp, new string[] {
                        "FolderUpdater"});
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
                        && (testRunner.FeatureContext.FeatureInfo.Title != "FolderUpdater")))
            {
                global::Aspenlaub.Net.GitHub.CSharp.Shatilaya.Test.FolderUpdaterFeature.FeatureSetup(null);
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
 testRunner.Given("I have an empty source and an empty destination folder beneath the user\'s temp fo" +
                    "lder", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("Destination folder is created when missing")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "FolderUpdater")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestCategoryAttribute("FolderUpdater")]
        public virtual void DestinationFolderIsCreatedWhenMissing()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Destination folder is created when missing", ((string[])(null)));
#line 9
this.ScenarioSetup(scenarioInfo);
#line 6
this.FeatureBackground();
#line 10
 testRunner.Given("The destination folder does not exist", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 11
 testRunner.When("I update the destination folder", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 12
 testRunner.Then("the destination folder exists", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("Empty file is updated once")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "FolderUpdater")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestCategoryAttribute("FolderUpdater")]
        public virtual void EmptyFileIsUpdatedOnce()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Empty file is updated once", ((string[])(null)));
#line 14
this.ScenarioSetup(scenarioInfo);
#line 6
this.FeatureBackground();
#line 15
 testRunner.Given("I place an empty file into the source folder", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 16
 testRunner.When("I update the destination folder", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 17
 testRunner.Then("the empty file is present in the destination folder", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 18
 testRunner.And("the last write times of the empty source and the empty destination files are iden" +
                    "tical", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 19
    testRunner.When("I wait two seconds", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 20
 testRunner.And("I overwrite the empty file in the source folder", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 21
 testRunner.And("I update the destination folder", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 22
 testRunner.Then("the last write time of the empty destination file is unchanged", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("4 kilobyte file is updated")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "FolderUpdater")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestCategoryAttribute("FolderUpdater")]
        public virtual void _4KilobyteFileIsUpdated()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("4 kilobyte file is updated", ((string[])(null)));
#line 24
this.ScenarioSetup(scenarioInfo);
#line 6
this.FeatureBackground();
#line 25
 testRunner.Given("I place a 4 kilobyte file into the source folder", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 26
    testRunner.And("I wait two seconds", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 27
 testRunner.And("I place a 4 kilobyte file with 50 differences into the destintion folder", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 28
 testRunner.When("I update the destination folder", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 29
 testRunner.Then("the 4 kilobyte destination file is identical to the 4 kilobyte source file", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 30
 testRunner.And("the last write times of the 4 kilobyte source and the 4 kilobyte destination file" +
                    "s are identical", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("4 kilobyte file is not updated")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "FolderUpdater")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestCategoryAttribute("FolderUpdater")]
        public virtual void _4KilobyteFileIsNotUpdated()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("4 kilobyte file is not updated", ((string[])(null)));
#line 32
this.ScenarioSetup(scenarioInfo);
#line 6
this.FeatureBackground();
#line 33
 testRunner.Given("I place a 4 kilobyte file into the source folder", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 34
    testRunner.And("I wait two seconds", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 35
 testRunner.And("I place a 4 kilobyte file with 20 differences into the destintion folder", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 36
 testRunner.When("I update the destination folder", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 37
 testRunner.Then("the last write time of the 4 kilobyte destination file is unchanged", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("3 kilobyte file is updated")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "FolderUpdater")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestCategoryAttribute("FolderUpdater")]
        public virtual void _3KilobyteFileIsUpdated()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("3 kilobyte file is updated", ((string[])(null)));
#line 39
this.ScenarioSetup(scenarioInfo);
#line 6
this.FeatureBackground();
#line 40
 testRunner.Given("I place a 3 kilobyte file into the source folder", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 41
    testRunner.And("I wait two seconds", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 42
 testRunner.And("I place a 3 kilobyte file with 20 differences into the destintion folder", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 43
 testRunner.When("I update the destination folder", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 44
 testRunner.Then("the 3 kilobyte destination file is identical to the 3 kilobyte source file", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 45
 testRunner.And("the last write times of the 3 kilobyte source and the 3 kilobyte destination file" +
                    "s are identical", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            this.ScenarioCleanup();
        }
    }
}
#pragma warning restore
#endregion

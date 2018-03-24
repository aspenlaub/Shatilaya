using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces;
using LibGit2Sharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TechTalk.SpecFlow;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Test {
    [Binding]
    public class CakeBuildSteps {
        protected ErrorsAndInfos CakeErrorsAndInfos = new ErrorsAndInfos();
        protected IDictionary<string, DateTime> MasterDebugBinFolderSnapshot, MasterReleaseBinFolderSnapshot;
        protected static TestTargetFolder ChabTarget = new TestTargetFolder(nameof(CakeBuildSteps), "Chab");
        protected IComponentProvider ComponentProvider;

        public CakeBuildSteps() {
            var componentProviderMock = new Mock<IComponentProvider>();
            componentProviderMock.SetupGet(c => c.ProcessRunner).Returns(new ProcessRunner());
            componentProviderMock.SetupGet(c => c.CakeRunner).Returns(new CakeRunner(componentProviderMock.Object));
            ComponentProvider = componentProviderMock.Object;
        }

        [BeforeFeature("CakeBuild")]
        public static void RecreateCakeFolder() {
            ChabTarget.DeleteCakeFolder();
            ChabTarget.CreateCakeFolder();
        }

        [AfterFeature("CakeBuild")]
        public static void DeleteCakeFolder() {
            ChabTarget.DeleteCakeFolder();
        }

        [AfterScenario("CakeBuild")]
        public void CleanUpScenario() {
            ChabTarget.Delete();
        }

        #region Given
        [Given(@"I have a green solution with unit tests in a temp folder")]
        public void GivenIHaveAGreenSolutionWithUnitTestsInATempFolder() {
            if (ChabTarget.Exists()) {
                CleanUpScenario();
            }
            const string url = "https://github.com/aspenlaub/Chab.git";
            Repository.Clone(url, ChabTarget.FullName(), new CloneOptions { BranchName = "master" });
        }

        [Given(@"I copy the latest build\.cake script from my Shatilaya solution")]
        public void GivenIHaveTheLatestBuild_CakeScript() {
            var latestBuildCakeScriptProvider = new LatestBuildCakeScriptProvider();
            var latestScript = latestBuildCakeScriptProvider.GetLatestBuildCakeScript();
            Assert.IsTrue(latestScript.Length > 120);
            Assert.IsTrue(latestScript.Contains("#load \"solution.cake\""));
            var currentScriptFileName = ChabTarget.FullName() + @"\build.cake";
            var currentScript = File.ReadAllText(currentScriptFileName);
            if (latestScript == currentScript) { return; }

            Assert.IsTrue(latestScript.Contains(@"checkIfBuildCakeIsOutdated = true;"));
            latestScript = latestScript.Replace(@"checkIfBuildCakeIsOutdated = true;", @"checkIfBuildCakeIsOutdated = false;");
            File.WriteAllText(currentScriptFileName, latestScript);
        }

        [Given(@"I change the cake script so that debug build is suppressed")]
        public void GivenIChangeTheCakeScriptSoThatDebugBuildIsSuppressed() {
            var scriptFileName = ChabTarget.FullName() + @"\build.cake";
            var script = File.ReadAllText(scriptFileName);
            Assert.IsTrue(script.Contains(@"doDebugCompilation = true;"));
            script = script.Replace(@"doDebugCompilation = true;", @"doDebugCompilation = false;");
            File.WriteAllText(scriptFileName, script);
        }

        [Given(@"Nuget packages are not restored yet")]
        public void GivenNugetPackagesAreNotRestoredYet() {
            Assert.IsFalse(OctoPackFolder().Exists());
        }

        [Given(@"I change a source file so that it cannot be compiled")]
        public void GivenIChangeASourceFileSoThatItCannotBeCompiled() {
            var folder = ChabTarget.Folder().SubFolder("src");
            var fileName = folder.FullName + @"\Oven.cs";
            Assert.IsTrue(File.Exists(fileName));
            var contents = File.ReadAllText(fileName);
            Assert.IsTrue(contents.Contains(@"new Cake()"));
            contents = contents.Replace(@"new Cake()", @"old Cake()");
            File.WriteAllText(fileName, contents);
        }

        [Given(@"I clean up the master debug folder")]
        public void GivenICleanUpTheMasterDebugFolder() {
            var folder = ChabTarget.MasterDebugBinFolder();
            if (!folder.Exists()) { return; }

            var deleter = new FolderDeleter();
            deleter.DeleteFolder(folder);
        }

        [Given(@"I run the build\.cake script")]
        public void GivenIRunTheBuild_CakeScript() {
            ChabTarget.RunBuildCakeScript(ComponentProvider, CakeErrorsAndInfos);
        }

        [Given(@"I save the master debug folder file names and timestamps")]
        public void GivenISaveTheMasterDebugFolderFileNamesAndTimestamps() {
            MasterDebugBinFolderSnapshot = new Dictionary<string, DateTime>();
            var folder = ChabTarget.MasterDebugBinFolder();
            Assert.IsTrue(folder.Exists());
            foreach (var fileName in Directory.GetFiles(folder.FullName, "*.*")) {
                MasterDebugBinFolderSnapshot[fileName] = File.GetLastWriteTime(fileName);
            }
        }

        [Given(@"I wait two seconds")]
        public void GivenIWaitTwoSeconds() {
            Thread.Sleep(TimeSpan.FromSeconds(2));
        }

        [Given(@"no cake errors were reported")]
        public void GivenNoCakeErrorsWereReported() {
            Assert.IsFalse(CakeErrorsAndInfos.Errors.Any(), string.Join("\r\n", CakeErrorsAndInfos.Errors));
        }

        [Given(@"I change a test case so that it will fail")]
        public void GivenIChangeATestCaseSoThatItWillFail() {
            var folder = ChabTarget.Folder().SubFolder(@"src\Test");
            var fileName = folder.FullName + @"\OvenTest.cs";
            Assert.IsTrue(File.Exists(fileName));
            var contents = File.ReadAllText(fileName);
            Assert.IsTrue(contents.Contains(@"Assert.IsNotNull"));
            contents = contents.Replace(@"Assert.IsNotNull", @"Assert.IsNull");
            File.WriteAllText(fileName, contents);
        }

        [Given(@"I clean up the master release folder")]
        public void GivenICleanUpTheMasterReleaseFolder() {
            var folder = ChabTarget.MasterReleaseBinFolder();
            if (!folder.Exists()) { return; }

            var deleter = new FolderDeleter();
            deleter.DeleteFolder(folder);
        }

        [Given(@"I save the master release folder file names and timestamps")]
        public void GivenISaveTheMasterReleaseFolderFileNamesAndTimestamps() {
            MasterReleaseBinFolderSnapshot = new Dictionary<string, DateTime>();
            var folder = ChabTarget.MasterReleaseBinFolder();
            Assert.IsTrue(folder.Exists());
            foreach (var fileName in Directory.GetFiles(folder.FullName, "*.*")) {
                MasterReleaseBinFolderSnapshot[fileName] = File.GetLastWriteTime(fileName);
            }
        }

        [Given(@"I change a test case so that it will fail in release")]
        public void GivenIChangeATestCaseSoThatItWillFailInRelease() {
            var folder = ChabTarget.Folder().SubFolder(@"src\Test");
            var fileName = folder.FullName + @"\OvenTest.cs";
            Assert.IsTrue(File.Exists(fileName));
            var contents = File.ReadAllText(fileName);
            Assert.IsTrue(contents.Contains(@"Assert.IsNotNull(cake);"));
            contents = contents.Replace("Assert.IsNotNull(cake);", "#if DEBUG\r\nAssert.IsNotNull(cake);\r\n#else\r\nAssert.IsNull(cake);\r\n#endif");
            File.WriteAllText(fileName, contents);
        }

        [Given(@"I modify the build\.cake script")]
        public void GivenIModifyTheBuild_CakeScript() {
            var scriptFileName = ChabTarget.Folder().FullName + @"\build.cake";
            var contents = File.ReadAllText(scriptFileName);
            contents = contents.Replace("Please retry", "Retry");
            File.WriteAllText(scriptFileName, contents);
        }

        #endregion

        #region When
        [When(@"I run the build\.cake script")]
        public void WhenIRunTheBuild_CakeScript() {
            ChabTarget.RunBuildCakeScript(ComponentProvider, CakeErrorsAndInfos);
        }
        #endregion

        #region Then
        [Then(@"the build\.cake file is identical to the latest found on the GitHub Shatilaya master branch")]
        public void ThenTheBuild_CakeFileIsIdenticalToTheLatestFoundOnTheGitHubShatilayaMasterBranch() {
            const string url = @"https://raw.githubusercontent.com/aspenlaub/Shatilaya/master/build.cake";
            var request = WebRequest.Create(url) as HttpWebRequest;
            Assert.IsNotNull(request);
            using (var response = (HttpWebResponse)request.GetResponse()) {
                Assert.IsNotNull(response);
                var scriptFileFullName = ChabTarget.FullName() + @"\build.cake";
                var stream = response.GetResponseStream();
                Assert.IsNotNull(stream);
                string expectedContents;
                using (var reader = new StreamReader(stream, Encoding.Default)) {
                    expectedContents = reader.ReadToEnd();
                }
                var actualContents = File.ReadAllText(scriptFileFullName);

                /* NB if there are differences then have in mind that it is the Chab repository that we are cloning! */
                Assert.AreEqual(expectedContents, actualContents);
            }
        }

        [Then(@"no artifact exists")]
        public void ThenNoArtifactExists() {
            var files = Directory.GetFiles(ArtifactsFolder().FullName, "*.*", SearchOption.AllDirectories);
            Assert.IsFalse(files.Any());
        }

        [Then(@"no intermediate build output exists")]
        public void ThenNoIntermediateBuildOutputExists() {
            var files = Directory.GetFiles(IntermediateOutputFolder().FullName, "*.*", SearchOption.AllDirectories);
            Assert.IsFalse(files.Any());
        }

        [Then(@"the Nuget packages are restored")]
        public void ThenTheNugetPackagesAreRestored() {
            Assert.IsTrue(OctoPackFolder().Exists());
        }

        [Then(@"no cake errors were reported")]
        public void ThenNoCakeErrorsWereReported() {
            Assert.IsFalse(CakeErrorsAndInfos.Errors.Any(), string.Join("\r\n", CakeErrorsAndInfos.Errors));
        }

        [Then(@"a compilation error was reported for the changed source file")]
        public void ThenACompilationErrorWasReportedForTheChangedSourceFile() {
            Assert.IsTrue(CakeErrorsAndInfos.Errors.Any(e => e.Contains(@"MSBuild: Process returned an error")));
            Assert.IsTrue(CakeErrorsAndInfos.Infos.Any(m => m.Contains(@"Oven.cs") && m.Contains(@"error CS1002") && m.Contains(@"; expected")));
        }

        [Then(@"build step ""(.*)"" was skipped")]
        public void ThenBuildStepWasSkipped(string p0) {
            Assert.IsTrue(CakeErrorsAndInfos.Infos.Any(m => m.Contains(p0) && m.Contains(@"Skipped")));
        }

        [Then(@"I get an error message saying that I need to rerun my cake script")]
        public void ThenIGetAnErrorMessageSayingThatINeedToRerunMyCakeScript() {
            Assert.IsTrue(CakeErrorsAndInfos.Errors.Any(e => e.Contains(@"build.cake file has been updated")));
        }

        [Then(@"I find the artifacts in the master debug folder")]
        public void ThenIFindTheArtifactsInTheMasterDebugFolder() {
            var folder = ChabTarget.MasterDebugBinFolder();
            Assert.IsTrue(folder.Exists());
            Assert.IsTrue(File.Exists(folder.FullName + @"\Aspenlaub.Net.GitHub.CSharp.Chab.dll"));
            Assert.IsTrue(File.Exists(folder.FullName + @"\Aspenlaub.Net.GitHub.CSharp.Chab.Test.dll"));
            Assert.IsTrue(File.Exists(folder.FullName + @"\Aspenlaub.Net.GitHub.CSharp.Chab.Test.pdb"));
        }

        [Then(@"the contents of the master debug folder has not changed")]
        public void ThenTheContentsOfTheMasterDebugFolderHasNotChanged() {
            foreach (var snapShotFile in MasterDebugBinFolderSnapshot) {
                Assert.AreEqual(snapShotFile.Value, File.GetLastWriteTime(snapShotFile.Key));
            }
        }

        [Then(@"I do not find any artifacts in the master debug folder")]
        public void ThenIDoNotFindAnyArtifactsInTheMasterDebugFolder() {
            var folder = ChabTarget.MasterDebugBinFolder();
            Assert.IsFalse(folder.Exists() && Directory.GetFiles(folder.FullName, "*.*").Any());
        }

        [Then(@"a failed test case was reported")]
        public void ThenAFailedTestCaseWasReported() {
            Assert.IsTrue(CakeErrorsAndInfos.Errors.Any(e => e.Contains(@"MSTest: Process returned an error")));
            Assert.IsTrue(CakeErrorsAndInfos.Infos.Any(m => m.Contains(@"Failed") && m.Contains(@"OvenTest.CanBakeACake")));
        }

        [Then(@"(.*) ""(.*)"" artifact/-s was/were produced")]
        public void ThenArtifactsWasWereProduced(int p0, string p1) {
            var folder = ChabTarget.Folder().SubFolder(@"artifacts\" + p1);
            Assert.AreEqual(p0, Directory.GetFiles(folder.FullName, "*Chab*.dll", SearchOption.TopDirectoryOnly).Length);
        }

        [Then(@"(.*) ""(.*)"" nupkg file/-s was/were produced")]
        public void ThenNupkgFileWasWereProduced(int p0, string p1) {
            var folder = ChabTarget.Folder().SubFolder(@"artifacts\" + p1);
            Assert.AreEqual(p0, Directory.GetFiles(folder.FullName, "*.nupkg", SearchOption.TopDirectoryOnly).Length);
        }

        [Then(@"I find the artifacts in the master release folder")]
        public void ThenIFindTheArtifactsInTheMasterReleaseFolder() {
            var folder = ChabTarget.MasterReleaseBinFolder();
            Assert.IsTrue(folder.Exists());
            Assert.IsTrue(File.Exists(folder.FullName + @"\Aspenlaub.Net.GitHub.CSharp.Chab.dll"));
            Assert.IsTrue(File.Exists(folder.FullName + @"\Aspenlaub.Net.GitHub.CSharp.Chab.Test.dll"));
            Assert.IsTrue(File.Exists(folder.FullName + @"\Aspenlaub.Net.GitHub.CSharp.Chab.Test.pdb"));
        }

        [Then(@"the contents of the master release folder has not changed")]
        public void ThenTheContentsOfTheMasterReleaseFolderHasNotChanged() {
            foreach (var snapShotFile in MasterReleaseBinFolderSnapshot) {
                Assert.AreEqual(snapShotFile.Value, File.GetLastWriteTime(snapShotFile.Key));
            }
        }

        [Then(@"I do not find any artifacts in the master release folder")]
        public void ThenIDoNotFindAnyArtifactsInTheMasterReleaseFolder() {
            var folder = ChabTarget.MasterReleaseBinFolder();
            Assert.IsFalse(folder.Exists() && Directory.GetFiles(folder.FullName, "*.*").Any());
        }

        #endregion

        protected static IFolder OctoPackFolder() {
            return ChabTarget.Folder().SubFolder(@"src\packages\OctoPack.3.6.3");
        }

        protected static IFolder ArtifactsFolder() {
            return ChabTarget.Folder().SubFolder(@"artifacts");
        }

        protected static IFolder IntermediateOutputFolder() {
            return ChabTarget.Folder().SubFolder(@"temp/obj");
        }
    }
}

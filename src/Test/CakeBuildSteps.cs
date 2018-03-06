using System.Collections.Generic;
using System.IO;
using System.Linq;
using Aspenlaub.Net.GitHub.CSharp.Shatilaya.Entities;
using Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces;
using LibGit2Sharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TechTalk.SpecFlow;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Test {
    [Binding]
    public class CakeBuildSteps {
        protected IList<string> CakeErrors;

        [AfterFeature("CakeBuild")]
        public static void CleanUpFeature() {
            if (!CakeFolder().Exists()) { return; }

            var deleter = new FolderDeleter();
            deleter.DeleteFolder(CakeFolder());
        }

        [AfterScenario("CakeBuild")]
        public void CleanUpScenario() {
            if (!ChabFolder().Exists()) { return; }

            var deleter = new FolderDeleter();
            deleter.DeleteFolder(ChabFolder());
        }

        [Given(@"I have a green solution with unit tests in a temp folder")]
        public void GivenIHaveAGreenSolutionWithUnitTestsInATempFolder() {
            if (ChabFolder().Exists()) {
                CleanUpScenario();
            }
            if (!CakeFolder().Exists()) {
                CreateCakeFolder();
            }
            const string url = "https://github.com/aspenlaub/Chab.git";
            Repository.Clone(url, ChabFolder().FullName, new CloneOptions { BranchName = "master" });
        }

        [Given(@"I have the latest build\.cake script")]
        public void GivenIHaveTheLatestBuild_CakeScript() {
            var latestBuildCakeScriptProvider = new LatestBuildCakeScriptProvider();
            var latestScript = latestBuildCakeScriptProvider.GetLatestBuildCakeScript();
            Assert.IsTrue(latestScript.Length > 120);
            Assert.IsTrue(latestScript.Contains("#load \"solution.cake\""));
            var currentScriptFileName = ChabFolder().FullName + @"\build.cake";
            var currentScript = File.ReadAllText(currentScriptFileName);
            if (latestScript == currentScript) { return; }

            File.WriteAllText(currentScriptFileName, latestScript);
        }

        [Given(@"Nuget packages are not restored yet")]
        public void GivenNugetPackagesAreNotRestoredYet() {
            Assert.IsFalse(OctoPackFolder().Exists());
        }

        [When(@"I run the build\.cake script")]
        public void WhenIRunTheBuild_CakeScript() {
            var runner = new CakeRunner();
            var cakeExeFileFullName = CakeFolder().FullName + @"\tools\Cake\cake.exe";
            Assert.IsTrue(File.Exists(cakeExeFileFullName));
            var scriptFileFullName = ChabFolder().FullName + @"\build.cake";
            runner.CallCake(cakeExeFileFullName, scriptFileFullName, out CakeErrors);
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
            Assert.IsFalse(CakeErrors.Any(), string.Join("\r\n", CakeErrors));
        }

        protected static IFolder CakeFolder() {
            return new Folder(Path.GetTempPath() + nameof(CakeBuildSteps) + @"\Cake");
        }

        protected static IFolder ChabFolder() {
            return new Folder(Path.GetTempPath() + nameof(CakeBuildSteps) + @"\Chab");
        }

        protected void CreateCakeFolder() {
            var cakeInstaller = new CakeInstaller();
            cakeInstaller.InstallCake(CakeFolder());
        }

        protected static IFolder OctoPackFolder() {
            return ChabFolder().SubFolder(@"src\packages\OctoPack.3.6.3");
        }

        protected static IFolder ArtifactsFolder() {
            return ChabFolder().SubFolder(@"artifacts");
        }

        protected static IFolder IntermediateOutputFolder() {
            return ChabFolder().SubFolder(@"temp/obj");
        }
    }
}

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Aspenlaub.Net.GitHub.CSharp.Shatilaya.Entities;
using Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces;
using LibGit2Sharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TechTalk.SpecFlow;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Test {
    [Binding]
    public class CakeBuildSteps {
        protected IList<string> CakeMessages, CakeErrors;

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

        [Given(@"I copy the latest build\.cake script from my Shatilaya solution")]
        public void GivenIHaveTheLatestBuild_CakeScript() {
            var latestBuildCakeScriptProvider = new LatestBuildCakeScriptProvider();
            var latestScript = latestBuildCakeScriptProvider.GetLatestBuildCakeScript();
            Assert.IsTrue(latestScript.Length > 120);
            Assert.IsTrue(latestScript.Contains("#load \"solution.cake\""));
            var currentScriptFileName = ChabFolder().FullName + @"\build.cake";
            var currentScript = File.ReadAllText(currentScriptFileName);
            if (latestScript == currentScript) { return; }

            Assert.IsTrue(latestScript.Contains(@"checkIfBuildCakeIsOutdated = true;"));
            latestScript = latestScript.Replace(@"checkIfBuildCakeIsOutdated = true;", @"checkIfBuildCakeIsOutdated = false;");
            File.WriteAllText(currentScriptFileName, latestScript);
        }

        [Given(@"I change the cake script so that debug build is suppressed")]
        public void GivenIChangeTheCakeScriptSoThatDebugBuildIsSuppressed() {
            var scriptFileName = ChabFolder().FullName + @"\build.cake";
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
            var folder = ChabFolder().SubFolder("src");
            var fileName = folder.FullName + @"\Oven.cs";
            Assert.IsTrue(File.Exists(fileName));
            var contents = File.ReadAllText(fileName);
            Assert.IsTrue(contents.Contains(@"new Cake()"));
            contents = contents.Replace(@"new Cake()", @"old Cake()");
            File.WriteAllText(fileName, contents);
        }

        [When(@"I run the build\.cake script")]
        public void WhenIRunTheBuild_CakeScript() {
            var runner = new CakeRunner();
            var cakeExeFileFullName = CakeFolder().FullName + @"\tools\Cake\cake.exe";
            Assert.IsTrue(File.Exists(cakeExeFileFullName));
            var scriptFileFullName = ChabFolder().FullName + @"\build.cake";
            runner.CallCake(cakeExeFileFullName, scriptFileFullName, out CakeMessages, out CakeErrors);
        }

        [Then(@"the build\.cake file is identical to the latest found on the GitHub Shatilaya master branch")]
        public void ThenTheBuild_CakeFileIsIdenticalToTheLatestFoundOnTheGitHubShatilayaMasterBranch() {
            const string url = @"https://raw.githubusercontent.com/aspenlaub/Shatilaya/master/build.cake";
            var request = WebRequest.Create(url) as HttpWebRequest;
            Assert.IsNotNull(request);
            using (var response = (HttpWebResponse)request.GetResponse()) {
                Assert.IsNotNull(response);
                var scriptFileFullName = ChabFolder().FullName + @"\build.cake";
                var stream = response.GetResponseStream();
                Assert.IsNotNull(stream);
                string expectedContents;
                using (var reader = new StreamReader(stream, Encoding.Default)) {
                    expectedContents = reader.ReadToEnd();
                }
                var actualContents = File.ReadAllText(scriptFileFullName);
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
            Assert.IsFalse(CakeErrors.Any(), string.Join("\r\n", CakeErrors));
        }

        [Then(@"Debug artifacts were produced")]
        public void ThenDebugArtifactsWereProduced() {
            var folder = ChabFolder().SubFolder("artifacts");
            Assert.AreEqual(2, Directory.GetFiles(folder.FullName, "*Chab*.dll", SearchOption.TopDirectoryOnly).Length);
        }

        [Then(@"no nupkg files were produced")]
        public void ThenNoNupkgFilesWereProduced() {
            var folder = ChabFolder().SubFolder("artifacts");
            Assert.IsFalse(Directory.GetFiles(folder.FullName, "*.nupkg", SearchOption.TopDirectoryOnly).Any());
        }

        [Then(@"a compilation error was reported for the changed source file")]
        public void ThenACompilationErrorWasReportedForTheChangedSourceFile() {
            Assert.IsTrue(CakeErrors.Any(e => e.Contains(@"MSBuild: Process returned an error")));
            Assert.IsTrue(CakeMessages.Any(m => m.Contains(@"Oven.cs") && m.Contains(@"error CS1002") && m.Contains(@"; expected")));
        }

        [Then(@"build step ""(.*)"" was skipped")]
        public void ThenBuildStepWasSkipped(string p0) {
            Assert.IsTrue(CakeMessages.Any(m => m.Contains(p0) && m.Contains(@"Skipped")));
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

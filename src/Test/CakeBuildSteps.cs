using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using Aspenlaub.Net.GitHub.CSharp.Fusion;
using Aspenlaub.Net.GitHub.CSharp.Gitty;
using Aspenlaub.Net.GitHub.CSharp.Gitty.Extensions;
using Aspenlaub.Net.GitHub.CSharp.Gitty.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Gitty.TestUtilities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Components;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Extensions;
using Autofac;
using LibGit2Sharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuGet.Common;
using NuGet.Protocol;
using TechTalk.SpecFlow;
using IContainer = Autofac.IContainer;
// ReSharper disable UnusedMember.Global

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Test {
    [Binding]
    public class CakeBuildSteps {
        protected ErrorsAndInfos CakeErrorsAndInfos = new ErrorsAndInfos();
        protected IDictionary<string, DateTime> MasterDebugBinFolderSnapshot, MasterReleaseBinFolderSnapshot;
        protected static TestTargetFolder ChabStandardTarget = new TestTargetFolder(nameof(CakeBuildSteps), "ChabStandard");
        private static IContainer vContainer;
        protected IDictionary<string, DateTime> LastWriteTimes;

        public CakeBuildSteps() {
            LastWriteTimes = new Dictionary<string, DateTime>();
        }

        [BeforeFeature("CakeBuild")]
        public static void RecreateCakeFolder() {
            vContainer = new ContainerBuilder().UseGittyTestUtilities().UseFusionNuclideProtchAndGitty(new DummyCsArgumentPrompter()).Build();
            vContainer.Resolve<TestTargetInstaller>().DeleteCakeFolder(ChabStandardTarget);
            vContainer.Resolve<TestTargetInstaller>().CreateCakeFolder(ChabStandardTarget, out var errorsAndInfos);
            Assert.IsFalse(errorsAndInfos.Errors.Any(), errorsAndInfos.ErrorsPlusRelevantInfos());
        }

        [AfterFeature("CakeBuild")]
        public static void DeleteCakeFolder() {
            vContainer.Resolve<TestTargetInstaller>().DeleteCakeFolder(ChabStandardTarget);
        }

        [AfterScenario("CakeBuild")]
        public void CleanUpScenario() {
            ChabStandardTarget.Delete();
        }

        #region Given
        [Given(@"I have a green solution with unit tests in a temp folder")]
        public void GivenIHaveAGreenSolutionWithUnitTestsInATempFolder() {
            if (ChabStandardTarget.Exists()) {
                CleanUpScenario();
            }
            const string url = "https://github.com/aspenlaub/ChabStandard.git";
            var errorsAndInfos = new ErrorsAndInfos();
            vContainer.Resolve<IGitUtilities>().Clone(url, "master", ChabStandardTarget.Folder(), new CloneOptions { BranchName = "master" }, true, errorsAndInfos);
            Assert.IsFalse(errorsAndInfos.Errors.Any(), errorsAndInfos.ErrorsPlusRelevantInfos());
            vContainer.Resolve<IGitUtilities>().Pull(ChabStandardTarget.Folder(), "Shatilaya tester", "shatilayatester@aspenlaub.net" );
        }

        [Given(@"I copy the latest build\.cake script from my Shatilaya solution with a comment added at the top")]
        public void GivenIHaveTheLatestBuildCakeScript() {
            var errorsAndInfos = new ErrorsAndInfos();
            vContainer.Resolve<CakeBuildUtilities>().CopyCakeScriptEmbeddedInAssembly(Assembly.GetExecutingAssembly(), BuildCake.Standard, ChabStandardTarget, errorsAndInfos);
            Assert.IsFalse(errorsAndInfos.Errors.Any(), errorsAndInfos.ErrorsPlusRelevantInfos());
            var buildCakeFileName = ChabStandardTarget.FullName() + @"\" + BuildCake.Standard;
            var buildCake = File.ReadAllText(buildCakeFileName);
            File.WriteAllText(buildCakeFileName, "// This should make me different from the master build.cake\r\n" + buildCake);
        }

        [Given(@"Nuget packages are not restored yet")]
        public void GivenNugetPackagesAreNotRestoredYet() {
            var folder = ChabStandardTarget.Folder().SubFolder(@"src\packages\OctoPack.3.6.3");
            Assert.IsFalse(folder.Exists());
        }

        [Given(@"I change a source file so that it cannot be compiled")]
        public void GivenIChangeASourceFileSoThatItCannotBeCompiled() {
            var folder = ChabStandardTarget.Folder().SubFolder("src");
            var fileName = folder.FullName + @"\Oven.cs";
            Assert.IsTrue(File.Exists(fileName));
            var contents = File.ReadAllText(fileName);
            Assert.IsTrue(contents.Contains(@"new Cake()"));
            contents = contents.Replace(@"new Cake()", @"old Cake()");
            File.WriteAllText(fileName, contents);
        }

        [Given(@"I change a source file so that it still can be compiled")]
        public void GivenIChangeASourceFileSoThatItStillCanBeCompiled() {
            var folder = ChabStandardTarget.Folder().SubFolder("src");
            var fileName = folder.FullName + @"\Oven.cs";
            Assert.IsTrue(File.Exists(fileName));
            var contents = File.ReadAllText(fileName);
            Assert.IsTrue(contents.Contains(@"namespace Aspenlaub"));
            contents = contents.Replace(@"namespace Aspenlaub", "/* Commented */\r\n" + @"namespace Aspenlaub");
            File.WriteAllText(fileName, contents);
        }

        [Given(@"I clean up the master debug folder")]
        public void GivenICleanUpTheMasterDebugFolder() {
            var folder = ChabStandardTarget.MasterDebugBinFolder();
            if (!folder.Exists()) { return; }

            var deleter = new FolderDeleter();
            deleter.DeleteFolder(folder);
        }

        [Given(@"I run the build\.cake script")]
        public void GivenIRunTheBuild_CakeScript() {
            vContainer.Resolve<TestTargetRunner>().RunBuildCakeScript(BuildCake.Standard, ChabStandardTarget, "", CakeErrorsAndInfos);
        }

        [Given(@"I save the master debug folder file names and timestamps")]
        public void GivenISaveTheMasterDebugFolderFileNamesAndTimestamps() {
            MasterDebugBinFolderSnapshot = new Dictionary<string, DateTime>();
            var folder = ChabStandardTarget.MasterDebugBinFolder();
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
            Assert.IsFalse(CakeErrorsAndInfos.Errors.Any(), CakeErrorsAndInfos.ErrorsToString());
        }

        [Given(@"I change a test case so that it will fail")]
        public void GivenIChangeATestCaseSoThatItWillFail() {
            var folder = ChabStandardTarget.Folder().SubFolder(@"src\Test");
            var fileName = folder.FullName + @"\OvenTest.cs";
            Assert.IsTrue(File.Exists(fileName));
            var contents = File.ReadAllText(fileName);
            Assert.IsTrue(contents.Contains(@"Assert.IsNotNull"));
            contents = contents.Replace(@"Assert.IsNotNull", @"Assert.IsNull");
            File.WriteAllText(fileName, contents);
        }

        [Given(@"I clean up the master release folder")]
        public void GivenICleanUpTheMasterReleaseFolder() {
            var folder = ChabStandardTarget.MasterReleaseBinFolder();
            if (!folder.Exists()) { return; }

            var deleter = new FolderDeleter();
            deleter.DeleteFolder(folder);
        }

        [Given(@"I save the master release folder file names and timestamps")]
        public void GivenISaveTheMasterReleaseFolderFileNamesAndTimestamps() {
            MasterReleaseBinFolderSnapshot = new Dictionary<string, DateTime>();
            var folder = ChabStandardTarget.MasterReleaseBinFolder();
            Assert.IsTrue(folder.Exists());
            foreach (var fileName in Directory.GetFiles(folder.FullName, "*.*")) {
                MasterReleaseBinFolderSnapshot[fileName] = File.GetLastWriteTime(fileName);
            }
        }

        [Given(@"I change a test case so that it will fail in release")]
        public void GivenIChangeATestCaseSoThatItWillFailInRelease() {
            var folder = ChabStandardTarget.Folder().SubFolder(@"src\Test");
            var fileName = folder.FullName + @"\OvenTest.cs";
            Assert.IsTrue(File.Exists(fileName));
            var contents = File.ReadAllText(fileName);
            Assert.IsTrue(contents.Contains(@"Assert.IsNotNull(cake);"));
            contents = contents.Replace("Assert.IsNotNull(cake);", "#if DEBUG\r\nAssert.IsNotNull(cake);\r\n#else\r\nAssert.IsNull(cake);\r\n#endif");
            File.WriteAllText(fileName, contents);
        }

        [Given(@"I empty the nuspec file")]
        public void GivenIDeleteTheNuspecFile() {
            var nuSpecFileName = ChabStandardTarget.Folder().SubFolder("src").FullName + @"\ChabStandard.nuspec";
            File.WriteAllText(nuSpecFileName, "");
        }

        #endregion

        #region When
        [When(@"I run the build\.cake script")]
        public void WhenIRunTheBuild_CakeScript() {
            vContainer.Resolve<TestTargetRunner>().RunBuildCakeScript(BuildCake.Standard, ChabStandardTarget, "", CakeErrorsAndInfos);
        }

        [When(@"I run the build\.cake script with target ""(.*)""")]
        public void WhenIRunTheBuild_CakeScriptWithTarget(string target) {
            vContainer.Resolve<TestTargetRunner>().RunBuildCakeScript(BuildCake.Standard, ChabStandardTarget, target, CakeErrorsAndInfos);
        }
        #endregion

        #region Then
        [Then(@"the build\.cake file is identical to the latest found on the GitHub Shatilaya master branch")]
        public void ThenTheBuild_CakeFileIsIdenticalToTheLatestFoundOnTheGitHubShatilayaMasterBranch() {
            string expectedContents;
            do {
                expectedContents = LatestCakeFileOnTheGitHubShatilayaMasterBranch();
                if (expectedContents == "") {
                    Thread.Sleep(TimeSpan.FromSeconds(10));
                }
            } while (expectedContents == "");

            var scriptFileFullName = ChabStandardTarget.FullName() + @"\" + "build.cake";
            var actualContents = File.ReadAllText(scriptFileFullName).Replace("\r\n", "\n");

            /* NB if there are differences then have in mind that it is the ChabStandard repository that we are cloning! */
            Assert.AreEqual(expectedContents, actualContents);
        }

        private string LatestCakeFileOnTheGitHubShatilayaMasterBranch() {
            const string url = @"https://raw.githubusercontent.com/aspenlaub/Shatilaya/master/build.cake";
            var request = WebRequest.Create(url) as HttpWebRequest;
            Assert.IsNotNull(request);
            try {
                using (var response = (HttpWebResponse)request.GetResponse()) {
                    Assert.IsNotNull(response);
                    var stream = response.GetResponseStream();
                    Assert.IsNotNull(stream);
                    using (var reader = new StreamReader(stream, Encoding.Default)) {
                        return reader.ReadToEnd().Replace("\r\n", "\n");
                    }
                }
            } catch {
                return "";
            }
        }

        [Then(@"no artifact exists")]
        public void ThenNoArtifactExists() {
            var folder = ChabStandardTarget.Folder().SubFolder("src");
            var files = Directory.GetFiles(folder.FullName, "*.*", SearchOption.AllDirectories).Where(f => f.Contains(@"\bin\"));
            Assert.IsFalse(files.Any());
        }

        [Then(@"no cake errors were reported")]
        public void ThenNoCakeErrorsWereReported() {
            Assert.IsFalse(CakeErrorsAndInfos.Errors.Any(), CakeErrorsAndInfos.ErrorsPlusRelevantInfos());
            Assert.IsFalse(CakeErrorsAndInfos.Infos.Any(i => i.StartsWith("Could not load")), string.Join("\r\n", CakeErrorsAndInfos.Infos.Where(i => i.StartsWith("Could not load"))));
        }

        [Then(@"a compilation error was reported for the changed source file")]
        public void ThenACompilationErrorWasReportedForTheChangedSourceFile() {
            Assert.IsTrue(CakeErrorsAndInfos.Errors.Any(e => e.Contains(@"MSBuild: Process returned an error")), CakeErrorsAndInfos.ErrorsToString());
            Assert.IsTrue(CakeErrorsAndInfos.Infos.Any(m => m.Contains(@"Oven.cs") && m.Contains(@"error CS1002") && m.Contains(@"; expected")));
        }

        [Then(@"an uncommitted change error was reported for the changed source file")]
        public void ThenAUncommittedChangeErrorWasReportedForTheChangedSourceFile() {
            Assert.IsTrue(CakeErrorsAndInfos.Errors.Any(m => m.Contains(@"Oven.cs") && m.Contains("Uncommitted change", StringComparison.InvariantCultureIgnoreCase)), CakeErrorsAndInfos.ErrorsToString());
        }

        [Then(@"build step ""(.*)"" was not a target")]
        public void ThenBuildStepWasSkipped(string p0) {
            Assert.IsFalse(CakeErrorsAndInfos.Infos.Any(m => m.Contains(p0)));
        }

        [Then(@"I get an error message saying that I need to rerun my cake script")]
        public void ThenIGetAnErrorMessageSayingThatINeedToRerunMyCakeScript() {
            Assert.IsTrue(CakeErrorsAndInfos.Errors.Any(e => e.Contains(@"build.cake file has been updated")), CakeErrorsAndInfos.ErrorsToString());
        }

        [Then(@"I find the artifacts in the master debug folder")]
        public void ThenIFindTheArtifactsInTheMasterDebugFolder() {
            var folder = ChabStandardTarget.MasterDebugBinFolder();
            Assert.IsTrue(folder.Exists());
            Assert.IsTrue(File.Exists(folder.FullName + @"\Aspenlaub.Net.GitHub.CSharp.ChabStandard.dll"));
            Assert.IsTrue(File.Exists(folder.FullName + @"\Aspenlaub.Net.GitHub.CSharp.ChabStandard.pdb"));
            Assert.IsFalse(File.Exists(folder.FullName + @"\Aspenlaub.Net.GitHub.CSharp.ChabStandard.Test.dll"));
            Assert.IsFalse(File.Exists(folder.FullName + @"\Aspenlaub.Net.GitHub.CSharp.ChabStandard.Test.pdb"));
        }

        [Then(@"the contents of the master debug folder has not changed")]
        public void ThenTheContentsOfTheMasterDebugFolderHasNotChanged() {
            foreach (var snapShotFile in MasterDebugBinFolderSnapshot) {
                VerifyEqualLastWriteTime(snapShotFile.Key, snapShotFile.Value);
            }
        }

        [Then(@"I do not find any artifacts in the master debug folder")]
        public void ThenIDoNotFindAnyArtifactsInTheMasterDebugFolder() {
            var folder = ChabStandardTarget.MasterDebugBinFolder();
            Assert.IsFalse(folder.Exists() && Directory.GetFiles(folder.FullName, "*.*").Any());
        }

        [Then(@"a failed test case was reported")]
        public void ThenAFailedTestCaseWasReported() {
            Assert.IsTrue(CakeErrorsAndInfos.Errors.Any(e => e.Contains(@"test run failed.", StringComparison.InvariantCultureIgnoreCase)), CakeErrorsAndInfos.ErrorsToString());
            Assert.IsTrue(CakeErrorsAndInfos.Infos.Any(m => m.Contains(@"X CanBakeACake")));
        }

        [Then(@"(.*) ""(.*)"" artifact/-s was/were produced")]
        public void ThenArtifactsWasWereProduced(int p0, string p1) {
            var folder = ChabStandardTarget.Folder().SubFolder(@"src");
            Assert.AreEqual(p0, Directory.GetFiles(folder.FullName, "*ChabStandard*.dll", SearchOption.AllDirectories).Count(f => f.Contains(@"\bin\" + p1 + @"\")));
        }

        [Then(@"(.*) ""(.*)"" nupkg file/-s was/were produced")]
        public void ThenNupkgFileWasWereProduced(int p0, string p1) {
            var folder = ChabStandardTarget.Folder().SubFolder(@"src\bin\" + p1);
            Assert.AreEqual(p0, Directory.GetFiles(folder.FullName, "*.nupkg", SearchOption.TopDirectoryOnly).Length);
        }

        [Then(@"I find the artifacts in the master release folder")]
        public void ThenIFindTheArtifactsInTheMasterReleaseFolder() {
            var folder = ChabStandardTarget.MasterReleaseBinFolder();
            Assert.IsTrue(folder.Exists());
            Assert.IsTrue(File.Exists(folder.FullName + @"\Aspenlaub.Net.GitHub.CSharp.ChabStandard.dll"));
            Assert.IsTrue(File.Exists(folder.FullName + @"\Aspenlaub.Net.GitHub.CSharp.ChabStandard.pdb"));
            Assert.IsFalse(File.Exists(folder.FullName + @"\Aspenlaub.Net.GitHub.CSharp.ChabStandard.Test.dll"));
            Assert.IsFalse(File.Exists(folder.FullName + @"\Aspenlaub.Net.GitHub.CSharp.ChabStandard.Test.pdb"));
        }

        [Then(@"the contents of the master release folder has not changed")]
        public void ThenTheContentsOfTheMasterReleaseFolderHasNotChanged() {
            foreach (var snapShotFile in MasterReleaseBinFolderSnapshot) {
                VerifyEqualLastWriteTime(snapShotFile.Key, snapShotFile.Value);
            }
        }

        protected void VerifyEqualLastWriteTime(string fileName, DateTime lastKnownWriteTime) {
            Assert.AreEqual(lastKnownWriteTime, File.GetLastWriteTime(fileName),
                fileName + " updated " + File.GetLastWriteTime(fileName).ToLongTimeString() + " after " + lastKnownWriteTime.ToLongTimeString());
        }

        [Then(@"I do not find any artifacts in the master release folder")]
        public void ThenIDoNotFindAnyArtifactsInTheMasterReleaseFolder() {
            var folder = ChabStandardTarget.MasterReleaseBinFolder();
            Assert.IsFalse(folder.Exists() && Directory.GetFiles(folder.FullName, "*.*").Any());
        }

        [Then(@"the number of ""(.*)"" files in the master ""(.*)"" folder is (.*)")]
        public void ThenTheNumberOfFilesInTheMasterFolderIs(string p0, string p1, int p2) {
            var folder = p1 == "Release" ? ChabStandardTarget.MasterReleaseBinFolder() : ChabStandardTarget.MasterDebugBinFolder();
            var numberOfFiles = Directory.GetFiles(folder.FullName, "*." + p0).Length;
            Assert.AreEqual(p2, numberOfFiles);
        }

        [Then(@"the newest file in the master ""(.*)"" folder is of type ""(.*)""")]
        public void ThenTheNewestFileInTheMasterFolderIsOfType(string p0, string p1) {
            var folder = p0 == "Release" ? ChabStandardTarget.MasterReleaseBinFolder() : ChabStandardTarget.MasterDebugBinFolder();
            var newestFile = folder.LastWrittenFileFullName();
            Assert.IsTrue(newestFile.EndsWith(p1));
        }

        [Then(@"I remember the last write time of the newest file in the master ""(.*)"" folder")]
        public void ThenIRememberTheLastWriteTimeOfTheNewestFileInTheMasterFolder(string p0) {
            var folder = p0 == "Release" ? ChabStandardTarget.MasterReleaseBinFolder() : ChabStandardTarget.MasterDebugBinFolder();
            var newestFile = folder.LastWrittenFileFullName();
            LastWriteTimes[p0] = File.GetLastWriteTime(newestFile);
        }

        [Then(@"the last write time of the newest file in the master ""(.*)"" folder is as remembered")]
        public void ThenTheLastWriteTimeOfTheNewestFileInTheMasterFolderIsAsRemembered(string p0) {
            Assert.IsTrue(LastWriteTimes.ContainsKey(p0));
            var folder = p0 == "Release" ? ChabStandardTarget.MasterReleaseBinFolder() : ChabStandardTarget.MasterDebugBinFolder();
            var newestFile = folder.LastWrittenFileFullName();
            Assert.AreEqual(LastWriteTimes[p0], File.GetLastWriteTime(newestFile));
        }

        [Then(@"a non-empty nuspec file is there again")]
        public void ThenANon_EmptyNuspecFileIsThereAgain() {
            var nuSpecFileName = ChabStandardTarget.Folder().SubFolder("src").FullName + @"\ChabStandard.nuspec";
            Assert.IsTrue(File.Exists(nuSpecFileName));
            Assert.IsTrue(File.ReadAllText(nuSpecFileName).Length > 1024);
        }

        [Then(@"the newest nuget package in the master ""(.*)"" folder is tagged with the head tip id sha")]
        public void ThenTheNewestNugetPackageInTheMasterFolderIsTaggedWithTheHeadTipIdSha(string p0) {
            var headTipIdSha = vContainer.Resolve<IGitUtilities>().HeadTipIdSha(ChabStandardTarget.Folder());
            var packagesFolder = p0 == "Release" ? ChabStandardTarget.MasterReleaseBinFolder().FullName : ChabStandardTarget.MasterDebugBinFolder().FullName;
            var repository = new FindLocalPackagesResourceV2(packagesFolder);
            var logger = new NullLogger();
            var packages = repository.GetPackages(logger, CancellationToken.None).ToList();
            var latestPackageVersion = packages.Max(p => p.Identity.Version.Version);
            var package = packages.FirstOrDefault(p => p.Identity.Version.Version == latestPackageVersion);
            Assert.IsNotNull(package);
            var tags = package.Nuspec.GetTags().Split(' ').Where(s => s != "").ToList();
            Assert.IsTrue(tags.Contains(headTipIdSha));
        }

        [Then(@"the newest nuget package in the master ""(.*)"" folder does not contain a test assembly")]
        public void ThenTheNewestNugetPackageInTheMasterFolderDoesNotContainATestAssembly(string p0) {
            var packagesFolder = p0 == "Release" ? ChabStandardTarget.MasterReleaseBinFolder().FullName : ChabStandardTarget.MasterDebugBinFolder().FullName;
            var repository = new FindLocalPackagesResourceV2(packagesFolder);
            var logger = new NullLogger();
            var packages = repository.GetPackages(logger, CancellationToken.None).ToList();
            var latestPackageVersion = packages.Max(p => p.Identity.Version.Version);
            var package = packages.FirstOrDefault(p => p.Identity.Version.Version == latestPackageVersion);
            Assert.IsNotNull(package);
            using (var reader = package.GetReader()) {
                var filesInPackage = reader.GetFiles().ToList();
                var wantedFiles = filesInPackage.Where(f => f.EndsWith("ChabStandard.dll") && !f.Contains("Test")).ToList();
                Assert.IsTrue(wantedFiles.Any());
                var unwantedFiles = filesInPackage.Where(f => f.Contains("Test")).ToList();
                Assert.IsFalse(unwantedFiles.Any());
            }
        }

        #endregion
    }
}

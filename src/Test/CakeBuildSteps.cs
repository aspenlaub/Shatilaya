using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using LibGit2Sharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TechTalk.SpecFlow;
using Aspenlaub.Net.GitHub.CSharp.Pegh;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Components;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using NuGet;
using IComponentProvider = Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces.IComponentProvider;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Test {
    [Binding]
    public class CakeBuildSteps {
        protected ErrorsAndInfos CakeErrorsAndInfos = new ErrorsAndInfos();
        protected IDictionary<string, DateTime> MasterDebugBinFolderSnapshot, MasterReleaseBinFolderSnapshot;
        protected static TestTargetFolder ChabTarget = new TestTargetFolder(nameof(CakeBuildSteps), "Chab");
        protected IComponentProvider ComponentProvider;
        protected IDictionary<string, DateTime> LastWriteTimes;

        public CakeBuildSteps() {
            var componentProviderMock = new Mock<IComponentProvider>();
            componentProviderMock.SetupGet(c => c.ProcessRunner).Returns(new ProcessRunner());
            componentProviderMock.SetupGet(c => c.CakeRunner).Returns(new CakeRunner(componentProviderMock.Object));
            componentProviderMock.SetupGet(c => c.GitUtilities).Returns(new GitUtilities());
            ComponentProvider = componentProviderMock.Object;
            LastWriteTimes = new Dictionary<string, DateTime>();
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
            var errorsAndInfos = new ErrorsAndInfos();
            ComponentProvider.GitUtilities.Clone(url, ChabTarget.Folder(), new CloneOptions { BranchName = "master" }, true, errorsAndInfos);
            Assert.IsFalse(errorsAndInfos.Errors.Any(), string.Join("\r\n", errorsAndInfos.Errors));
        }

        [Given(@"I copy the latest build\.cake script from my Shatilaya solution and reference the local assemblies")]
        public void GivenIHaveTheLatestBuildCakeScript() {
            CakeBuildUtilities.CopyLatestScriptFromShatilayaSolution(ChabTarget);
        }

        [Given(@"Nuget packages are not restored yet")]
        public void GivenNugetPackagesAreNotRestoredYet() {
            var folder = ChabTarget.Folder().SubFolder(@"src\packages\OctoPack.3.6.3");
            Assert.IsFalse(folder.Exists());
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

        [Given(@"I change a source file so that it still can be compiled")]
        public void GivenIChangeASourceFileSoThatItStillCanBeCompiled() {
            var folder = ChabTarget.Folder().SubFolder("src");
            var fileName = folder.FullName + @"\Oven.cs";
            Assert.IsTrue(File.Exists(fileName));
            var contents = File.ReadAllText(fileName);
            Assert.IsTrue(contents.Contains(@"namespace Aspenlaub"));
            contents = contents.Replace(@"namespace Aspenlaub", "/* Commented */\r\n" + @"namespace Aspenlaub");
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

        [Given(@"I empty the nuspec file")]
        public void GivenIDeleteTheNuspecFile() {
            var nuSpecFileName = ChabTarget.Folder().SubFolder("src").FullName + @"\Chab.nuspec";
            File.WriteAllText(nuSpecFileName, "");
        }

        #endregion

        #region When
        [When(@"I run the build\.cake script")]
        public void WhenIRunTheBuild_CakeScript() {
            ChabTarget.RunBuildCakeScript(ComponentProvider, CakeErrorsAndInfos);
        }

        [When(@"I run the build\.cake script with target ""(.*)""")]
        public void WhenIRunTheBuild_CakeScriptWithTarget(string target) {
            ChabTarget.RunBuildCakeScript(ComponentProvider, target, CakeErrorsAndInfos);
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
                    expectedContents = reader.ReadToEnd().Replace("\r\n", "\n");
                }
                var actualContents = File.ReadAllText(scriptFileFullName).Replace("\r\n", "\n");

                /* NB if there are differences then have in mind that it is the Chab repository that we are cloning! */
                Assert.AreEqual(expectedContents, actualContents);
            }
        }

        [Then(@"no artifact exists")]
        public void ThenNoArtifactExists() {
            var folder = ChabTarget.Folder().SubFolder("src");
            var files = Directory.GetFiles(folder.FullName, "*.*", SearchOption.AllDirectories).Where(f => f.Contains(@"\bin\"));
            Assert.IsFalse(files.Any());
        }

        [Then(@"no intermediate build output exists")]
        public void ThenNoIntermediateBuildOutputExists() {
            var folder = ChabTarget.Folder().SubFolder("src");
            var files = Directory.GetFiles(folder.FullName, "*.*", SearchOption.AllDirectories).Where(f => f.Contains(@"\obj\"));
            Assert.IsFalse(files.Any());
        }

        [Then(@"the Nuget packages are restored")]
        public void ThenTheNugetPackagesAreRestored() {
            var folder = ChabTarget.Folder().SubFolder(@"src\packages\OctoPack.3.6.3");
            Assert.IsTrue(folder.Exists());
        }

        [Then(@"no cake errors were reported")]
        public void ThenNoCakeErrorsWereReported() {
            Assert.IsFalse(CakeErrorsAndInfos.Errors.Any(), string.Join("\r\n", CakeErrorsAndInfos.Errors));
        }

        [Then(@"a compilation error was reported for the changed source file")]
        public void ThenACompilationErrorWasReportedForTheChangedSourceFile() {
            Assert.IsTrue(CakeErrorsAndInfos.Errors.Any(e => e.Contains(@"MSBuild: Process returned an error")), string.Join("\r\n", CakeErrorsAndInfos.Errors));
            Assert.IsTrue(CakeErrorsAndInfos.Infos.Any(m => m.Contains(@"Oven.cs") && m.Contains(@"error CS1002") && m.Contains(@"; expected")));
        }

        [Then(@"an uncommitted change error was reported for the changed source file")]
        public void ThenAUncommittedChangeErrorWasReportedForTheChangedSourceFile() {
            Assert.IsTrue(CakeErrorsAndInfos.Errors.Any(m => m.Contains(@"Oven.cs") && m.Contains("ncommitted change")), string.Join("\r\n", CakeErrorsAndInfos.Errors));
        }

        [Then(@"build step ""(.*)"" was not a target")]
        public void ThenBuildStepWasSkipped(string p0) {
            Assert.IsFalse(CakeErrorsAndInfos.Infos.Any(m => m.Contains(p0)));
        }

        [Then(@"I get an error message saying that I need to rerun my cake script")]
        public void ThenIGetAnErrorMessageSayingThatINeedToRerunMyCakeScript() {
            Assert.IsTrue(CakeErrorsAndInfos.Errors.Any(e => e.Contains(@"build.cake file has been updated")), string.Join("\r\n", CakeErrorsAndInfos.Errors));
        }

        [Then(@"I find the artifacts in the master debug folder")]
        public void ThenIFindTheArtifactsInTheMasterDebugFolder() {
            var folder = ChabTarget.MasterDebugBinFolder();
            Assert.IsTrue(folder.Exists());
            Assert.IsTrue(File.Exists(folder.FullName + @"\Aspenlaub.Net.GitHub.CSharp.Chab.dll"));
            Assert.IsTrue(File.Exists(folder.FullName + @"\Aspenlaub.Net.GitHub.CSharp.Chab.pdb"));
            Assert.IsFalse(File.Exists(folder.FullName + @"\Aspenlaub.Net.GitHub.CSharp.Chab.Test.dll"));
            Assert.IsFalse(File.Exists(folder.FullName + @"\Aspenlaub.Net.GitHub.CSharp.Chab.Test.pdb"));
        }

        [Then(@"the contents of the master debug folder has not changed")]
        public void ThenTheContentsOfTheMasterDebugFolderHasNotChanged() {
            foreach (var snapShotFile in MasterDebugBinFolderSnapshot) {
                VerifyEqualLastWriteTime(snapShotFile.Key, snapShotFile.Value);
            }
        }

        [Then(@"I do not find any artifacts in the master debug folder")]
        public void ThenIDoNotFindAnyArtifactsInTheMasterDebugFolder() {
            var folder = ChabTarget.MasterDebugBinFolder();
            Assert.IsFalse(folder.Exists() && Directory.GetFiles(folder.FullName, "*.*").Any());
        }

        [Then(@"a failed test case was reported")]
        public void ThenAFailedTestCaseWasReported() {
            Assert.IsTrue(CakeErrorsAndInfos.Errors.Any(e => e.Contains(@"VSTest: Process returned an error")), string.Join("\r\n", CakeErrorsAndInfos.Errors));
            Assert.IsTrue(CakeErrorsAndInfos.Infos.Any(m => m.Contains(@"Failed") && m.Contains(@"CanBakeACake")));
        }

        [Then(@"(.*) ""(.*)"" artifact/-s was/were produced")]
        public void ThenArtifactsWasWereProduced(int p0, string p1) {
            var folder = ChabTarget.Folder().SubFolder(@"src");
            Assert.AreEqual(p0, Directory.GetFiles(folder.FullName, "*Chab*.dll", SearchOption.AllDirectories).Count(f => f.Contains(@"\bin\" + p1 + @"\")));
        }

        [Then(@"(.*) ""(.*)"" nupkg file/-s was/were produced")]
        public void ThenNupkgFileWasWereProduced(int p0, string p1) {
            var folder = ChabTarget.Folder().SubFolder(@"src\bin\" + p1);
            Assert.AreEqual(p0, Directory.GetFiles(folder.FullName, "*.nupkg", SearchOption.TopDirectoryOnly).Length);
        }

        [Then(@"I find the artifacts in the master release folder")]
        public void ThenIFindTheArtifactsInTheMasterReleaseFolder() {
            var folder = ChabTarget.MasterReleaseBinFolder();
            Assert.IsTrue(folder.Exists());
            Assert.IsTrue(File.Exists(folder.FullName + @"\Aspenlaub.Net.GitHub.CSharp.Chab.dll"));
            Assert.IsTrue(File.Exists(folder.FullName + @"\Aspenlaub.Net.GitHub.CSharp.Chab.pdb"));
            Assert.IsFalse(File.Exists(folder.FullName + @"\Aspenlaub.Net.GitHub.CSharp.Chab.Test.dll"));
            Assert.IsFalse(File.Exists(folder.FullName + @"\Aspenlaub.Net.GitHub.CSharp.Chab.Test.pdb"));
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
            var folder = ChabTarget.MasterReleaseBinFolder();
            Assert.IsFalse(folder.Exists() && Directory.GetFiles(folder.FullName, "*.*").Any());
        }

        [Then(@"the number of ""(.*)"" files in the master ""(.*)"" folder is (.*)")]
        public void ThenTheNumberOfFilesInTheMasterFolderIs(string p0, string p1, int p2) {
            var folder = p1 == "Release" ? ChabTarget.MasterReleaseBinFolder() : ChabTarget.MasterDebugBinFolder();
            var numberOfFiles = Directory.GetFiles(folder.FullName, "*." + p0).Length;
            Assert.AreEqual(p2, numberOfFiles);
        }

        [Then(@"the newest file in the master ""(.*)"" folder is of type ""(.*)""")]
        public void ThenTheNewestFileInTheMasterFolderIsOfType(string p0, string p1) {
            var folder = p0 == "Release" ? ChabTarget.MasterReleaseBinFolder() : ChabTarget.MasterDebugBinFolder();
            var newestFile = folder.LastWrittenFileFullName();
            Assert.IsTrue(newestFile.EndsWith(p1));
        }

        [Then(@"I remember the last write time of the newest file in the master ""(.*)"" folder")]
        public void ThenIRememberTheLastWriteTimeOfTheNewestFileInTheMasterFolder(string p0) {
            var folder = p0 == "Release" ? ChabTarget.MasterReleaseBinFolder() : ChabTarget.MasterDebugBinFolder();
            var newestFile = folder.LastWrittenFileFullName();
            LastWriteTimes[p0] = File.GetLastWriteTime(newestFile);
        }

        [Then(@"the last write time of the newest file in the master ""(.*)"" folder is as remembered")]
        public void ThenTheLastWriteTimeOfTheNewestFileInTheMasterFolderIsAsRemembered(string p0) {
            Assert.IsTrue(LastWriteTimes.ContainsKey(p0));
            var folder = p0 == "Release" ? ChabTarget.MasterReleaseBinFolder() : ChabTarget.MasterDebugBinFolder();
            var newestFile = folder.LastWrittenFileFullName();
            Assert.AreEqual(LastWriteTimes[p0], File.GetLastWriteTime(newestFile));
        }

        [Then(@"a non-empty nuspec file is there again")]
        public void ThenANon_EmptyNuspecFileIsThereAgain() {
            var nuSpecFileName = ChabTarget.Folder().SubFolder("src").FullName + @"\Chab.nuspec";
            Assert.IsTrue(File.Exists(nuSpecFileName));
            Assert.IsTrue(File.ReadAllText(nuSpecFileName).Length > 1024);
        }

        [Then(@"the newest nuget package in the master ""(.*)"" folder is tagged with the head tip id sha")]
        public void ThenTheNewestNugetPackageInTheMasterFolderIsTaggedWithTheHeadTipIdSha(string p0) {
            var headTipIdSha = ComponentProvider.GitUtilities.HeadTipIdSha(ChabTarget.Folder());
            var packagesFolder = p0 == "Release" ? ChabTarget.MasterReleaseBinFolder().FullName : ChabTarget.MasterDebugBinFolder().FullName;
            var repository = new LocalPackageRepository(packagesFolder);
            var packages = repository.GetPackages();
            var latestPackageVersion = packages.Max(p => p.Version);
            var package = packages.FirstOrDefault(p => p.Version == latestPackageVersion);
            Assert.IsNotNull(package);
            var tags = package.Tags.Split(' ').Where(s => s != "").ToList();
            Assert.IsTrue(tags.Contains(headTipIdSha));
        }

        [Then(@"the newest nuget package in the master ""(.*)"" folder does not contain a test assembly")]
        public void ThenTheNewestNugetPackageInTheMasterFolderDoesNotContainATestAssembly(string p0) {
            var packagesFolder = p0 == "Release" ? ChabTarget.MasterReleaseBinFolder().FullName : ChabTarget.MasterDebugBinFolder().FullName;
            var repository = new LocalPackageRepository(packagesFolder);
            var packages = repository.GetPackages();
            var latestPackageVersion = packages.Max(p => p.Version);
            var package = packages.FirstOrDefault(p => p.Version == latestPackageVersion);
            Assert.IsNotNull(package);
            var unwantedReferences = package.AssemblyReferences.Where(a => a.Name.Contains("Test")).ToList();
            Assert.IsFalse(unwantedReferences.Any());
        }

        [Then(@"there is no obj folder in the src folder")]
        public void ThenThereIsNoObjFolderInTheSrcFolder() {
            var folder = ChabTarget.Folder().SubFolder("src").FullName;
            var objFolders = Directory.GetDirectories(folder, "obj", SearchOption.AllDirectories).ToList();
            Assert.AreEqual(0, objFolders.Count, $"There is {objFolders.FirstOrDefault()}");
        }

        #endregion
    }
}

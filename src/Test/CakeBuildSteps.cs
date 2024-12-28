using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Fusion;
using Aspenlaub.Net.GitHub.CSharp.Gitty;
using Aspenlaub.Net.GitHub.CSharp.Gitty.Extensions;
using Aspenlaub.Net.GitHub.CSharp.Gitty.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Gitty.TestUtilities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Components;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Extensions;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Autofac;
using LibGit2Sharp;
using NuGet.Common;
using NuGet.Protocol;
using Reqnroll;

// ReSharper disable UnusedMember.Global

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Test;

[Binding]
public class CakeBuildSteps {
    protected ErrorsAndInfos CakeErrorsAndInfos = new();
    protected IDictionary<string, DateTime> MasterDebugBinFolderWriteTimeSnapshot,
        MasterReleaseBinFolderWriteTimeSnapshot, MasterReleaseCandidateBinFolderWriteTimeSnapshot;
    protected IDictionary<string, string> MasterReleaseBinFolderContentsSnapshot,
        MasterReleaseCandidateBinFolderContentsSnapshot;
    protected static TestTargetFolder ChabTarget = new(nameof(CakeBuildSteps), "Chab");
    private static IContainer _container;
    protected IDictionary<string, DateTime> LastWriteTimes = new Dictionary<string, DateTime>();

    [BeforeFeature("CakeBuild")]
    public static void RecreateCakeFolder() {
        _container = new ContainerBuilder().UseGittyTestUtilities().UseFusionNuclideProtchAndGitty("Shatilaya", new DummyCsArgumentPrompter()).Build();
    }

    [BeforeScenario("CakeBuild")]
    public void BeforeScenario() {
        MasterDebugBinFolderWriteTimeSnapshot = new Dictionary<string, DateTime>();
        MasterReleaseBinFolderWriteTimeSnapshot = new Dictionary<string, DateTime>();
        MasterReleaseCandidateBinFolderWriteTimeSnapshot = new Dictionary<string, DateTime>();
        MasterReleaseBinFolderContentsSnapshot = new Dictionary<string, string>();
        MasterReleaseCandidateBinFolderContentsSnapshot = new Dictionary<string, string>();
    }

    [AfterScenario("CakeBuild")]
    public void CleanUpScenario() {
        ChabTarget.Delete();
    }

    #region Given
    [Given(@"I have cloned the ""([^""]*)"" branch of a green solution with unit tests to a temp folder")]
    public void GivenIHaveClonedTheBranchOfAGreenSolutionWithUnitTestsToATempFolder(string branchId) {
        if (ChabTarget.Exists()) {
            CleanUpScenario();
        }
        const string url = "https://github.com/aspenlaub/Chab.git";
        var errorsAndInfos = new ErrorsAndInfos();
        _container.Resolve<IGitUtilities>().Clone(url, branchId, ChabTarget.Folder(), new CloneOptions { BranchName = branchId }, true, errorsAndInfos);
        Assert.IsFalse(errorsAndInfos.Errors.Any(), errorsAndInfos.ErrorsPlusRelevantInfos());
        _container.Resolve<IGitUtilities>().Pull(ChabTarget.Folder(), "Shatilaya tester", "shatilayatester@aspenlaub.net" );
    }

    [Given("I copy the latest cake script from my Shatilaya solution with a comment added at the top")]
    public void GivenIHaveTheLatestBuildCakeScript() {
        var errorsAndInfos = new ErrorsAndInfos();
        _container.Resolve<IEmbeddedCakeScriptCopier>().CopyCakeScriptEmbeddedInAssembly(Assembly.GetExecutingAssembly(), BuildCake.Standard, ChabTarget, errorsAndInfos);
        Assert.IsFalse(errorsAndInfos.Errors.Any(), errorsAndInfos.ErrorsPlusRelevantInfos());
        var buildCakeFileName = ChabTarget.FullName() + @"\" + BuildCake.Standard;
        var buildCake = File.ReadAllText(buildCakeFileName);
        File.WriteAllText(buildCakeFileName, "// This should make me different from the master cake\r\n" + buildCake);
    }

    [Given("Nuget packages are not restored yet")]
    public void GivenNugetPackagesAreNotRestoredYet() {
        var folder = ChabTarget.Folder().SubFolder(@"src\packages\OctoPack.3.6.3");
        Assert.IsFalse(folder.Exists());
    }

    [Given("I change a source file so that it cannot be compiled")]
    public void GivenIChangeASourceFileSoThatItCannotBeCompiled() {
        var folder = ChabTarget.Folder().SubFolder("src");
        var fileName = folder.FullName + @"\Oven.cs";
        Assert.IsTrue(File.Exists(fileName));
        var contents = File.ReadAllText(fileName);
        Assert.IsTrue(contents.Contains("new()"));
        contents = contents.Replace("new()", "old()");
        File.WriteAllText(fileName, contents);
    }

    [Given("I change a source file so that it still can be compiled")]
    public void GivenIChangeASourceFileSoThatItStillCanBeCompiled() {
        var folder = ChabTarget.Folder().SubFolder("src");
        var fileName = folder.FullName + @"\Oven.cs";
        Assert.IsTrue(File.Exists(fileName));
        var contents = File.ReadAllText(fileName);
        Assert.IsTrue(contents.Contains("namespace Aspenlaub"));
        contents = contents.Replace("namespace Aspenlaub", "/* Commented */\r\n" + "namespace Aspenlaub");
        File.WriteAllText(fileName, contents);
    }

    [Given("I clean up the master debug folder")]
    public void GivenICleanUpTheMasterDebugFolder() {
        var folder = ChabTarget.MasterDebugBinFolder();
        if (!folder.Exists()) { return; }

        var deleter = new FolderDeleter();
        deleter.DeleteFolder(folder);
    }

    [Given("I run the cake script")]
    public void GivenIRunTheBuild_CakeScript() {
        _container.Resolve<ITestTargetRunner>().RunBuildCakeScript(BuildCake.Standard, ChabTarget, "", CakeErrorsAndInfos);
    }

    [Given("I save the master debug folder file names and timestamps")]
    public void GivenISaveTheMasterDebugFolderFileNamesAndTimestamps() {
        var folder = ChabTarget.MasterDebugBinFolder();
        Assert.IsTrue(folder.Exists());
        foreach (var fileName in Directory.GetFiles(folder.FullName, "*.*")) {
            MasterDebugBinFolderWriteTimeSnapshot[fileName] = File.GetLastWriteTime(fileName);
        }
    }

    [Given("I wait two seconds")]
    public void GivenIWaitTwoSeconds() {
        Thread.Sleep(TimeSpan.FromSeconds(2));
    }

    [Given("I wait for a minute change on the clock")]
    public void GivenIWaitForAMinuteChangeOnTheClock() {
        var minute = DateTime.Now.Minute;
        do {
            Thread.Sleep(TimeSpan.FromSeconds(2));
        } while (minute == DateTime.Now.Minute);
    }

    [Given("no cake errors were reported")]
    public void GivenNoCakeErrorsWereReported() {
        Assert.IsFalse(CakeErrorsAndInfos.Errors.Any(), CakeErrorsAndInfos.ErrorsToString());
    }

    [Given("I change a test case so that it will fail")]
    public void GivenIChangeATestCaseSoThatItWillFail() {
        var folder = ChabTarget.Folder().SubFolder(@"src\Test");
        var fileName = folder.FullName + @"\OvenTest.cs";
        Assert.IsTrue(File.Exists(fileName));
        var contents = File.ReadAllText(fileName);
        Assert.IsTrue(contents.Contains("Assert.IsNotNull"));
        contents = contents.Replace("Assert.IsNotNull", "Assert.IsNull");
        File.WriteAllText(fileName, contents);
    }

    [Given("I clean up the master release folder")]
    public void GivenICleanUpTheMasterReleaseFolder() {
        var folder = ChabTarget.MasterReleaseBinFolder();
        if (!folder.Exists()) { return; }

        var deleter = new FolderDeleter();
        deleter.DeleteFolder(folder);
    }

    [Given("I clean up the master release candidate folder")]
    public void GivenICleanUpTheMasterReleaseCandidateFolder() {
        var folder = ChabTargetMasterReleaseCandidateFolder();
        if (!folder.Exists()) { return; }

        var deleter = new FolderDeleter();
        deleter.DeleteFolder(folder);
    }

    [Given("I save the master release folder file names and timestamps")]
    public void GivenISaveTheMasterReleaseFolderFileNamesAndTimestamps() {
        var folder = ChabTarget.MasterReleaseBinFolder();
        Assert.IsTrue(folder.Exists());
        foreach (var fileName in Directory.GetFiles(folder.FullName, "*.*")) {
            MasterReleaseBinFolderWriteTimeSnapshot[fileName] = File.GetLastWriteTime(fileName);
        }
    }

    [Given("I save the master release candidate folder file names and timestamps")]
    public void GivenISaveTheMasterReleaseCandidateFolderFileNamesAndTimestamps() {
        var folder = ChabTargetMasterReleaseCandidateFolder();
        Assert.IsTrue(folder.Exists());
        foreach (var fileName in Directory.GetFiles(folder.FullName, "*.*")) {
            MasterReleaseCandidateBinFolderWriteTimeSnapshot[fileName] = File.GetLastWriteTime(fileName);
        }
    }

    [Given("I save the contents of the master release json dependencies file")]
    public void GivenISaveTheContentsOfTheMasterReleaseJsonDependenciesFile() {
        var folder = ChabTarget.MasterReleaseBinFolder();
        Assert.IsTrue(folder.Exists());
        var fileName = Directory.GetFiles(folder.FullName, "*.deps.json").SingleOrDefault();
        Assert.IsNotNull(fileName);
        MasterReleaseBinFolderContentsSnapshot[fileName] = File.ReadAllText(fileName);
    }

    [Given("I save the contents of the master release candidate json dependencies file")]
    public void GivenISaveTheContentsOfTheMasterReleaseCandidateJsonDependenciesFile() {
        var folder = ChabTargetMasterReleaseCandidateFolder();
        Assert.IsTrue(folder.Exists());
        var fileName = Directory.GetFiles(folder.FullName, "*.deps.json").SingleOrDefault();
        Assert.IsNotNull(fileName);
        MasterReleaseCandidateBinFolderContentsSnapshot[fileName] = File.ReadAllText(fileName);
    }

    [Given("I change a test case so that it will fail in release")]
    public void GivenIChangeATestCaseSoThatItWillFailInRelease() {
        var folder = ChabTarget.Folder().SubFolder(@"src\Test");
        var fileName = folder.FullName + @"\OvenTest.cs";
        Assert.IsTrue(File.Exists(fileName));
        var contents = File.ReadAllText(fileName);
        Assert.IsTrue(contents.Contains("Assert.IsNotNull(cake);"));
        contents = contents.Replace("Assert.IsNotNull(cake);", "#if DEBUG\r\nAssert.IsNotNull(cake);\r\n#else\r\nAssert.IsNull(cake);\r\n#endif");
        File.WriteAllText(fileName, contents);
    }

    [Given("I empty the nuspec file")]
    public void GivenIDeleteTheNuspecFile() {
        var nuSpecFileName = ChabTarget.Folder().SubFolder("src").FullName + @"\Chab.nuspec";
        File.WriteAllText(nuSpecFileName, "");
    }

    #endregion

    #region When
    [When("I run the cake script")]
    public void WhenIRunTheBuild_CakeScript() {
        _container.Resolve<ITestTargetRunner>().RunBuildCakeScript(BuildCake.Standard, ChabTarget, "", CakeErrorsAndInfos);
    }

    [When(@"I run the cake script with target ""(.*)""")]
    public void WhenIRunTheBuild_CakeScriptWithTarget(string target) {
        _container.Resolve<ITestTargetRunner>().RunBuildCakeScript(BuildCake.Standard, ChabTarget, target, CakeErrorsAndInfos);
    }
    #endregion

    #region Then
    [Then("the cake file is identical to the latest found on the GitHub Shatilaya master branch")]
    public async Task ThenTheBuild_CakeFileIsIdenticalToTheLatestFoundOnTheGitHubShatilayaMasterBranchAsync() {
        string expectedContents;
        do {
            expectedContents = await LatestCakeFileOnTheGitHubShatilayaMasterBranchAsync();
            if (expectedContents == "") {
                Thread.Sleep(TimeSpan.FromSeconds(10));
            }
        } while (expectedContents == "");

        var scriptFileFullName = ChabTarget.FullName() + @"\" + BuildCake.Standard;
        var actualContents = (await File.ReadAllTextAsync(scriptFileFullName)).Replace("\r\n", "\n");

        /* NB if there are differences then have in mind that it is the Chab repository that we are cloning! */
        Assert.AreEqual(expectedContents, actualContents);
    }

    private async Task<string> LatestCakeFileOnTheGitHubShatilayaMasterBranchAsync() {
        const string url = "https://raw.githubusercontent.com/aspenlaub/Shatilaya/master/" + BuildCake.Standard;
        try {
            using var client = new HttpClient();
            var content = await client.GetStringAsync(url);
            return content.Replace("\r\n", "\n");
        } catch {
            return "";
        }
    }

    [Then("no artifact exists")]
    public void ThenNoArtifactExists() {
        var folder = ChabTarget.Folder().SubFolder("src");
        var files = Directory.GetFiles(folder.FullName, "*.*", SearchOption.AllDirectories).Where(f => f.Contains(@"\bin\"));
        Assert.IsFalse(files.Any());
    }

    [Then("no cake errors were reported")]
    public void ThenNoCakeErrorsWereReported() {
        Assert.IsFalse(CakeErrorsAndInfos.Errors.Any(), CakeErrorsAndInfos.ErrorsPlusRelevantInfos());
        Assert.IsFalse(CakeErrorsAndInfos.Infos.Any(i => i.StartsWith("Could not load")), string.Join("\r\n", CakeErrorsAndInfos.Infos.Where(i => i.StartsWith("Could not load"))));
    }

    [Then("the branch is considered the master branch or a branch with packages")]
    public void ThenTheBranchIsConsideredTheMasterBranchOrABranchWithPackages() {
        Assert.IsTrue(CakeErrorsAndInfos.Infos.Any(i => i == "Is master branch or branch with packages: true"));
    }

    [Then("a compilation error was reported for the changed source file")]
    public void ThenACompilationErrorWasReportedForTheChangedSourceFile() {
        Assert.IsTrue(CakeErrorsAndInfos.Errors.Any(e => e.Contains("MSBuild: Process returned an error")), CakeErrorsAndInfos.ErrorsToString());
        Assert.IsTrue(CakeErrorsAndInfos.Infos.Any(m => m.Contains("Oven.cs") && m.Contains("error CS0103") && m.Contains("'old' does not exist")));
    }

    [Then("an uncommitted change error was reported for the changed source file")]
    public void ThenAUncommittedChangeErrorWasReportedForTheChangedSourceFile() {
        Assert.IsTrue(CakeErrorsAndInfos.Errors.Any(m => m.Contains("Oven.cs") && m.Contains("Uncommitted change", StringComparison.InvariantCultureIgnoreCase)), CakeErrorsAndInfos.ErrorsToString());
    }

    [Then(@"build step ""(.*)"" was not a target")]
    public void ThenBuildStepWasSkipped(string p0) {
        Assert.IsFalse(CakeErrorsAndInfos.Infos.Any(m => m.Contains(p0)));
    }

    [Then("I get an error message saying that I need to rerun my cake script")]
    public void ThenIGetAnErrorMessageSayingThatINeedToRerunMyCakeScript() {
        Assert.IsTrue(CakeErrorsAndInfos.Errors.Any(e => e.Contains("Your cake file has been updated")), CakeErrorsAndInfos.ErrorsToString());
    }

    [Then("I find the artifacts in the master debug folder")]
    public void ThenIFindTheArtifactsInTheMasterDebugFolder() {
        var folder = ChabTarget.MasterDebugBinFolder();
        Assert.IsTrue(folder.Exists());
        Assert.IsTrue(File.Exists(folder.FullName + @"\Aspenlaub.Net.GitHub.CSharp.Chab.dll"));
        Assert.IsTrue(File.Exists(folder.FullName + @"\Aspenlaub.Net.GitHub.CSharp.Chab.pdb"));
        Assert.IsFalse(File.Exists(folder.FullName + @"\Aspenlaub.Net.GitHub.CSharp.Chab.Test.dll"));
        Assert.IsFalse(File.Exists(folder.FullName + @"\Aspenlaub.Net.GitHub.CSharp.Chab.Test.pdb"));
    }

    [Then("the contents of the master debug folder has not changed")]
    public void ThenTheContentsOfTheMasterDebugFolderHasNotChanged() {
        foreach (var snapShotFile in MasterDebugBinFolderWriteTimeSnapshot) {
            VerifyEqualLastWriteTime(snapShotFile.Key, snapShotFile.Value);
        }
    }

    [Then("I do not find any artifacts in the master debug folder")]
    public void ThenIDoNotFindAnyArtifactsInTheMasterDebugFolder() {
        var folder = ChabTarget.MasterDebugBinFolder();
        Assert.IsFalse(folder.Exists() && Directory.GetFiles(folder.FullName, "*.*").Any());
    }

    [Then(@"a failed ""(.*)"" test case was reported")]
    public void ThenAFailedTestCaseWasReported(string p0) {
        Assert.IsTrue(CakeErrorsAndInfos.Errors.Any(e => e.Contains($"An error occurred when executing task 'RunTestsOn{p0}Artifacts'", StringComparison.InvariantCultureIgnoreCase)), CakeErrorsAndInfos.ErrorsToString());
        Assert.IsTrue(CakeErrorsAndInfos.Infos.Any(m => m.Contains("Failed CanBakeACake")));
    }

    [Then(@"(.*) ""(.*)"" artifact/-s was/were produced")]
    public void ThenArtifactsWasWereProduced(int p0, string p1) {
        var folder = ChabTarget.Folder().SubFolder("src");
        Assert.AreEqual(p0, Directory.GetFiles(folder.FullName, "*Chab*.dll", SearchOption.AllDirectories)
                                     .Count(f => !f.Contains(@"\ref\") && f.Contains(@"\bin\" + p1 + @"\")));
    }

    [Then(@"(.*) ""(.*)"" nupkg file/-s was/were produced")]
    public void ThenNupkgFileWasWereProduced(int p0, string p1) {
        var folder = ChabTarget.Folder().SubFolder(@"src\bin\" + p1);
        Assert.AreEqual(p0, Directory.GetFiles(folder.FullName, "*.nupkg", SearchOption.TopDirectoryOnly).Length);
    }

    [Then("I find the artifacts in the master release folder")]
    public void ThenIFindTheArtifactsInTheMasterReleaseFolder() {
        var folder = ChabTarget.MasterReleaseBinFolder();
        Assert.IsTrue(folder.Exists());
        Assert.IsTrue(File.Exists(folder.FullName + @"\Aspenlaub.Net.GitHub.CSharp.Chab.dll"));
        Assert.IsTrue(File.Exists(folder.FullName + @"\Aspenlaub.Net.GitHub.CSharp.Chab.pdb"));
        Assert.IsFalse(File.Exists(folder.FullName + @"\Aspenlaub.Net.GitHub.CSharp.Chab.Test.dll"));
        Assert.IsFalse(File.Exists(folder.FullName + @"\Aspenlaub.Net.GitHub.CSharp.Chab.Test.pdb"));
    }

    [Then("I find the artifacts in the master release candidate folder")]
    public void ThenIFindTheArtifactsInTheMasterReleaseCandidateFolder() {
        var folder = ChabTargetMasterReleaseCandidateFolder();
        Assert.IsTrue(folder.Exists());
        Assert.IsTrue(File.Exists(folder.FullName + @"\Aspenlaub.Net.GitHub.CSharp.Chab.dll"));
        Assert.IsTrue(File.Exists(folder.FullName + @"\Aspenlaub.Net.GitHub.CSharp.Chab.pdb"));
        Assert.IsFalse(File.Exists(folder.FullName + @"\Aspenlaub.Net.GitHub.CSharp.Chab.Test.dll"));
        Assert.IsFalse(File.Exists(folder.FullName + @"\Aspenlaub.Net.GitHub.CSharp.Chab.Test.pdb"));
    }

    [Then("the contents of the master release folder has not changed")]
    public void ThenTheContentsOfTheMasterReleaseFolderHasNotChanged() {
        foreach (var snapShotFile in MasterReleaseBinFolderWriteTimeSnapshot) {
            VerifyEqualLastWriteTime(snapShotFile.Key, snapShotFile.Value);
        }
    }

    [Then("the contents of the master release candidate folder has changed")]
    public void ThenTheContentsOfTheMasterReleaseCandidateFolderHasChanged() {
        Assert.IsTrue(MasterReleaseCandidateBinFolderWriteTimeSnapshot.Any(
            snapShotFile => DifferentLastWriteTime(snapShotFile.Key, snapShotFile.Value)
        ));
    }

    [Then("the contents of the master release json dependencies file has not changed")]
    public void ThenTheContentsOfTheMasterReleaseJsonDependenciesFileHasNotChanged() {
        var folder = ChabTarget.MasterReleaseBinFolder();
        Assert.IsTrue(folder.Exists());
        var fileName = Directory.GetFiles(folder.FullName, "*.deps.json").SingleOrDefault();
        Assert.IsNotNull(fileName);
        var expectedContents = MasterReleaseBinFolderContentsSnapshot[fileName];
        var actualContents = File.ReadAllText(fileName);
        Assert.AreEqual(expectedContents.Length, actualContents.Length);
        var differences = Enumerable.Range(0, expectedContents.Length).Where(i => expectedContents[i] != actualContents[i]).ToList();
        Assert.AreEqual(0, differences.Count);
    }

    [Then("the contents of the master release candidate json dependencies file has changed")]
    public void ThenTheContentsOfTheMasterReleaseCandidateJsonDependenciesFileHasChanged() {
        var folder = ChabTargetMasterReleaseCandidateFolder();
        Assert.IsTrue(folder.Exists());
        var fileName = Directory.GetFiles(folder.FullName, "*.deps.json").SingleOrDefault();
        Assert.IsNotNull(fileName);
        var expectedContents = MasterReleaseCandidateBinFolderContentsSnapshot[fileName];
        var actualContents = File.ReadAllText(fileName);
        Assert.AreEqual(expectedContents.Length, actualContents.Length);
        var differences = Enumerable.Range(0, expectedContents.Length).Where(i => expectedContents[i] != actualContents[i]).ToList();
        Assert.AreEqual(2, differences.Count);
    }

    protected void VerifyEqualLastWriteTime(string fileName, DateTime lastKnownWriteTime) {
        Assert.AreEqual(lastKnownWriteTime, File.GetLastWriteTime(fileName),
            fileName + " updated " + File.GetLastWriteTime(fileName).ToLongTimeString() + " after " + lastKnownWriteTime.ToLongTimeString());
    }

    protected bool DifferentLastWriteTime(string fileName, DateTime lastKnownWriteTime) {
        return lastKnownWriteTime != File.GetLastWriteTime(fileName);
    }

    [Then("I do not find any artifacts in the master release folder")]
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

    [Then("a non-empty nuspec file is there again")]
    public void ThenANon_EmptyNuspecFileIsThereAgain() {
        var nuSpecFileName = ChabTarget.Folder().SubFolder("src").FullName + @"\Chab.nuspec";
        Assert.IsTrue(File.Exists(nuSpecFileName));
        Assert.IsTrue(File.ReadAllText(nuSpecFileName).Length > 1024);
    }

    [Then(@"the newest nuget package in the master ""(.*)"" folder is tagged with the head tip id sha")]
    public void ThenTheNewestNugetPackageInTheMasterFolderIsTaggedWithTheHeadTipIdSha(string p0) {
        var headTipIdSha = _container.Resolve<IGitUtilities>().HeadTipIdSha(ChabTarget.Folder());
        var packagesFolder = p0 == "Release" ? ChabTarget.MasterReleaseBinFolder().FullName : ChabTarget.MasterDebugBinFolder().FullName;
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
        var packagesFolder = p0 == "Release" ? ChabTarget.MasterReleaseBinFolder().FullName : ChabTarget.MasterDebugBinFolder().FullName;
        var repository = new FindLocalPackagesResourceV2(packagesFolder);
        var logger = new NullLogger();
        var packages = repository.GetPackages(logger, CancellationToken.None).ToList();
        var latestPackageVersion = packages.Max(p => p.Identity.Version.Version);
        var package = packages.FirstOrDefault(p => p.Identity.Version.Version == latestPackageVersion);
        Assert.IsNotNull(package);
        using var reader = package.GetReader();
        var filesInPackage = reader.GetFiles().ToList();
        var wantedFiles = filesInPackage.Where(f => f.EndsWith("Chab.dll") && !f.Contains("Test")).ToList();
        Assert.IsTrue(wantedFiles.Any());
        var unwantedFiles = filesInPackage.Where(f => f.Contains("Test")).ToList();
        Assert.IsFalse(unwantedFiles.Any());
    }

    #endregion

    private IFolder ChabTargetMasterReleaseCandidateFolder() {
        return new Folder(ChabTarget.MasterReleaseBinFolder().FullName.Replace("Release", "ReleaseCandidate"));
    }
}
using System.Linq;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Extensions;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Shatilaya.Entities;
using Aspenlaub.Net.GitHub.CSharp.Shatilaya.Extensions;
using Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces;
using LibGit2Sharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using IComponentProvider = Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces.IComponentProvider;
using PeghComponentProvider = Aspenlaub.Net.GitHub.CSharp.Pegh.Components.ComponentProvider;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Test {
    [TestClass]
    public class NugetPackageToPushFinderTest {
        protected static TestTargetFolder PakledCoreTarget = new TestTargetFolder(nameof(NugetPackageToPushFinderTest), "PakledCore");
        protected static TestTargetFolder ChabStandardTarget = new TestTargetFolder(nameof(NugetPackageToPushFinderTest), "ChabStandard");

        protected IComponentProvider ComponentProvider;

        public NugetPackageToPushFinderTest() {
            var componentProviderMock = new Mock<IComponentProvider>();
            componentProviderMock.SetupGet(c => c.ProcessRunner).Returns(new ProcessRunner());
            componentProviderMock.SetupGet(c => c.PeghComponentProvider).Returns(new PeghComponentProvider());
            componentProviderMock.SetupGet(c => c.CakeRunner).Returns(new CakeRunner(componentProviderMock.Object));
            componentProviderMock.SetupGet(c => c.NugetConfigReader).Returns(new NugetConfigReader());
            componentProviderMock.SetupGet(c => c.NugetFeedLister).Returns(new NugetFeedLister());
            componentProviderMock.SetupGet(c => c.GitUtilities).Returns(new GitUtilities());
            ComponentProvider = componentProviderMock.Object;
        }

        [ClassInitialize]
        public static void ClassInitialize(TestContext context) {
            PakledCoreTarget.DeleteCakeFolder();
            PakledCoreTarget.CreateCakeFolder();
            ChabStandardTarget.DeleteCakeFolder();
            ChabStandardTarget.CreateCakeFolder();
        }

        [ClassCleanup]
        public static void ClassCleanup() {
            PakledCoreTarget.DeleteCakeFolder();
            ChabStandardTarget.DeleteCakeFolder();
        }

        [TestInitialize]
        public void Initialize() {
            PakledCoreTarget.Delete();
            ChabStandardTarget.Delete();
        }

        [TestCleanup]
        public void TestCleanup() {
            PakledCoreTarget.Delete();
            ChabStandardTarget.Delete();
        }

        [TestMethod]
        public async Task CanFindNugetPackagesToPushForPakled() {
            var errorsAndInfos = new ErrorsAndInfos();
            var developerSettings = await GetDeveloperSettingsAsync(errorsAndInfos);

            CloneTarget(PakledCoreTarget, errorsAndInfos);

            ChangeCakeScriptAndRunIt(PakledCoreTarget, true, errorsAndInfos);

            errorsAndInfos = new ErrorsAndInfos();
            INugetPackageToPushFinder sut = new NugetPackageToPushFinder(ComponentProvider);
            var packageToPush = await sut.FindPackageToPushAsync(PakledCoreTarget.Folder().ParentFolder().SubFolder(PakledCoreTarget.SolutionId + @"Bin\Release"), PakledCoreTarget.Folder(), PakledCoreTarget.Folder().SubFolder("src").FullName + @"\" + PakledCoreTarget.SolutionId + ".sln", errorsAndInfos);
            Assert.IsFalse(errorsAndInfos.Errors.Any(), errorsAndInfos.ErrorsPlusRelevantInfos());
            Assert.AreEqual(developerSettings.NugetFeedUrl, packageToPush.FeedUrl);
            Assert.IsTrue(packageToPush.ApiKey.Length > 256);
        }

        private async Task<DeveloperSettings> GetDeveloperSettingsAsync(IErrorsAndInfos errorsAndInfos) {
            var developerSettingsSecret = new DeveloperSettingsSecret();
            var developerSettings = await ComponentProvider.PeghComponentProvider.SecretRepository.GetAsync(developerSettingsSecret, errorsAndInfos);
            Assert.IsFalse(errorsAndInfos.Errors.Any(), errorsAndInfos.ErrorsPlusRelevantInfos());
            Assert.IsNotNull(developerSettings);
            return developerSettings;
        }

        [TestMethod]
        public async Task PackageForTheSameCommitIsNotPushed() {
            var errorsAndInfos = new ErrorsAndInfos();
            var developerSettings = await GetDeveloperSettingsAsync(errorsAndInfos);
            Assert.IsFalse(errorsAndInfos.Errors.Any(), errorsAndInfos.ErrorsPlusRelevantInfos());

            CloneTarget(PakledCoreTarget, errorsAndInfos);

            var packages = await ComponentProvider.NugetFeedLister.ListReleasedPackagesAsync(developerSettings.NugetFeedUrl, @"Aspenlaub.Net.GitHub.CSharp." + PakledCoreTarget.SolutionId);
            if (!packages.Any()) { return; }

            var latestPackageVersion = packages.Max(p => p.Identity.Version.Version);
            var latestPackage = packages.First(p => p.Identity.Version.Version == latestPackageVersion);

            var headTipIdSha = ComponentProvider.GitUtilities.HeadTipIdSha(PakledCoreTarget.Folder());
            if (!latestPackage.Tags.Contains(headTipIdSha)) {
                return; // $"No package has been pushed for {headTipIdSha} and {PakledCoreTarget.SolutionId}, please run build.cake for this solution"
            }

            ChangeCakeScriptAndRunIt(PakledCoreTarget, false, errorsAndInfos);

            packages = await ComponentProvider.NugetFeedLister.ListReleasedPackagesAsync(developerSettings.NugetFeedUrl, @"Aspenlaub.Net.GitHub.CSharp." + PakledCoreTarget.SolutionId);
            Assert.AreEqual(latestPackageVersion, packages.Max(p => p.Identity.Version.Version));
        }

        private static void CloneTarget(TestTargetFolder testTargetFolder, IErrorsAndInfos errorsAndInfos) {
            var gitUtilities = new GitUtilities();
            var url = "https://github.com/aspenlaub/" + testTargetFolder.SolutionId + ".git";
            gitUtilities.Clone(url, testTargetFolder.Folder(), new CloneOptions { BranchName = "master" }, true, errorsAndInfos);
            Assert.IsFalse(errorsAndInfos.Errors.Any(), errorsAndInfos.ErrorsPlusRelevantInfos());
        }

        private void ChangeCakeScriptAndRunIt(TestTargetFolder testTargetFolder, bool disableNugetPush, IErrorsAndInfos errorsAndInfos) {
            CakeBuildUtilities.CopyLatestScriptFromShatilayaSolution(testTargetFolder);

            var projectLogic = new ProjectLogic();
            var projectFactory = new ProjectFactory();
            var solutionFileFullName = testTargetFolder.Folder().SubFolder("src").FullName + '\\' + testTargetFolder.SolutionId + ".sln";
            var projectErrorsAndInfos = new ErrorsAndInfos();
            Assert.IsTrue(projectLogic.DoAllNetStandardOrCoreConfigurationsHaveNuspecs(projectFactory.Load(solutionFileFullName, solutionFileFullName.Replace(".sln", ".csproj"), projectErrorsAndInfos)));

            var target = disableNugetPush ? "IgnoreOutdatedBuildCakePendingChangesAndDoNotPush" : "IgnoreOutdatedBuildCakePendingChanges";
            testTargetFolder.RunBuildCakeScript(ComponentProvider, target, errorsAndInfos);
            Assert.IsFalse(errorsAndInfos.Errors.Any(), errorsAndInfos.ErrorsPlusRelevantInfos());
        }

        [TestMethod]
        public async Task CanFindNugetPackagesToPushForChabStandard() {
            var errorsAndInfos = new ErrorsAndInfos();
            var developerSettings = await GetDeveloperSettingsAsync(errorsAndInfos);

            CloneTarget(ChabStandardTarget, errorsAndInfos);

            ChangeCakeScriptAndRunIt(ChabStandardTarget, true, errorsAndInfos);

            Assert.IsFalse(errorsAndInfos.Infos.Any(i => i.Contains("No test")));

            errorsAndInfos = new ErrorsAndInfos();
            var sut = new NugetPackageToPushFinder(ComponentProvider);
            var packageToPush = await sut.FindPackageToPushAsync(ChabStandardTarget.Folder().ParentFolder().SubFolder(ChabStandardTarget.SolutionId + @"Bin\Release"), ChabStandardTarget.Folder(), ChabStandardTarget.Folder().SubFolder("src").FullName + @"\" + ChabStandardTarget.SolutionId + ".sln", errorsAndInfos);
            Assert.IsFalse(errorsAndInfos.Errors.Any(), errorsAndInfos.ErrorsPlusRelevantInfos());
            Assert.AreEqual(developerSettings.NugetFeedUrl, packageToPush.FeedUrl);
            Assert.IsTrue(packageToPush.ApiKey.Length > 256);
        }
    }
}

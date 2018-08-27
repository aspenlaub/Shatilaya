using System.IO;
using System.Linq;
using LibGit2Sharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Aspenlaub.Net.GitHub.CSharp.Pegh;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Shatilaya.Entities;
using IComponentProvider = Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces.IComponentProvider;
using PeghComponentProvider = Aspenlaub.Net.GitHub.CSharp.Pegh.Components.ComponentProvider;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Test {
    [TestClass]
    public class NugetPackageToPushFinderTest {
        protected static TestTargetFolder PakledTarget = new TestTargetFolder(nameof(NugetPackageToPushFinderTest), "Pakled");
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
            PakledTarget.DeleteCakeFolder();
            PakledTarget.CreateCakeFolder();
            ChabStandardTarget.DeleteCakeFolder();
            ChabStandardTarget.CreateCakeFolder();
        }

        [ClassCleanup]
        public static void ClassCleanup() {
            PakledTarget.DeleteCakeFolder();
            ChabStandardTarget.DeleteCakeFolder();
        }

        [TestInitialize]
        public void Initialize() {
            PakledTarget.Delete();
            ChabStandardTarget.Delete();
        }

        [TestCleanup]
        public void TestCleanup() {
            PakledTarget.Delete();
            ChabStandardTarget.Delete();
        }

        [TestMethod]
        public void CanFindNugetPackagesToPushForPakled() {
            var errorsAndInfos = new ErrorsAndInfos();
            var developerSettings = DeveloperSettings(errorsAndInfos);

            CloneTarget(PakledTarget, errorsAndInfos);

            ChangeCakeScriptAndRunIt(PakledTarget, true, errorsAndInfos);

            errorsAndInfos = new ErrorsAndInfos();
            var sut = new NugetPackageToPushFinder(ComponentProvider);
            string packageFileFullName, feedUrl, apiKey;
            sut.FindPackageToPush(PakledTarget.Folder().ParentFolder().SubFolder(PakledTarget.SolutionId + @"Bin\Release"), PakledTarget.Folder(), PakledTarget.Folder().SubFolder("src").FullName + @"\" + PakledTarget.SolutionId + ".sln", out packageFileFullName, out feedUrl, out apiKey, errorsAndInfos);
            Assert.IsFalse(errorsAndInfos.Errors.Any(), string.Join("\r\n", errorsAndInfos.Errors));
            Assert.AreEqual(developerSettings.NugetFeedUrl, feedUrl);
            Assert.IsTrue(apiKey.Length > 256);
        }

        private DeveloperSettings DeveloperSettings(IErrorsAndInfos errorsAndInfos) {
            var developerSettingsSecret = new DeveloperSettingsSecret();
            var developerSettings = ComponentProvider.PeghComponentProvider.SecretRepository.Get(developerSettingsSecret, errorsAndInfos);
            Assert.IsFalse(errorsAndInfos.Errors.Any(), string.Join("\r\n", errorsAndInfos.Errors));
            Assert.IsNotNull(developerSettings);
            return developerSettings;
        }

        [TestMethod]
        public void PackageForTheSameCommitIsNotPushed() {
            var errorsAndInfos = new ErrorsAndInfos();
            var developerSettings = DeveloperSettings(errorsAndInfos);
            Assert.IsFalse(errorsAndInfos.Errors.Any(), string.Join("\r\n", errorsAndInfos.Errors));

            CloneTarget(PakledTarget, errorsAndInfos);

            var packages = ComponentProvider.NugetFeedLister.ListReleasedPackages(developerSettings.NugetFeedUrl, @"Aspenlaub.Net.GitHub.CSharp." + PakledTarget.SolutionId);
            if (!packages.Any()) { return; }

            var latestPackageVersion = packages.Max(p => p.Version);
            var latestPackage = packages.First(p => p.Version == latestPackageVersion);

            var headTipIdSha = ComponentProvider.GitUtilities.HeadTipIdSha(PakledTarget.Folder());
            if (!latestPackage.Tags.Contains(headTipIdSha)) {
                return; // $"No package has been pushed for {headTipIdSha} and {PakledTarget.SolutionId}, please run build.cake for this solution"
            }

            ChangeCakeScriptAndRunIt(PakledTarget, false, errorsAndInfos);

            packages = ComponentProvider.NugetFeedLister.ListReleasedPackages(developerSettings.NugetFeedUrl, @"Aspenlaub.Net.GitHub.CSharp." + PakledTarget.SolutionId);
            Assert.AreEqual(latestPackageVersion, packages.Max(p => p.Version));
        }

        private static void CloneTarget(TestTargetFolder testTargetFolder, IErrorsAndInfos errorsAndInfos) {
            var gitUtilities = new GitUtilities();
            var url = "https://github.com/aspenlaub/" + testTargetFolder.SolutionId + ".git";
            gitUtilities.Clone(url, testTargetFolder.Folder(), new CloneOptions { BranchName = "master" }, true, errorsAndInfos);
            Assert.IsFalse(errorsAndInfos.Errors.Any(), string.Join("\r\n", errorsAndInfos.Errors));
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
            Assert.IsFalse(errorsAndInfos.Errors.Any(), string.Join("\r\n", errorsAndInfos.Errors));
        }

        [TestMethod]
        public void CanFindNugetPackagesToPushForChabStandard() {
            var errorsAndInfos = new ErrorsAndInfos();
            var developerSettings = DeveloperSettings(errorsAndInfos);

            CloneTarget(ChabStandardTarget, errorsAndInfos);

            ChangeCakeScriptAndRunIt(ChabStandardTarget, true, errorsAndInfos);

            errorsAndInfos = new ErrorsAndInfos();
            var sut = new NugetPackageToPushFinder(ComponentProvider);
            string packageFileFullName, feedUrl, apiKey;
            sut.FindPackageToPush(ChabStandardTarget.Folder().ParentFolder().SubFolder(ChabStandardTarget.SolutionId + @"Bin\Release"), ChabStandardTarget.Folder(), ChabStandardTarget.Folder().SubFolder("src").FullName + @"\" + ChabStandardTarget.SolutionId + ".sln", out packageFileFullName, out feedUrl, out apiKey, errorsAndInfos);
            Assert.IsFalse(errorsAndInfos.Errors.Any(), string.Join("\r\n", errorsAndInfos.Errors));
            Assert.AreEqual(developerSettings.NugetFeedUrl, feedUrl);
            Assert.IsTrue(apiKey.Length > 256);
        }
    }
}

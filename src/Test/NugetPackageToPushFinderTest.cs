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
        protected static TestTargetFolder PakledTarget = new TestTargetFolder(nameof(NugetPackageToPushFinderTest), "PakledConsumer");
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
        }

        [ClassCleanup]
        public static void ClassCleanup() {
            PakledTarget.DeleteCakeFolder();
        }

        [TestInitialize]
        public void Initialize() {
            PakledTarget.Delete();
        }

        [TestCleanup]
        public void TestCleanup() {
            PakledTarget.Delete();
        }

        [TestMethod]
        public void CanFindNugetPackagesToPush() {
            var errorsAndInfos = new ErrorsAndInfos();
            var developerSettings = DeveloperSettings(errorsAndInfos);

            CloneTarget(errorsAndInfos);

            ChangeCakeScriptAndRunIt(true, errorsAndInfos);

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

            CloneTarget(errorsAndInfos);

            var packages = ComponentProvider.NugetFeedLister.ListReleasedPackages(developerSettings.NugetFeedUrl, @"Aspenlaub.Net.GitHub.CSharp." + PakledTarget.SolutionId);
            if (!packages.Any()) { return; }

            var latestPackageVersion = packages.Max(p => p.Version);
            var latestPackage = packages.First(p => p.Version == latestPackageVersion);

            var headTipIdSha = ComponentProvider.GitUtilities.HeadTipIdSha(PakledTarget.Folder());
            Assert.IsTrue(latestPackage.Tags.Contains(headTipIdSha));

            ChangeCakeScriptAndRunIt(false, errorsAndInfos);

            packages = ComponentProvider.NugetFeedLister.ListReleasedPackages(developerSettings.NugetFeedUrl, @"Aspenlaub.Net.GitHub.CSharp." + PakledTarget.SolutionId);
            Assert.AreEqual(latestPackageVersion, packages.Max(p => p.Version));
        }

        private static void CloneTarget(IErrorsAndInfos errorsAndInfos) {
            var gitUtilities = new GitUtilities();
            var url = "https://github.com/aspenlaub/" + PakledTarget.SolutionId + ".git";
            gitUtilities.Clone(url, PakledTarget.Folder(), new CloneOptions {BranchName = "master"}, true, errorsAndInfos);
            Assert.IsFalse(errorsAndInfos.Errors.Any(), string.Join("\r\n", errorsAndInfos.Errors));
        }

        private void ChangeCakeScriptAndRunIt(bool disableNugetPush, IErrorsAndInfos errorsAndInfos) {
            var cakeScriptFileFullName = PakledTarget.Folder().FullName + @"\build.cake";
            var cakeScript = File.ReadAllText(cakeScriptFileFullName);
            cakeScript = CakeBuildUtilities.UseLocalShatilayaAssemblies(cakeScript);
            File.WriteAllText(cakeScriptFileFullName, cakeScript);

            var target = disableNugetPush ? "IgnoreOutdatedBuildCakePendingChangesAndDoNotPush" : "IgnoreOutdatedBuildCakePendingChanges";
            PakledTarget.RunBuildCakeScript(ComponentProvider, target, errorsAndInfos);
            Assert.IsFalse(errorsAndInfos.Errors.Any(), string.Join("\r\n", errorsAndInfos.Errors));
        }
    }
}

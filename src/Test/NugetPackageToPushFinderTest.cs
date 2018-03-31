using System.Linq;
using Aspenlaub.Net.GitHub.CSharp.Shatilaya.Entities;
using Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces;
using LibGit2Sharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PeghComponentProvider = Aspenlaub.Net.GitHub.CSharp.Pegh.Components.ComponentProvider;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Test {
    [TestClass]
    public class NugetPackageToPushFinderTest {
        protected static TestTargetFolder PakledTarget = new TestTargetFolder(nameof(NugetPackageToPushFinderTest), "Pakled");
        protected IComponentProvider ComponentProvider;

        public NugetPackageToPushFinderTest() {
            var componentProviderMock = new Mock<IComponentProvider>();
            componentProviderMock.SetupGet(c => c.ProcessRunner).Returns(new ProcessRunner());
            componentProviderMock.SetupGet(c => c.PeghComponentProvider).Returns(new PeghComponentProvider());
            componentProviderMock.SetupGet(c => c.CakeRunner).Returns(new CakeRunner(componentProviderMock.Object));
            componentProviderMock.SetupGet(c => c.NugetConfigReader).Returns(new NugetConfigReader());
            componentProviderMock.SetupGet(c => c.NugetFeedLister).Returns(new NugetFeedLister());
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
            var gitUtilities = new GitUtilities();
            var errorsAndInfos = new ErrorsAndInfos();
            var url = "https://github.com/aspenlaub/" + PakledTarget.SolutionId + ".git";
            gitUtilities.Clone(url, PakledTarget.Folder(), new CloneOptions { BranchName = "master" }, errorsAndInfos);
            PakledTarget.RunBuildCakeScript(ComponentProvider, errorsAndInfos);
            Assert.AreEqual(3, errorsAndInfos.Errors.Count);
            Assert.IsTrue(errorsAndInfos.Errors.All(e => e.Contains("PushNuGet") || e.Contains("Not implemented yet") || e.Contains("error")));

            var developerSettingsSecret = new DeveloperSettingsSecret();
            var developerSettings = ComponentProvider.PeghComponentProvider.SecretRepository.Get(developerSettingsSecret);
            Assert.IsNotNull(developerSettings);

            errorsAndInfos = new ErrorsAndInfos();
            var sut = new NugetPackageToPushFinder(ComponentProvider);
            string packageFileFullName, feedUrl, apiKey;
            sut.FindPackageToPush(PakledTarget.Folder().ParentFolder().SubFolder(PakledTarget.SolutionId + @"Bin\Release"), PakledTarget.Folder().SubFolder("src").FullName + @"\" + PakledTarget.SolutionId + ".sln", out packageFileFullName, out feedUrl, out apiKey, errorsAndInfos);
            Assert.IsFalse(errorsAndInfos.Errors.Any(), string.Join("\r\n", errorsAndInfos.Errors));
            Assert.AreEqual(developerSettings.NugetFeedUrl, feedUrl);
            Assert.IsTrue(apiKey.Length > 256);
        }
    }
}

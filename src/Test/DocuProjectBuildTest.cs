using System.Linq;
using Aspenlaub.Net.GitHub.CSharp.PeghStandard.Entities;
using Aspenlaub.Net.GitHub.CSharp.Shatilaya.Extensions;
using Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces;
using LibGit2Sharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Test {
    [TestClass]
    public class DocuProjectBuildTest {
        protected static TestTargetFolder RoxannTarget = new TestTargetFolder(nameof(DocuProjectBuildTest), "Roxann");
        protected IComponentProvider ComponentProvider;

        public DocuProjectBuildTest() {
            var componentProviderMock = new Mock<IComponentProvider>();
            componentProviderMock.SetupGet(c => c.ProcessRunner).Returns(new ProcessRunner());
            componentProviderMock.SetupGet(c => c.CakeRunner).Returns(new CakeRunner(componentProviderMock.Object));
            ComponentProvider = componentProviderMock.Object;
        }

        [ClassInitialize]
        public static void ClassInitialize(TestContext context) {
            RoxannTarget.DeleteCakeFolder();
            RoxannTarget.CreateCakeFolder();
        }

        [ClassCleanup]
        public static void ClassCleanup() {
            RoxannTarget.DeleteCakeFolder();
        }

        [TestInitialize]
        public void Initialize() {
            RoxannTarget.Delete();
        }

        [TestCleanup]
        public void TestCleanup() {
            RoxannTarget.Delete();
        }

        [TestMethod]
        public void CanBuildProjectForDocumentation() {
            var gitUtilities = new GitUtilities();
            var errorsAndInfos = new ErrorsAndInfos();
            const string url = "https://github.com/aspenlaub/Roxann.git";
            gitUtilities.Clone(url, RoxannTarget.Folder(), new CloneOptions { BranchName = "master" }, true, errorsAndInfos);
            Assert.IsFalse(errorsAndInfos.Errors.Any(), errorsAndInfos.ErrorsPlusRelevantInfos());

            CakeBuildUtilities.CopyLatestScriptFromShatilayaSolution(RoxannTarget);

            RoxannTarget.RunBuildCakeScript(ComponentProvider, "IgnoreOutdatedBuildCakePendingChangesAndDoNotPush", errorsAndInfos);
            Assert.IsFalse(errorsAndInfos.Errors.Any(), errorsAndInfos.ErrorsPlusRelevantInfos());
        }
    }
}
using System.Linq;
using Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces;
using LibGit2Sharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Test {
    [TestClass]
    public class SelfBuildTest {
        protected static TestTargetFolder ShatilayaTarget = new TestTargetFolder(nameof(SelfBuildTest), "Shatilaya");
        protected IComponentProvider ComponentProvider;

        public SelfBuildTest() {
            var componentProviderMock = new Mock<IComponentProvider>();
            componentProviderMock.SetupGet(c => c.ProcessRunner).Returns(new ProcessRunner());
            componentProviderMock.SetupGet(c => c.CakeRunner).Returns(new CakeRunner(componentProviderMock.Object));
            ComponentProvider = componentProviderMock.Object;
        }

        [ClassInitialize]
        public static void ClassInitialize(TestContext context) {
            ShatilayaTarget.DeleteCakeFolder();
            ShatilayaTarget.CreateCakeFolder();
        }

        [ClassCleanup]
        public static void ClassCleanup() {
            ShatilayaTarget.DeleteCakeFolder();
        }

        [TestInitialize]
        public void Initialize() {
            ShatilayaTarget.Delete();
        }

        [TestCleanup]
        public void TestCleanup() {
            ShatilayaTarget.Delete();
        }

        [TestMethod, Ignore]
        public void CanBuildMyself() {
            GitTestUtilities.MakeSureGit2AssembliesAreInPlace();
            const string url = "https://github.com/aspenlaub/Shatilaya.git";
            Repository.Clone(url, ShatilayaTarget.FullName(), new CloneOptions { BranchName = "master" });
            var errorsAndInfos = new ErrorsAndInfos();
            ShatilayaTarget.RunBuildCakeScript(ComponentProvider, errorsAndInfos);
            Assert.IsFalse(errorsAndInfos.Errors.Any(), string.Join("\r\n", errorsAndInfos.Errors));
        }
    }
}
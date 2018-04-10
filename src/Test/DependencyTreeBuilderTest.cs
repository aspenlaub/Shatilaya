using System.IO;
using System.Linq;
using LibGit2Sharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Aspenlaub.Net.GitHub.CSharp.Pegh;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Test {
    [TestClass]
    public class DependencyTreeBuilderTest {
        protected static TestTargetFolder ShatilayaTarget = new TestTargetFolder(nameof(DependencyTreeBuilderTest), "Shatilaya");
        protected IComponentProvider ComponentProvider;

        public DependencyTreeBuilderTest() {
            var componentProviderMock = new Mock<IComponentProvider>();
            componentProviderMock.SetupGet(c => c.ProcessRunner).Returns(new ProcessRunner());
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

        [TestMethod]
        public void ThereArentAnyUnwantedDependencies() {
            var gitUtilities = new GitUtilities();
            var errorsAndInfos = new ErrorsAndInfos();
            const string url = "https://github.com/aspenlaub/Shatilaya.git";
            gitUtilities.Clone(url, ShatilayaTarget.Folder(), new CloneOptions { BranchName = "master" }, true, errorsAndInfos);
            Assert.IsFalse(errorsAndInfos.Errors.Any(), string.Join("\r\n", errorsAndInfos.Errors));
            var restorer = new NugetPackageRestorer(ComponentProvider);
            var sourceFolder = ShatilayaTarget.Folder().SubFolder("src").FullName;
            Directory.CreateDirectory(sourceFolder + @"\packages\");
            restorer.RestoreNugetPackages(sourceFolder + @"\" + ShatilayaTarget.SolutionId + ".sln", errorsAndInfos);
            Assert.IsFalse(errorsAndInfos.Errors.Any(), string.Join("\r\n", errorsAndInfos.Errors));
            Assert.IsTrue(errorsAndInfos.Infos.Any(i => i.Contains("package(s) to packages.config")));
            var builder = new DependencyTreeBuilder();
            var dependencyTree = builder.BuildDependencyTree(sourceFolder + @"\packages\");
            var nodes = dependencyTree.FindNodes(ContainsThreadingTasks);
            Assert.IsTrue(nodes.All(IsCorrectThreadingTasksVersion));
        }

        protected bool ContainsThreadingTasks(IDependencyNode node) {
            return !string.IsNullOrEmpty(node.Id) && node.Id.Contains("System.Threading.Tasks.Extensions");
        }

        protected bool IsCorrectThreadingTasksVersion(IDependencyNode node) {
            return node?.Version == "4.3.0";
        }
    }
}

using System.IO;
using System.Linq;
using LibGit2Sharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Test {
    [TestClass]
    public class DependencyTreeBuilderTest {
        protected static TestTargetFolder ShatilayaTarget = new TestTargetFolder(nameof(DependencyTreeBuilderTest), "Shatilaya");

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
            const string url = "https://github.com/aspenlaub/Shatilaya.git";
            Repository.Clone(url, ShatilayaTarget.FullName(), new CloneOptions { BranchName = "master" });
            var restorer = new NugetPackageRestorer();
            var sourceFolder = ShatilayaTarget.Folder().SubFolder("src").FullName;
            Directory.CreateDirectory(sourceFolder + @"\packages\");
            restorer.RestoreNugetPackages(sourceFolder + @"\" + ShatilayaTarget.SolutionId + ".sln");
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

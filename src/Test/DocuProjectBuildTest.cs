using System.Linq;
using System.Reflection;
using Aspenlaub.Net.GitHub.CSharp.Fusion;
using Aspenlaub.Net.GitHub.CSharp.Gitty;
using Aspenlaub.Net.GitHub.CSharp.Gitty.Extensions;
using Aspenlaub.Net.GitHub.CSharp.Gitty.TestUtilities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Autofac;
using LibGit2Sharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IContainer = Autofac.IContainer;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Test {
    [TestClass]
    public class DocuProjectBuildTest {
        protected static TestTargetFolder RoxannTarget = new TestTargetFolder(nameof(DocuProjectBuildTest), "Roxann");
        private static IContainer vContainer;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context) {
            vContainer = new ContainerBuilder().UseGittyTestUtilities().UseFusionNuclideProtchAndGitty().Build();
            vContainer.Resolve<TestTargetInstaller>().DeleteCakeFolder(RoxannTarget);
            vContainer.Resolve<TestTargetInstaller>().CreateCakeFolder(RoxannTarget, out var errorsAndInfos);
            Assert.IsFalse(errorsAndInfos.AnyErrors(), errorsAndInfos.ErrorsPlusRelevantInfos());
        }

        [ClassCleanup]
        public static void ClassCleanup() {
            vContainer.Resolve<TestTargetInstaller>().DeleteCakeFolder(RoxannTarget);
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

            vContainer.Resolve<CakeBuildUtilities>().CopyCakeScriptEmbeddedInAssembly(Assembly.GetExecutingAssembly(), BuildCake.Standard, RoxannTarget, errorsAndInfos);
            Assert.IsFalse(errorsAndInfos.Errors.Any(), errorsAndInfos.ErrorsPlusRelevantInfos());

            vContainer.Resolve<TestTargetRunner>().RunBuildCakeScript(BuildCake.Standard, RoxannTarget, "IgnoreOutdatedBuildCakePendingChangesAndDoNotPush", errorsAndInfos);
            Assert.IsFalse(errorsAndInfos.Errors.Any(), errorsAndInfos.ErrorsPlusRelevantInfos());
        }
    }
}
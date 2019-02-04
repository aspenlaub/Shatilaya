using System.IO;
using System.Linq;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Components;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Extensions;
using Aspenlaub.Net.GitHub.CSharp.Shatilaya.Extensions;
using Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces;
using LibGit2Sharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Test {
    [TestClass]
    public class GitPullTest {
        protected static TestTargetFolder ChabStandardTargetOne = new TestTargetFolder(nameof(GitPullTest), "ChabStandard");
        protected static TestTargetFolder ChabStandardTargetTwo = new TestTargetFolder(nameof(GitPullTest) + "Copy", "ChabStandard");
        protected IComponentProvider ComponentProvider;

        public GitPullTest() {
            var componentProviderMock = new Mock<IComponentProvider>();
            componentProviderMock.SetupGet(c => c.ProcessRunner).Returns(new ProcessRunner());
            componentProviderMock.SetupGet(c => c.CakeRunner).Returns(new CakeRunner(componentProviderMock.Object));
            ComponentProvider = componentProviderMock.Object;
        }

        [ClassInitialize]
        public static void ClassInitialize(TestContext context) {
            ChabStandardTargetTwo.DeleteCakeFolder();
            ChabStandardTargetTwo.CreateCakeFolder();
        }

        [ClassCleanup]
        public static void ClassCleanup() {
            ChabStandardTargetTwo.DeleteCakeFolder();
        }

        [TestInitialize]
        public void Initialize() {
            ChabStandardTargetOne.Delete();
        }

        [TestCleanup]
        public void TestCleanup() {
            ChabStandardTargetOne.Delete();
        }

        [TestMethod]
        public void LatestChangesArePulled() {
            var gitUtilities = new GitUtilities();
            var errorsAndInfos = new ErrorsAndInfos();
            var url = "https://github.com/aspenlaub/" + ChabStandardTargetOne.SolutionId + ".git";
            foreach (var target in new[] { ChabStandardTargetOne, ChabStandardTargetTwo }) {
                gitUtilities.Clone(url, target.Folder(), new CloneOptions { BranchName = "master" }, true, errorsAndInfos);
                ComponentProvider.CakeRunner.VerifyCakeVersion(target.Folder().SubFolder("tools"), errorsAndInfos);
                Assert.IsFalse(errorsAndInfos.Errors.Any(), errorsAndInfos.ErrorsPlusRelevantInfos());

                var addinsFolder = target.Folder().SubFolder("tools").SubFolder("Addins");
                if (!addinsFolder.Exists()) { continue; }

                var deleter = new FolderDeleter();
                deleter.DeleteFolder(addinsFolder);
            }

            // https://github.com/aspenlaub/ChabStandard/commit/b8c4dee904e5748fce9aba8f912c37cf13f87a7c came before
            // https://github.com/aspenlaub/ChabStandard/commit/c6eb57b5ad242222f3aa95d8a936bd08fcbab299 where package reference to Microsoft.NET.Test.Sdk was added
            gitUtilities.Reset(ChabStandardTargetOne.Folder(), "b8c4dee904e5748fce9aba8f912c37cf13f87a7c", errorsAndInfos);
            Assert.IsFalse(errorsAndInfos.Errors.Any(), errorsAndInfos.ErrorsPlusRelevantInfos());

            var projectFile = ChabStandardTargetOne.Folder().SubFolder("src").SubFolder("Test").FullName + '\\' + ChabStandardTargetOne.SolutionId + @".Test.csproj";
            Assert.IsFalse(File.ReadAllText(projectFile).Contains("<PackageReference Include=\"Microsoft.NET.Test.Sdk\""));

            CakeBuildUtilities.CopyLatestScriptFromShatilayaSolution(ChabStandardTargetTwo);
            var buildCakeScriptFileName = ChabStandardTargetTwo.FullName() + @"\" + "build.cake";
            var repositoryFolderSetStatement = "var repositoryFolder = MakeAbsolute(DirectoryPath.FromString(\"../../" + nameof(GitPullTest) + "/" + ChabStandardTargetOne.SolutionId + "\")).FullPath;";
            var buildCakeScript = File.ReadAllLines(buildCakeScriptFileName).Select(s => s.Contains("var repositoryFolder =") ?  repositoryFolderSetStatement : s);
            File.WriteAllLines(buildCakeScriptFileName, buildCakeScript);

            var solutionCakeFileFullName = ChabStandardTargetTwo.Folder().FullName + @"\solution.cake";
            var solutionCakeContents = File.ReadAllText(solutionCakeFileFullName);
            solutionCakeContents = solutionCakeContents.Replace(@"./src", @"../../" + nameof(GitPullTest) + @"/" + ChabStandardTargetOne.SolutionId + @"/src");
            File.WriteAllText(solutionCakeFileFullName, solutionCakeContents);

            ChabStandardTargetTwo.RunBuildCakeScript(ComponentProvider, "CleanRestorePull", errorsAndInfos);
            Assert.IsFalse(errorsAndInfos.Errors.Any(), errorsAndInfos.ErrorsPlusRelevantInfos());
            Assert.IsTrue(File.ReadAllText(projectFile).Contains("<PackageReference Include=\"Microsoft.NET.Test.Sdk\""));
        }
    }
}

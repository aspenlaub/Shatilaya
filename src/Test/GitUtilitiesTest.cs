using System.IO;
using System.Linq;
using Aspenlaub.Net.GitHub.CSharp.PeghStandard.Components;
using Aspenlaub.Net.GitHub.CSharp.PeghStandard.Entities;
using Aspenlaub.Net.GitHub.CSharp.PeghStandard.Extensions;
using Aspenlaub.Net.GitHub.CSharp.PeghStandard.Interfaces;
using LibGit2Sharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using IComponentProvider = Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces.IComponentProvider;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Test {
    [TestClass]
    public class GitUtilitiesTest {
        protected IFolder DevelopmentFolder, MasterFolder, NoGitFolder;
        protected static TestTargetFolder DoNotPullFolder = new TestTargetFolder(nameof(GitUtilitiesTest) + @"DoNotPull", "Pakled");
        protected IComponentProvider ComponentProvider;

        public GitUtilitiesTest() {
            var componentProviderMock = new Mock<IComponentProvider>();
            componentProviderMock.SetupGet(c => c.ProcessRunner).Returns(new ProcessRunner());
            componentProviderMock.SetupGet(c => c.CakeRunner).Returns(new CakeRunner(componentProviderMock.Object));
            ComponentProvider = componentProviderMock.Object;
        }

        [ClassInitialize]
        public static void ClassInitialize(TestContext context) {
            DoNotPullFolder.DeleteCakeFolder();
            DoNotPullFolder.CreateCakeFolder();
        }

        [ClassCleanup]
        public static void ClassCleanup() {
            DoNotPullFolder.DeleteCakeFolder();
        }

        [TestInitialize]
        public void Initialize() {
            var checkOutFolder = Path.GetTempPath() + nameof(GitUtilitiesTest) + '\\';
            DevelopmentFolder = new Folder(checkOutFolder + @"Pakled-Development");
            MasterFolder = new Folder(checkOutFolder + @"Pakled-Master");
            NoGitFolder = new Folder(checkOutFolder + @"NoGit");
            DoNotPullFolder.Delete();

            CleanUp();
            CloneRepository(MasterFolder, "master");
            CloneRepository(DevelopmentFolder, "development");
            if (!NoGitFolder.Exists()) {
                Directory.CreateDirectory(NoGitFolder.FullName);
            }
        }

        [TestCleanup]
        public void CleanUp() {
            var deleter = new FolderDeleter();
            foreach (var folder in new[] { DevelopmentFolder, MasterFolder, NoGitFolder }.Where(folder => folder.Exists())) {
                deleter.DeleteFolder(folder);
            }

            DoNotPullFolder.Delete();
        }

        private static void CloneRepository(IFolder folder, string branch) {
            if (folder.GitSubFolder().Exists()) {
                return;
            }

            if (folder.Exists()) {
                var deleter = new FolderDeleter();
                Assert.IsTrue(deleter.CanDeleteFolder(folder));
                deleter.DeleteFolder(folder);
            }

            const string url = "https://github.com/aspenlaub/Pakled.git";
            Repository.Clone(url, folder.FullName, new CloneOptions {BranchName = branch});
        }

        [TestMethod]
        public void CanIdentifyCheckedOutBranch() {
            var sut = new GitUtilities();
            Assert.AreEqual("development", sut.CheckedOutBranch(DevelopmentFolder));
            var developmentSubFolder = DevelopmentFolder.SubFolder(@"\Test\Properties");
            Assert.AreEqual("development", sut.CheckedOutBranch(developmentSubFolder));
            Assert.AreEqual("master", sut.CheckedOutBranch(MasterFolder));
            Assert.AreEqual("", sut.CheckedOutBranch(NoGitFolder));
        }

        [TestMethod]
        public void CanGetHeadTipIdSha() {
            var sut = new GitUtilities();
            var headTipIdSha = sut.HeadTipIdSha(MasterFolder);
            Assert.IsFalse(string.IsNullOrEmpty(headTipIdSha));
            Assert.IsTrue(headTipIdSha.Length >= 40);
        }

        [TestMethod]
        public void CanDetermineUncommittedChanges() {
            var sut = new GitUtilities();
            var errorsAndInfos = new ErrorsAndInfos();
            sut.VerifyThatThereAreNoUncommittedChanges(MasterFolder, errorsAndInfos);
            Assert.IsFalse(errorsAndInfos.AnyErrors(), string.Join("\r\n", errorsAndInfos.Errors));
            File.WriteAllText(MasterFolder.FullName + @"\change.cs", @"This is not a change");
            sut.VerifyThatThereAreNoUncommittedChanges(MasterFolder, errorsAndInfos);
            Assert.IsTrue(errorsAndInfos.Errors.Any(e => e.Contains(@"change.cs")));
        }

        [TestMethod]
        public void CanCheckIfIsBranchAheadOfMaster() {
            CloneRepository(DoNotPullFolder.Folder(), "do-not-pull-from-me");
            var sut = new GitUtilities();
            Assert.IsFalse(sut.IsBranchAheadOfMaster(MasterFolder));
            CakeBuildUtilities.CopyLatestScriptFromShatilayaSolution(DoNotPullFolder);
            var errorsAndInfos = new ErrorsAndInfos();
            DoNotPullFolder.RunBuildCakeScript(ComponentProvider, "CleanRestorePull", errorsAndInfos);
            Assert.IsFalse(errorsAndInfos.AnyErrors(), string.Join("\r\n", errorsAndInfos.Errors));
            Assert.IsTrue(sut.IsBranchAheadOfMaster(DoNotPullFolder.Folder()));
        }

        [TestMethod]
        public void CanIdentifyUrlOwnerAndName() {
            var sut = new GitUtilities();
            var errorsAndInfos = new ErrorsAndInfos();
            sut.IdentifyOwnerAndName(MasterFolder, out var owner, out var name, errorsAndInfos);
            Assert.IsFalse(errorsAndInfos.AnyErrors(), string.Join("\r\n", errorsAndInfos.Errors));
            Assert.AreEqual("aspenlaub", owner);
            Assert.AreEqual("Pakled", name);
        }
    }
}
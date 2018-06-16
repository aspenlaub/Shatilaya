using System.IO;
using System.Linq;
using System.Net;
using System.Net.Configuration;
using Aspenlaub.Net.GitHub.CSharp.Pegh;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Components;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using LibGit2Sharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using IComponentProvider = Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces.IComponentProvider;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Test {
    [TestClass]
    public class GitHubUtilitiesTest {
        protected IFolder MasterFolder, DevelopmentFolder;
        protected IComponentProvider ComponentProvider;

        public GitHubUtilitiesTest() {
            var componentProviderMock = new Mock<IComponentProvider>();
            componentProviderMock.SetupGet(c => c.ProcessRunner).Returns(new ProcessRunner());
            componentProviderMock.SetupGet(c => c.CakeRunner).Returns(new CakeRunner(componentProviderMock.Object));
            componentProviderMock.SetupGet(c => c.GitUtilities).Returns(new GitUtilities());
            componentProviderMock.SetupGet(c => c.PeghComponentProvider).Returns(new Pegh.Components.ComponentProvider());
            ComponentProvider = componentProviderMock.Object;
        }

        [TestInitialize]
        public void Initialize() {
            var checkOutFolder = Path.GetTempPath() + nameof(GitUtilitiesTest) + '\\';
            MasterFolder = new Folder(checkOutFolder + @"Pakled-Master");
            DevelopmentFolder = new Folder(checkOutFolder + @"Pakled-Development");

            CleanUp();
            CloneRepository(MasterFolder, "master");
            CloneRepository(DevelopmentFolder, "do-not-pull-from-me");
        }

        [TestCleanup]
        public void CleanUp() {
            var deleter = new FolderDeleter();
            foreach (var folder in new[] { MasterFolder, DevelopmentFolder }.Where(folder => folder.Exists())) {
                deleter.DeleteFolder(folder);
            }
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
            Repository.Clone(url, folder.FullName, new CloneOptions { BranchName = branch });
        }

        [TestMethod]
        public void CanCheckIfPullRequestsExist() {
            var sut = new GitHubUtilities(ComponentProvider);
            var errorsAndInfos = new ErrorsAndInfos();
            bool inconclusive;
            var hasOpenPullRequest = HasOpenPullRequest(sut, "", errorsAndInfos, out inconclusive);
            if (inconclusive) { return; }

            Assert.IsFalse(errorsAndInfos.AnyErrors(), string.Join("\r\n", errorsAndInfos.Errors));
            Assert.IsTrue(hasOpenPullRequest);

            hasOpenPullRequest = HasOpenPullRequest(sut, "2", errorsAndInfos, out inconclusive);
            if (inconclusive) { return; }

            Assert.IsFalse(errorsAndInfos.AnyErrors(), string.Join("\r\n", errorsAndInfos.Errors));
            Assert.IsFalse(hasOpenPullRequest);

            hasOpenPullRequest = HasOpenPullRequestForThisBranch(sut, true, errorsAndInfos, out inconclusive);
            if (inconclusive) { return; }

            Assert.IsFalse(errorsAndInfos.AnyErrors(), string.Join("\r\n", errorsAndInfos.Errors));
            Assert.IsFalse(hasOpenPullRequest);

            hasOpenPullRequest = HasOpenPullRequestForThisBranch(sut, false, errorsAndInfos, out inconclusive);
            if (inconclusive) { return; }

            Assert.IsFalse(errorsAndInfos.AnyErrors(), string.Join("\r\n", errorsAndInfos.Errors));
            Assert.IsTrue(hasOpenPullRequest);
        }

        protected bool HasOpenPullRequest(GitHubUtilities sut, string semicolonSeparatedListOfPullRequestNumbersToIgnore, ErrorsAndInfos errorsAndInfos, out bool inconclusive) {
            inconclusive = false;
            var hasOpenPullRequest = false;
            try {
                hasOpenPullRequest = sut.HasOpenPullRequest(MasterFolder, semicolonSeparatedListOfPullRequestNumbersToIgnore, errorsAndInfos);
            } catch (WebException) {
                inconclusive = true; // ToDo: use Assert.Inconclusive
            }
            return hasOpenPullRequest;
        }

        protected bool HasOpenPullRequestForThisBranch(GitHubUtilities sut, bool master, ErrorsAndInfos errorsAndInfos, out bool inconclusive) {
            inconclusive = false;
            var hasOpenPullRequest = false;
            try {
                hasOpenPullRequest = sut.HasOpenPullRequestForThisBranch(master ? MasterFolder : DevelopmentFolder, errorsAndInfos);
            } catch (WebException) {
                inconclusive = true; // ToDo: use Assert.Inconclusive
            }
            return hasOpenPullRequest;
        }

        [TestMethod]
        public void CanCheckHowManyPullRequestsExist() {
            var sut = new GitHubUtilities(ComponentProvider);
            var errorsAndInfos = new ErrorsAndInfos();
            var numberOfPullRequests = sut.NumberOfPullRequests(MasterFolder, errorsAndInfos);
            Assert.IsFalse(errorsAndInfos.AnyErrors(), string.Join("\r\n", errorsAndInfos.Errors));
            Assert.IsTrue(numberOfPullRequests > 1);
        }

        // HasOpenPullRequestForThisBranch(
    }
}

﻿using System.IO;
using System.Linq;
using System.Net;
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
        protected IFolder MasterFolder;
        protected IComponentProvider ComponentProvider;

        public GitHubUtilitiesTest() {
            var componentProviderMock = new Mock<IComponentProvider>();
            componentProviderMock.SetupGet(c => c.ProcessRunner).Returns(new ProcessRunner());
            componentProviderMock.SetupGet(c => c.CakeRunner).Returns(new CakeRunner(componentProviderMock.Object));
            componentProviderMock.SetupGet(c => c.GitUtilities).Returns(new GitUtilities());
            ComponentProvider = componentProviderMock.Object;
        }

        [TestInitialize]
        public void Initialize() {
            var checkOutFolder = Path.GetTempPath() + nameof(GitUtilitiesTest) + '\\';
            MasterFolder = new Folder(checkOutFolder + @"Pakled-Master");

            CleanUp();
            CloneRepository(MasterFolder, "master");
        }

        [TestCleanup]
        public void CleanUp() {
            var deleter = new FolderDeleter();
            foreach (var folder in new[] { MasterFolder }.Where(folder => folder.Exists())) {
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
            var hasOpenPullRequest = HasOpenPullRequest(sut, "", errorsAndInfos);
            Assert.IsFalse(errorsAndInfos.AnyErrors(), string.Join("\r\n", errorsAndInfos.Errors));
            Assert.IsTrue(hasOpenPullRequest);
            hasOpenPullRequest = HasOpenPullRequest(sut, "2", errorsAndInfos);
            Assert.IsFalse(errorsAndInfos.AnyErrors(), string.Join("\r\n", errorsAndInfos.Errors));
            Assert.IsFalse(hasOpenPullRequest);
        }

        protected bool HasOpenPullRequest(GitHubUtilities sut, string semicolonSeparatedListOfPullRequestNumbersToIgnore, ErrorsAndInfos errorsAndInfos) {
            var hasOpenPullRequest = false;
            try {
                hasOpenPullRequest = sut.HasOpenPullRequest(MasterFolder, semicolonSeparatedListOfPullRequestNumbersToIgnore, errorsAndInfos);
            } catch (WebException e) {
                Assert.Inconclusive(e.Message);
            }
            return hasOpenPullRequest;
        }

        [TestMethod]
        public void CanListPullRequests() {
            var sut = new GitHubUtilities(ComponentProvider);
            var errorsAndInfos = new ErrorsAndInfos();
            var numberOfPullRequests = sut.NumberOfPullRequests(MasterFolder, errorsAndInfos);
            Assert.IsFalse(errorsAndInfos.AnyErrors(), string.Join("\r\n", errorsAndInfos.Errors));
            Assert.IsTrue(numberOfPullRequests > 1);
        }
    }
}
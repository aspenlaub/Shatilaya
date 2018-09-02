using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
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
    public class GitHubUtilitiesTest {
        protected IFolder MasterFolder, DevelopmentFolder;
        protected IComponentProvider ComponentProvider;

        public GitHubUtilitiesTest() {
            var componentProviderMock = new Mock<IComponentProvider>();
            componentProviderMock.SetupGet(c => c.ProcessRunner).Returns(new ProcessRunner());
            componentProviderMock.SetupGet(c => c.CakeRunner).Returns(new CakeRunner(componentProviderMock.Object));
            componentProviderMock.SetupGet(c => c.GitUtilities).Returns(new GitUtilities());
            componentProviderMock.SetupGet(c => c.PeghComponentProvider).Returns(new PeghStandard.Components.ComponentProvider());
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
        public async Task CanCheckIfPullRequestsExist() {
            var sut = new GitHubUtilities(ComponentProvider);
            var errorsAndInfos = new ErrorsAndInfos();
            var hasOpenPullRequest = await HasOpenPullRequestAsync(sut, "", errorsAndInfos);
            if (hasOpenPullRequest.Inconclusive) { return; }

            Assert.IsFalse(errorsAndInfos.AnyErrors(), string.Join("\r\n", errorsAndInfos.Errors));
            Assert.IsTrue(hasOpenPullRequest.YesNo);

            hasOpenPullRequest = await HasOpenPullRequestAsync(sut, "2", errorsAndInfos);
            if (hasOpenPullRequest.Inconclusive) { return; }

            Assert.IsFalse(errorsAndInfos.AnyErrors(), string.Join("\r\n", errorsAndInfos.Errors));
            Assert.IsFalse(hasOpenPullRequest.YesNo);

            hasOpenPullRequest = await HasOpenPullRequestForThisBranchAsync(sut, true, errorsAndInfos);
            if (hasOpenPullRequest.Inconclusive) { return; }

            Assert.IsFalse(errorsAndInfos.AnyErrors(), string.Join("\r\n", errorsAndInfos.Errors));
            Assert.IsFalse(hasOpenPullRequest.YesNo);

            hasOpenPullRequest = await HasOpenPullRequestForThisBranchAsync(sut, false, errorsAndInfos);
            if (hasOpenPullRequest.Inconclusive) { return; }

            Assert.IsFalse(errorsAndInfos.AnyErrors(), string.Join("\r\n", errorsAndInfos.Errors));
            Assert.IsTrue(hasOpenPullRequest.YesNo);

            var hasPullRequest = await HasPullRequestForThisBranchAndItsHeadTipAsync(sut, errorsAndInfos);
            if (hasOpenPullRequest.Inconclusive) { return; }

            Assert.IsFalse(errorsAndInfos.AnyErrors(), string.Join("\r\n", errorsAndInfos.Errors));
            Assert.IsTrue(hasPullRequest.YesNo);
        }

        protected async Task<YesNoInconclusive> HasOpenPullRequestAsync(GitHubUtilities sut, string semicolonSeparatedListOfPullRequestNumbersToIgnore, ErrorsAndInfos errorsAndInfos) {
            var inconclusive = false;
            var hasOpenPullRequest = false;
            try {
                hasOpenPullRequest = await sut.HasOpenPullRequestAsync(MasterFolder, semicolonSeparatedListOfPullRequestNumbersToIgnore, errorsAndInfos);
            } catch (WebException) {
                inconclusive = true; // ToDo: use Assert.Inconclusive
            }

            return new YesNoInconclusive { YesNo = hasOpenPullRequest, Inconclusive = inconclusive };
        }

        protected async Task<YesNoInconclusive> HasOpenPullRequestForThisBranchAsync(GitHubUtilities sut, bool master, ErrorsAndInfos errorsAndInfos) {
            var inconclusive = false;
            var hasOpenPullRequest = false;
            try {
                hasOpenPullRequest = await sut.HasOpenPullRequestForThisBranchAsync(master ? MasterFolder : DevelopmentFolder, errorsAndInfos);
            } catch (WebException) {
                inconclusive = true; // ToDo: use Assert.Inconclusive
            }

            return new YesNoInconclusive { YesNo = hasOpenPullRequest, Inconclusive = inconclusive };
        }

        protected async Task<YesNoInconclusive> HasPullRequestForThisBranchAndItsHeadTipAsync(GitHubUtilities sut, ErrorsAndInfos errorsAndInfos) {
            var inconclusive = false;
            var hasOpenPullRequest = false;
            try {
                hasOpenPullRequest = await sut.HasPullRequestForThisBranchAndItsHeadTipAsync(DevelopmentFolder, errorsAndInfos);
            } catch (WebException) {
                inconclusive = true; // ToDo: use Assert.Inconclusive
            }

            return new YesNoInconclusive { YesNo = hasOpenPullRequest, Inconclusive = inconclusive };
        }

        [TestMethod]
        public async Task CanCheckHowManyPullRequestsExist() {
            var sut = new GitHubUtilities(ComponentProvider);
            var errorsAndInfos = new ErrorsAndInfos();
            try {
                var numberOfPullRequests = await sut.GetNumberOfPullRequestsAsync(MasterFolder, errorsAndInfos);
                Assert.IsFalse(errorsAndInfos.AnyErrors(), string.Join("\r\n", errorsAndInfos.Errors));
                Assert.IsTrue(numberOfPullRequests > 1);
            } catch (WebException) { // ToDo: use Assert.Inconclusive
            }
        }
    }

    public class YesNoInconclusive {
        public bool YesNo { get; set; }
        public bool Inconclusive { get; set; }
    }
}

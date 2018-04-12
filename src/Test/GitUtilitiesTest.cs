﻿using System.IO;
using System.Linq;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LibGit2Sharp;
using Aspenlaub.Net.GitHub.CSharp.Pegh;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Components;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Test {
    [TestClass]
    public class GitUtilitiesTest {
        protected IFolder DevelopmentFolder, MasterFolder, NoGitFolder;

        [TestInitialize]
        public void Initialize() {
            var checkOutFolder = Path.GetTempPath();
            DevelopmentFolder = new Folder(checkOutFolder + @"Pakled-Development");
            MasterFolder = new Folder(checkOutFolder + @"Pakled-Master");
            NoGitFolder = new Folder(checkOutFolder + @"NoGit");

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
            if (DevelopmentFolder.Exists()) {
                deleter.DeleteFolder(DevelopmentFolder);
            }
            if (MasterFolder.Exists()) {
                deleter.DeleteFolder(MasterFolder);
            }
            if (NoGitFolder.Exists()) {
                deleter.DeleteFolder(NoGitFolder);
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

        protected static void CheckThatWeAreOnline() {
            bool online;
            try {
                Dns.GetHostEntry("www.google.com");
                online = true;
            }
            catch {
                online = false;
            }

            Assert.IsTrue(online, "You are not connected to the internet");
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
    }
}
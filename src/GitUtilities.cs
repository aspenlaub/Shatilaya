﻿using System;
using System.IO;
using System.Linq;
using System.Net;
using ICSharpCode.SharpZipLib.Zip;
using LibGit2Sharp;
using Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Pegh;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya {
    public class GitUtilities : IGitUtilities {
        public string CheckedOutBranch(IFolder folder) {
            while (folder.Exists()) {
                if (!folder.GitSubFolder().Exists()) {
                    folder = folder.ParentFolder();
                    if (folder == null) { return ""; }

                    continue;
                }

                using (var repo = new Repository(folder.FullName, new RepositoryOptions())) {
                    return repo.Head.FriendlyName;
                }
            }

            return "";
        }

        public void SynchronizeRepository(IFolder folder) {
            if (!folder.GitSubFolder().Exists()) { return; }

            using (var repository = new Repository(folder.FullName)) {
                foreach (var remote in repository.Network.Remotes) {
                    var refSpecs = remote.FetchRefSpecs.Select(x => x.Specification);
                    Commands.Fetch(repository, remote.Name, refSpecs, null, "");
                }
            }

        }

        public void Clone(string url, IFolder folder, CloneOptions cloneOptions, IErrorsAndInfos errorsAndInfos) {
            MakeSureGit2AssembliesAreInPlace(errorsAndInfos);
            Repository.Clone(url, folder.FullName, cloneOptions);
        }

        /// <summary>
        /// When tests are executed using cake, the lib sub folder does not make it to the working directory,
        /// so the execution fails and reports that a git2 assembly cannot be found
        /// </summary>
        private static void MakeSureGit2AssembliesAreInPlace(IErrorsAndInfos errorsAndInfos) {
            var folder = Directory.GetCurrentDirectory();
            var downloadFolder = Path.GetTempPath() + @"\AspenlaubDownloads";
            if (!Directory.Exists(downloadFolder)) {
                Directory.CreateDirectory(downloadFolder);
            }
            var downloadedZipFileFullName = downloadFolder + @"\lib.zip";
            if (!File.Exists(downloadedZipFileFullName) || File.GetLastWriteTime(downloadedZipFileFullName).AddMinutes(10) < DateTime.Now) {
                using (var client = new WebClient()) {
                    client.DownloadFile("https://www.aspenlaub.net/Github/lib.zip", downloadedZipFileFullName);
                }
            }
            using (var zipStream = new FileStream(downloadedZipFileFullName, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                if (Directory.Exists(folder + @"\lib")) {

                    /* Folder exists => make sure the file names are consistent with the zip file provided on aspenlaub.net, otherwise the lib.zip needs to be updated there */
                    var zipFile = new ZipFile(zipStream);
                    // ReSharper disable once LoopCanBePartlyConvertedToQuery
                    foreach (var zipEntry in zipFile.Cast<ZipEntry>().Where(zipEntry => zipEntry.IsFile)) {
                        if (File.Exists(folder + @"\" + zipEntry.Name.Replace('/', '\\'))) { continue; }

                        errorsAndInfos.Errors.Add(string.Format(Properties.Resources.FileNotFound, zipEntry.Name));
                    }
                } else {

                    /* Folder exists => create it from the downloaded zip file */
                    var fastZip = new FastZip();
                    fastZip.ExtractZip(zipStream, folder, FastZip.Overwrite.Never, s => { return true; }, null, null, true, true);
                    if (Directory.Exists(folder + @"\lib")) { return; }

                    errorsAndInfos.Errors.Add(string.Format(Properties.Resources.FolderCouldNotBeCreated, folder));
                }
            }
        }

        public string HeadTipIdSha(IFolder repositoryFolder) {
            if (!repositoryFolder.Exists()) { return ""; }

            using (var repo = new Repository(repositoryFolder.FullName, new RepositoryOptions())) {
                return repo.Head.Tip.Id.Sha;
            }
        }

        public void VerifyThatThereAreNoUncommittedChanges(IFolder repositoryFolder, IErrorsAndInfos errorsAndInfos) {
            if (!repositoryFolder.Exists()) { return; }

            using (var repo = new Repository(repositoryFolder.FullName, new RepositoryOptions())) {
                var changes = repo.Diff.Compare<TreeChanges>(repo.Head.Tip.Tree, DiffTargets.Index | DiffTargets.WorkingDirectory).ToList();
                foreach(var change in changes) {
                    errorsAndInfos.Errors.Add(string.Format(Properties.Resources.UncommittedChangeTo, change.Path));
                }
            }
        }
    }
}

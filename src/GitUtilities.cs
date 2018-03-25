using System.IO;
using System.Linq;
using System.Reflection;
using Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces;
using ICSharpCode.SharpZipLib.Zip;
using LibGit2Sharp;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya {
    public class GitUtilities : IGitUtilities {
        public const string GitSubFolder = @"\.git";

        public string CheckedOutBranch(IFolder folder) {
            while (folder.Exists()) {
                if (!folder.HasSubFolder(GitSubFolder)) {
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
            if (!folder.HasSubFolder(GitSubFolder)) { return; }

            using (var repository = new Repository(folder.FullName)) {
                foreach (var remote in repository.Network.Remotes) {
                    var refSpecs = remote.FetchRefSpecs.Select(x => x.Specification);
                    Commands.Fetch(repository, remote.Name, refSpecs, null, "");
                }
            }

        }

        public void Clone(string url, IFolder folder, CloneOptions cloneOptions, ErrorsAndInfos errorsAndInfos) {
            MakeSureGit2AssembliesAreInPlace(errorsAndInfos);
            Repository.Clone(url, folder.FullName, cloneOptions);
        }

        /// <summary>
        /// When tests are executed using cake, the lib sub folder does not make it to the working directory,
        /// so the execution fails and reports that a git2 assembly cannot be found
        /// </summary>
        private static void MakeSureGit2AssembliesAreInPlace(ErrorsAndInfos errorsAndInfos) {
            var folder = Directory.GetCurrentDirectory();
            var zipStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Aspenlaub.Net.GitHub.CSharp.Shatilaya.lib.zip");
            if (Directory.Exists(folder + @"\lib")) {

                /* Folder exists => make sure the file names are consistent with the embedded zip file, otherwise the lib.zip resource needs to be updated */
                var zipFile = new ZipFile(zipStream);
                // ReSharper disable once LoopCanBePartlyConvertedToQuery
                foreach (var zipEntry in zipFile.Cast<ZipEntry>().Where(zipEntry => zipEntry.IsFile)) {
                    if (File.Exists(folder + @"\" + zipEntry.Name.Replace('/', '\\'))) { continue; }

                    errorsAndInfos.Errors.Add(string.Format(Properties.Resources.FileNotFound, zipEntry.Name));
                }
            } else {

                /* Folder exists => create it from the embedded zip file */
                var fastZip = new FastZip();
                fastZip.ExtractZip(zipStream, folder, FastZip.Overwrite.Never, s => { return true; }, null, null, true,
                    true);
                if (Directory.Exists(folder + @"\lib")) { return; }

                errorsAndInfos.Errors.Add(string.Format(Properties.Resources.FolderCouldNotBeCreated, folder));
            }
        }
    }
}

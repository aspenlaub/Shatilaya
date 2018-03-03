using System.Linq;
using Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces;
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
    }
}

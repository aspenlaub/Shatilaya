using LibGit2Sharp;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces {
    public interface IGitUtilities {
        /// <summary>
        /// Given a git check-out folder, what is the friendly branch name?
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        string CheckedOutBranch(IFolder folder);

        /// <summary>
        /// Synchronize the check-out folder (assuming there are no local changes)
        /// </summary>
        /// <param name="folder"></param>
        void SynchronizeRepository(IFolder folder);

        /// <summary>
        /// Cover of Repository.Clone
        /// </summary>
        /// <param name="url"></param>
        /// <param name="folder"></param>
        /// <param name="cloneOptions"></param>
        /// <param name="errorsAndInfos"></param>
        void Clone(string url, IFolder folder, CloneOptions cloneOptions, IErrorsAndInfos errorsAndInfos);

        /// <summary>
        /// Return the sha of the latest commit
        /// </summary>
        /// <param name="repositoryFolder"></param>
        /// <returns></returns>
        string HeadTipIdSha(IFolder repositoryFolder);

        /// <summary>
        /// Verify that the local repository does not contain uncommitted changes
        /// </summary>
        /// <param name="repositoryFolder"></param>
        /// <param name="errorsAndInfos"></param>
        /// <returns></returns>
        void VerifyThatThereAreNoUncommittedChanges(IFolder repositoryFolder, IErrorsAndInfos errorsAndInfos);
    }
}

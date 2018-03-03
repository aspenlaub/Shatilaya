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
    }
}

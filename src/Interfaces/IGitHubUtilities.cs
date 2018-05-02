using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces {
    public interface IGitHubUtilities {
        /// <summary>
        /// Check if checked out branch has an open pull request
        /// </summary>
        /// <param name="repositoryFolder"></param>
        /// <param name="errorsAndInfos"></param>
        /// <returns></returns>
        bool HasOpenPullRequest(IFolder repositoryFolder, IErrorsAndInfos errorsAndInfos);

        /// <summary>
        /// Return number of pull requests (open or closed)
        /// </summary>
        /// <param name="repositoryFolder"></param>
        /// <param name="errorsAndInfos"></param>
        /// <returns></returns>
        int NumberOfPullRequests(IFolder repositoryFolder, IErrorsAndInfos errorsAndInfos);
    }
}

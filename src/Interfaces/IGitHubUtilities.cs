using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces {
    public interface IGitHubUtilities {
        /// <summary>
        /// Check if there is an open pull request (not necessarily for the checked out branch)
        /// </summary>
        /// <param name="repositoryFolder"></param>
        /// <param name="errorsAndInfos"></param>
        /// <returns></returns>
        bool HasOpenPullRequest(IFolder repositoryFolder, IErrorsAndInfos errorsAndInfos);

        /// <summary>
        /// Check if there is an open pull request (not necessarily for the checked out branch)
        /// </summary>
        /// <param name="repositoryFolder"></param>
        /// <param name="semicolonSeparatedListOfPullRequestNumbersToIgnore"></param>
        /// <param name="errorsAndInfos"></param>
        /// <returns></returns>
        bool HasOpenPullRequest(IFolder repositoryFolder, string semicolonSeparatedListOfPullRequestNumbersToIgnore, IErrorsAndInfos errorsAndInfos);

        /// <summary>
        /// Return number of pull requests (open or closed)
        /// </summary>
        /// <param name="repositoryFolder"></param>
        /// <param name="errorsAndInfos"></param>
        /// <returns></returns>
        int NumberOfPullRequests(IFolder repositoryFolder, IErrorsAndInfos errorsAndInfos);

        /// <summary>
        /// Check if there is an open pull request for the checked out branch
        /// </summary>
        /// <param name="repositoryFolder"></param>
        /// <param name="errorsAndInfos"></param>
        /// <returns></returns>
        bool HasOpenPullRequestForThisBranch(IFolder repositoryFolder, IErrorsAndInfos errorsAndInfos);
    }
}

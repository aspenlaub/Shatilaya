using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
// ReSharper disable UnusedMember.Global

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces {
    public interface IGitHubUtilities {
        /// <summary>
        /// Check if there is an open pull request (not necessarily for the checked out branch)
        /// </summary>
        /// <param name="repositoryFolder"></param>
        /// <param name="errorsAndInfos"></param>
        /// <returns></returns>
        Task<bool> HasOpenPullRequestAsync(IFolder repositoryFolder, IErrorsAndInfos errorsAndInfos);

        /// <summary>
        /// Check if there is an open pull request (not necessarily for the checked out branch)
        /// </summary>
        /// <param name="repositoryFolder"></param>
        /// <param name="semicolonSeparatedListOfPullRequestNumbersToIgnore"></param>
        /// <param name="errorsAndInfos"></param>
        /// <returns></returns>
        Task<bool> HasOpenPullRequestAsync(IFolder repositoryFolder, string semicolonSeparatedListOfPullRequestNumbersToIgnore, IErrorsAndInfos errorsAndInfos);

        /// <summary>
        /// Return number of pull requests (open or closed)
        /// </summary>
        /// <param name="repositoryFolder"></param>
        /// <param name="errorsAndInfos"></param>
        /// <returns></returns>
        Task<int> GetNumberOfPullRequestsAsync(IFolder repositoryFolder, IErrorsAndInfos errorsAndInfos);

        /// <summary>
        /// Check if there is an open pull request for the checked out branch
        /// </summary>
        /// <param name="repositoryFolder"></param>
        /// <param name="errorsAndInfos"></param>
        /// <returns></returns>
        Task<bool> HasOpenPullRequestForThisBranchAsync(IFolder repositoryFolder, IErrorsAndInfos errorsAndInfos);

        /// <summary>
        /// Check if there is a pull request for the checked out branch with matching head tip id sha (not necessarily open)
        /// </summary>
        /// <param name="repositoryFolder"></param>
        /// <param name="errorsAndInfos"></param>
        /// <returns></returns>
        Task<bool> HasPullRequestForThisBranchAndItsHeadTipAsync(IFolder repositoryFolder, IErrorsAndInfos errorsAndInfos);
    }
}

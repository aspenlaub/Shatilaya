using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces;
using Octokit;
using Octokit.Reactive;
using IComponentProvider = Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces.IComponentProvider;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya {
    public class GitHubUtilities : IGitHubUtilities {
        protected IComponentProvider ComponentProvider;

        public GitHubUtilities(IComponentProvider componentProvider) {
            ComponentProvider = componentProvider;
        }

        public bool HasOpenPullRequest(IFolder repositoryFolder, IErrorsAndInfos errorsAndInfos) {
            var pullRequests = PullRequests(repositoryFolder, ItemStateFilter.Open, errorsAndInfos);
            return pullRequests.Any();
        }

        public int NumberOfPullRequests(IFolder repositoryFolder, IErrorsAndInfos errorsAndInfos) {
            return PullRequests(repositoryFolder, ItemStateFilter.All, errorsAndInfos).Count;
        }

        protected IList<PullRequest> PullRequests(IFolder repositoryFolder, ItemStateFilter itemStateFilter, IErrorsAndInfos errorsAndInfos) {
            var productInformation = new ProductHeaderValue(GetType().Namespace);
            var client = new ObservableGitHubClient(productInformation);
            string owner, name;
            ComponentProvider.GitUtilities.IdentifyOwnerAndName(repositoryFolder, out owner, out name, errorsAndInfos);
            var pullRequests = new List<PullRequest>();
            if (errorsAndInfos.AnyErrors()) { return pullRequests; }

            var requestRequest = new PullRequestRequest { State = itemStateFilter };
            var task = client.PullRequest.GetAllForRepository(owner, name, requestRequest);
            task.ForEachAsync(x => { pullRequests.Add(x); });
            task.Wait();
            return pullRequests;
        }
    }
}

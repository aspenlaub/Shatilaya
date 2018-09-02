﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.PeghStandard.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Shatilaya.Entities;
using Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using IComponentProvider = Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces.IComponentProvider;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya {
    public class GitHubUtilities : IGitHubUtilities {
        protected IComponentProvider ComponentProvider;

        public GitHubUtilities(IComponentProvider componentProvider) {
            ComponentProvider = componentProvider;
        }

        public async Task<bool> HasOpenPullRequestAsync(IFolder repositoryFolder, IErrorsAndInfos errorsAndInfos) {
            var pullRequests = await GetPullRequestsAsync(repositoryFolder, "open", errorsAndInfos);
            return pullRequests.Any();
        }

        public async Task<bool> HasOpenPullRequestAsync(IFolder repositoryFolder, string semicolonSeparatedListOfPullRequestNumbersToIgnore, IErrorsAndInfos errorsAndInfos) {
            var pullRequests = await GetPullRequestsAsync(repositoryFolder, "open", errorsAndInfos);
            return pullRequests.Any(p => !semicolonSeparatedListOfPullRequestNumbersToIgnore.Split(';').Contains(p.Number));
        }

        public async Task<bool> HasOpenPullRequestForThisBranchAsync(IFolder repositoryFolder, IErrorsAndInfos errorsAndInfos) {
            var pullRequests = await GetPullRequestsAsync(repositoryFolder, "open", errorsAndInfos);
            var checkedOutBranch = ComponentProvider.GitUtilities.CheckedOutBranch(repositoryFolder);
            return pullRequests.Any(p => p.Branch == checkedOutBranch);
        }

        public async Task<int> GetNumberOfPullRequestsAsync(IFolder repositoryFolder, IErrorsAndInfos errorsAndInfos) {
            return (await GetPullRequestsAsync(repositoryFolder, "all", errorsAndInfos)).Count;
        }


        public async Task<bool> HasPullRequestForThisBranchAndItsHeadTipAsync(IFolder repositoryFolder, IErrorsAndInfos errorsAndInfos) {
            var pullRequests = await GetPullRequestsAsync(repositoryFolder, "all", errorsAndInfos);
            var checkedOutBranch = ComponentProvider.GitUtilities.CheckedOutBranch(repositoryFolder);
            var headTipIdSha = ComponentProvider.GitUtilities.HeadTipIdSha(repositoryFolder);
            return pullRequests.Any(p => p.Branch == checkedOutBranch && p.Sha == headTipIdSha);
        }

        protected async Task<IList<PullRequest>> GetPullRequestsAsync(IFolder repositoryFolder, string state, IErrorsAndInfos errorsAndInfos) {
            var pullRequests = new List<PullRequest>();
            ComponentProvider.GitUtilities.IdentifyOwnerAndName(repositoryFolder, out var owner, out var name, errorsAndInfos);
            if (errorsAndInfos.AnyErrors()) { return pullRequests; }


            var url = $"https://api.github.com/repos/{owner}/{name}/pulls?state=" + state;
            var result = await RunJsonWebRequestAsync(url, owner, errorsAndInfos) as JArray;
            if (errorsAndInfos.AnyErrors()) { return pullRequests; }

            if (result == null) {
                errorsAndInfos.Errors.Add(Texts.CouldNotGetListOfPullRequests);
                return pullRequests;
            }

            pullRequests.AddRange(result.Select(detailResult => CreatePullRequest(detailResult)));

            return pullRequests;
        }

        protected async Task<object> RunJsonWebRequestAsync(string url, string owner, IErrorsAndInfos errorsAndInfos) {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = WebRequestMethods.Http.Get;
            request.UserAgent = GetType().Namespace;
            var personalAccessTokens = await GetPersonalAccessTokensAsync(errorsAndInfos);
            if (errorsAndInfos.AnyErrors()) {
                return null;
            }

            var personalAccessToken = personalAccessTokens.FirstOrDefault(p => p.Owner == owner && p.TokenName == "CakeBuild");
            if (personalAccessToken != null) {
                request.Headers.Add("Authorization", "token " + personalAccessToken.Token);
            }
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var response = (HttpWebResponse)request.GetResponse();
            var responseStream = response.GetResponseStream();
            if (responseStream == null) {
                errorsAndInfos.Errors.Add(Texts.CouldNotGetListOfPullRequests);
                return null;
            }

            string text;
            using (var sr = new StreamReader(responseStream)) {
                text = sr.ReadToEnd();
            }

            return JsonConvert.DeserializeObject(text);
        }

        protected static PullRequest CreatePullRequest(JToken jToken) {
            return new PullRequest { Id = jToken["id"].Value<string>(), Number = jToken["number"].Value<string>(), State = jToken["state"].Value<string>(), Branch = jToken["head"]["ref"].Value<string>(), Sha = jToken["head"]["sha"].Value<string>() };
        }

        private async Task<PersonalAccessTokens> GetPersonalAccessTokensAsync(IErrorsAndInfos errorsAndInfos) {
            var personalAccessTokensSecret = new PersonalAccessTokensSecret();
            var personalAccessTokens = await ComponentProvider.PeghComponentProvider.SecretRepository.GetAsync(personalAccessTokensSecret, errorsAndInfos);
            return personalAccessTokens;
        }
    }
}
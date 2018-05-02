using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
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

        public bool HasOpenPullRequest(IFolder repositoryFolder, IErrorsAndInfos errorsAndInfos) {
            var pullRequests = PullRequests(repositoryFolder, "open", errorsAndInfos);
            return pullRequests.Any();
        }

        public int NumberOfPullRequests(IFolder repositoryFolder, IErrorsAndInfos errorsAndInfos) {
            return PullRequests(repositoryFolder, "all", errorsAndInfos).Count;
        }

        protected IList<PullRequest> PullRequests(IFolder repositoryFolder, string state, IErrorsAndInfos errorsAndInfos) {
            var pullRequests = new List<PullRequest>();
            string owner, name;
            ComponentProvider.GitUtilities.IdentifyOwnerAndName(repositoryFolder, out owner, out name, errorsAndInfos);
            if (errorsAndInfos.AnyErrors()) { return pullRequests; }


            var url = $"https://api.github.com/repos/{owner}/{name}/pulls?state=" + state;
            var result = RunJsonWebRequest(url, errorsAndInfos) as JArray;
            if (errorsAndInfos.AnyErrors()) { return pullRequests; }

            if (result == null) {
                errorsAndInfos.Errors.Add(Properties.Resources.CouldNotGetListOfPullRequests);
                return pullRequests;
            }

            pullRequests.AddRange(result.Select(detailResult => CreatePullRequest(detailResult)));

            return pullRequests;
        }

        protected object RunJsonWebRequest(string url, IErrorsAndInfos errorsAndInfos) {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = WebRequestMethods.Http.Get;
            request.UserAgent = GetType().Namespace;
            var response = (HttpWebResponse)request.GetResponse();
            var responseStream = response.GetResponseStream();
            if (responseStream == null) {
                errorsAndInfos.Errors.Add(Properties.Resources.CouldNotGetListOfPullRequests);
                return null;
            }

            string text;
            using (var sr = new StreamReader(responseStream)) {
                text = sr.ReadToEnd();
            }

            return JsonConvert.DeserializeObject(text);
        }

        protected static PullRequest CreatePullRequest(JToken jToken) {
            return new PullRequest { Id = jToken["id"].Value<string>(), Number = jToken["number"].Value<string>(), State = jToken["state"].Value<string>() };
        }
    }
}
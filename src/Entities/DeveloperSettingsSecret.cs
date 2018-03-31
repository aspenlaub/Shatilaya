using System;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Entities {
    public class DeveloperSettingsSecret : ISecret<DeveloperSettings> {
        private DeveloperSettings vDeveloperSettings;
        public DeveloperSettings DefaultValue {
            get {
                return vDeveloperSettings ?? (vDeveloperSettings = new DeveloperSettings { Author = Environment.UserName, GitHubRepositoryUrl = "https://github.com/" + Guid,
                    FaviconUrl = "https://www." + Guid + ".net/favicon.ico", NugetFeedUrl = "https://www." + Guid + "nuget" });
            }
        }

        public string Guid { get { return "C9CA6C10-7409-487F-B406-A9EF9AD835A5"; } }
    }
}

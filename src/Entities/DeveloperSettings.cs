﻿using System.Xml.Serialization;
using Aspenlaub.Net.GitHub.CSharp.PeghStandard.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Entities {
    [XmlRoot("DeveloperSettings", Namespace = "http://www.aspenlaub.net")]
    public class DeveloperSettings : ISecretResult<DeveloperSettings> {
        public string Author { get; set; }
        public string Email { get; set; }
        public string GitHubRepositoryUrl { get; set; }
        public string FaviconUrl { get; set; }
        public string NugetFeedUrl { get; set; }
        public string NugetFeedId { get; set; }

        public DeveloperSettings Clone() {
            return new DeveloperSettings {
                Author = Author,
                Email = Email,
                GitHubRepositoryUrl = GitHubRepositoryUrl,
                FaviconUrl = FaviconUrl,
                NugetFeedUrl = NugetFeedUrl,
                NugetFeedId = NugetFeedId
            };
        }
    }
}

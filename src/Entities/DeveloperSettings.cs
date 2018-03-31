﻿using System.Xml.Serialization;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Entities {
    [XmlRoot("DeveloperSettings", Namespace = "http://www.aspenlaub.net")]
    public class DeveloperSettings : ISecretResult<DeveloperSettings> {
        public string Author { get; set; }
        public string GitHubRepositoryUrl { get; set; }
        public string FaviconUrl { get; set; }
        public string NugetFeedUrl { get; set; }

        public DeveloperSettings Clone() {
            return new DeveloperSettings {
                Author = Author,
                GitHubRepositoryUrl = GitHubRepositoryUrl,
                FaviconUrl = FaviconUrl,
                NugetFeedUrl = NugetFeedUrl
            };
        }
    }
}

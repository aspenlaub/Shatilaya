using Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Entities {
    public class PackageToPush : IPackageToPush {
        public string PackageFileFullName { get; set; }
        public string FeedUrl { get; set; }
        public string ApiKey { get; set; }
    }
}

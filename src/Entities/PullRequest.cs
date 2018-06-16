using Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Entities {
    public class PullRequest : IPullRequest {
        public string Id { get; set; }
        public string Number { get; set; }
        public string State { get; set; }
        public string Branch { get; set; }
    }
}

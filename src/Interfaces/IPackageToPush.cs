namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces {
    public interface IPackageToPush {
        string PackageFileFullName { get; set; }
        string FeedUrl { get; set; }
        string ApiKey { get; set; }
    }
}
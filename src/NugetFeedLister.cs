using System.Collections.Generic;
using System.Linq;
using Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces;
using NuGet;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya {
    public class NugetFeedLister : INugetFeedLister {
        public IList<IPackage> ListReleasedPackages(string feedUrl, string packageId) {
            var repo = PackageRepositoryFactory.Default.CreateRepository(feedUrl);
            var packages = repo.FindPackagesById(packageId).ToList();
            packages = packages.Where(item => item.IsReleaseVersion()).ToList();
            return packages;
        }
    }
}

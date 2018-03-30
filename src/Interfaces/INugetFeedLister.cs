using System.Collections.Generic;
using NuGet;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces {
    public interface INugetFeedLister {

        /// <summary>
        /// See what packages are provided by the specified nuget feed
        /// </summary>
        /// <param name="feedUrl">e.g. https://packages.nuget.org/api/v2</param>
        /// <param name="packageId">e.g. Aspenlaub.Net.GitHub.CSharp.Shatilaya</param>
        /// <returns></returns>
        IList<IPackage> ListReleasedPackages(string feedUrl, string packageId);
    }
}

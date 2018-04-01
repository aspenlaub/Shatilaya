﻿using IPeghComponentProvider = Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces.IComponentProvider;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces {
    public interface IComponentProvider {
        ICakeRunner CakeRunner { get; }
        IDependencyTreeBuilder DependencyTreeBuilder { get; }
        IGitUtilities GitUtilities { get; }
        ILatestBuildCakeScriptProvider LatestBuildCakeScriptProvider { get; }
        INugetConfigReader NugetConfigReader { get; }
        INugetFeedLister NugetFeedLister { get; }
        INugetPackageInstaller NugetPackageInstaller { get; }
        INugetPackageRestorer NugetPackageRestorer { get; }
        INugetPackageToPushFinder NugetPackageToPushFinder { get; }
        INuSpecCreator NuSpecCreator { get; }
        IObsoletePackageFinder ObsoletePackageFinder { get; }
        IPackageConfigsScanner PackageConfigsScanner { get; }
        IPeghComponentProvider PeghComponentProvider { get; }
        IProcessRunner ProcessRunner { get; }
        IProjectFactory ProjectFactory { get; }
    }
}

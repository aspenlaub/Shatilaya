﻿namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces {
    public interface IComponentProvider {
        ICakeRunner CakeRunner { get; }
        IDependencyTreeBuilder DependencyTreeBuilder { get; }
        IFolderDeleter FolderDeleter { get; }
        IFolderUpdater FolderUpdater { get; }
        IGitUtilities GitUtilities { get; }
        ILatestBuildCakeScriptProvider LatestBuildCakeScriptProvider { get; }
        INugetPackageRestorer NugetPackageRestorer { get; }
        INuSpecCreator NuSpecCreator { get; }
        IObsoletePackageFinder ObsoletePackageFinder { get; }
        IPackageConfigsScanner PackageConfigsScanner { get; }
    }
}

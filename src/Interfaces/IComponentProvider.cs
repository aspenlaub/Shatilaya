using IPeghComponentProvider = Aspenlaub.Net.GitHub.CSharp.PeghStandard.Interfaces.IComponentProvider;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces {
    public interface IComponentProvider {
        // ReSharper disable once UnusedMember.Global
        ICakeInstaller CakeInstaller { get; }
        ICakeRunner CakeRunner { get; }
        IDependencyTreeBuilder DependencyTreeBuilder { get; }
        IExecutableFinder ExecutableFinder { get; }
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
        IProjectLogic ProjectLogic { get; }
        IToolsVersionFinder ToolsVersionFinder { get; }
    }
}

using System;
using System.Collections.Generic;
using Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces;
using IPeghComponentProvider = Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces.IComponentProvider;
using PeghComponentProvider = Aspenlaub.Net.GitHub.CSharp.Pegh.Components.ComponentProvider;
// ReSharper disable UnusedMember.Global

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya {
    public class ComponentProvider : IComponentProvider {
        private Dictionary<Type, object> DefaultComponents { get; }

        public ComponentProvider() {
            DefaultComponents = new Dictionary<Type, object>();
        }

        private T DefaultComponent<T, T2>() where T : class where T2 : T, new() {
            if (!DefaultComponents.ContainsKey(typeof(T))) {
                DefaultComponents[typeof(T)] = new T2();
            }
            return (T)DefaultComponents[typeof(T)];
        }

        private T DefaultComponent<T, T2>(Func<T2> constructor) where T : class where T2 : T {
            if (!DefaultComponents.ContainsKey(typeof(T))) {
                DefaultComponents[typeof(T)] = constructor();
            }
            return (T)DefaultComponents[typeof(T)];
        }

        public ICakeInstaller CakeInstaller => DefaultComponent<ICakeInstaller, CakeInstaller>();
        public ICakeRunner CakeRunner { get { return DefaultComponent<ICakeRunner, CakeRunner>(() => new CakeRunner(this)); } }
        public IDependencyTreeBuilder DependencyTreeBuilder => DefaultComponent<IDependencyTreeBuilder, DependencyTreeBuilder>();
        public IGitUtilities GitUtilities => DefaultComponent<IGitUtilities, GitUtilities>();
        public IGitHubUtilities GitHubUtilities { get { return DefaultComponent<IGitHubUtilities, GitHubUtilities>(() => new GitHubUtilities(this)); } }
        public ILatestBuildCakeScriptProvider LatestBuildCakeScriptProvider => DefaultComponent<ILatestBuildCakeScriptProvider, LatestBuildCakeScriptProvider>();
        public INugetConfigReader NugetConfigReader => DefaultComponent<INugetConfigReader, NugetConfigReader>();
        public INugetFeedLister NugetFeedLister => DefaultComponent<INugetFeedLister, NugetFeedLister>();
        public INugetPackageInstaller NugetPackageInstaller { get { return DefaultComponent<INugetPackageInstaller, NugetPackageInstaller>(() => new NugetPackageInstaller(this)); } }
        public INugetPackageRestorer NugetPackageRestorer { get { return DefaultComponent<INugetPackageRestorer, NugetPackageRestorer>(() => new NugetPackageRestorer(this)); } }
        public INugetPackageToPushFinder NugetPackageToPushFinder { get { return DefaultComponent<INugetPackageToPushFinder, NugetPackageToPushFinder>(() => new NugetPackageToPushFinder(this)); } }
        public INuSpecCreator NuSpecCreator { get { return DefaultComponent<INuSpecCreator, NuSpecCreator>(() => new NuSpecCreator(this)); } }
        public IObsoletePackageFinder ObsoletePackageFinder { get { return DefaultComponent<IObsoletePackageFinder, ObsoletePackageFinder>(() => new ObsoletePackageFinder(this)); } }
        public IPackageConfigsScanner PackageConfigsScanner => DefaultComponent<IPackageConfigsScanner, PackageConfigsScanner>();
        public IPeghComponentProvider PeghComponentProvider => DefaultComponent<IPeghComponentProvider, PeghComponentProvider>();
        public IProcessRunner ProcessRunner => DefaultComponent<IProcessRunner, ProcessRunner>();
        public IProjectFactory ProjectFactory => DefaultComponent<IProjectFactory, ProjectFactory>();
        public IProjectLogic ProjectLogic => DefaultComponent<IProjectLogic, ProjectLogic>();
    }
}

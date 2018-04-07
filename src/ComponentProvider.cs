using System;
using System.Collections.Generic;
using Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces;
using IPeghComponentProvider = Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces.IComponentProvider;
using PeghComponentProvider = Aspenlaub.Net.GitHub.CSharp.Pegh.Components.ComponentProvider;

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

        public ICakeRunner CakeRunner { get { return DefaultComponent<ICakeRunner, CakeRunner>(() => new CakeRunner(this)); } }
        public IDependencyTreeBuilder DependencyTreeBuilder { get { return DefaultComponent<IDependencyTreeBuilder, DependencyTreeBuilder>(); } }
        public IExecutableFinder ExecutableFinder { get { return DefaultComponent<IExecutableFinder, ExecutableFinder>(); } }
        public IGitUtilities GitUtilities { get { return DefaultComponent<IGitUtilities, GitUtilities>(); } }
        public ILatestBuildCakeScriptProvider LatestBuildCakeScriptProvider { get { return DefaultComponent<ILatestBuildCakeScriptProvider, LatestBuildCakeScriptProvider>(); } }
        public INugetConfigReader NugetConfigReader { get { return DefaultComponent<INugetConfigReader, NugetConfigReader>(); } }
        public INugetFeedLister NugetFeedLister { get { return DefaultComponent<INugetFeedLister, NugetFeedLister>(); } }
        public INugetPackageInstaller NugetPackageInstaller { get { return DefaultComponent<INugetPackageInstaller, NugetPackageInstaller>(() => new NugetPackageInstaller(this)); } }
        public INugetPackageRestorer NugetPackageRestorer { get { return DefaultComponent<INugetPackageRestorer, NugetPackageRestorer>(() => new NugetPackageRestorer(this)); } }
        public INugetPackageToPushFinder NugetPackageToPushFinder { get { return DefaultComponent<INugetPackageToPushFinder, NugetPackageToPushFinder>(() => new NugetPackageToPushFinder(this)); } }
        public INuSpecCreator NuSpecCreator { get { return DefaultComponent<INuSpecCreator, NuSpecCreator>(() => new NuSpecCreator(this)); } }
        public IObsoletePackageFinder ObsoletePackageFinder { get { return DefaultComponent<IObsoletePackageFinder, ObsoletePackageFinder>(() => new ObsoletePackageFinder(this)); } }
        public IPackageConfigsScanner PackageConfigsScanner { get { return DefaultComponent<IPackageConfigsScanner, PackageConfigsScanner>(); } }
        public IPeghComponentProvider PeghComponentProvider { get { return DefaultComponent<IPeghComponentProvider, PeghComponentProvider>(); } }
        public IProcessRunner ProcessRunner { get { return DefaultComponent<IProcessRunner, ProcessRunner>(); } }
        public IProjectFactory ProjectFactory { get { return DefaultComponent<IProjectFactory, ProjectFactory>(); } }
    }
}

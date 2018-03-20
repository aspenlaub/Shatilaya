using System.Collections.Generic;
using Aspenlaub.Net.GitHub.CSharp.Shatilaya.Entities;
using Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces;
using NuGet;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya {
    public class DependencyTreeBuilder : IDependencyTreeBuilder {
        public IDependencyNode BuildDependencyTree(string packagesFolder) {
            var repository = new LocalPackageRepository(packagesFolder);
            var packages = repository.GetPackages();
            return BuildDependencyTree(repository, packages);
        }

        protected IDependencyNode BuildDependencyTree(LocalPackageRepository repository, IEnumerable<IPackage> packages) {
            var tree = new DependencyNode();
            foreach (var package in packages) {
                tree.Id = package.Id;
                tree.Version = package.Version.ToString();

                IList<IPackage> dependentPackages = new List<IPackage>();
                // ReSharper disable once LoopCanBeConvertedToQuery
                foreach (var dependencySet in package.DependencySets) {
                    // ReSharper disable once LoopCanBePartlyConvertedToQuery
                    foreach (var dependency in dependencySet.Dependencies) {
                        var dependentPackage = repository.FindPackage(dependency.Id, new SemanticVersion(dependency.VersionSpec.ToString().Replace("[", "").Replace("]", "")));
                        if (dependentPackage == null) {
                            tree.ChildNodes.Add(new DependencyNode { Id = dependency.Id, Version = dependency.VersionSpec.ToString() });
                        } else {
                            dependentPackages.Add(dependentPackage);
                        }
                    }
                }

                tree.ChildNodes.Add(BuildDependencyTree(repository, dependentPackages));
            }

            return tree;
        }
    }
}

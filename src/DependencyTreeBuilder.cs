using System.Collections.Generic;
using System.Linq;
using Aspenlaub.Net.GitHub.CSharp.Shatilaya.Entities;
using Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces;
using NuGet;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya {
    public class DependencyTreeBuilder : IDependencyTreeBuilder {
        public IDependencyNode BuildDependencyTree(string packagesFolder) {
            var repository = new LocalPackageRepository(packagesFolder);
            var packages = repository.GetPackages();
            return BuildDependencyTree(repository, packages, new List<DependencyNode>());
        }

        protected IDependencyNode BuildDependencyTree(LocalPackageRepository repository, IEnumerable<IPackage> packages, IList<DependencyNode> ignoreNodes) {
            var tree = new DependencyNode();
            foreach (var package in packages) {
                tree.Id = package.Id;
                tree.Version = package.Version.ToString();
                if (ignoreNodes.Any(n => EqualNodes(n, tree))) {
                    continue;
                }

                IList<IPackage> dependentPackages = new List<IPackage>();
                // ReSharper disable once LoopCanBeConvertedToQuery
                foreach (var dependencySet in package.DependencySets) {
                    // ReSharper disable once LoopCanBePartlyConvertedToQuery
                    foreach (var dependency in dependencySet.Dependencies) {
                        var dependentPackage = repository.FindPackage(dependency.Id, new SemanticVersion(dependency.VersionSpec.ToString().Replace("[", "").Replace("]", "")));
                        var dependencyNode = new DependencyNode { Id = dependency.Id, Version = dependency.VersionSpec.ToString() };
                        if (ignoreNodes.Any(n => EqualNodes(n, dependencyNode))) {
                            continue;
                        }

                        ignoreNodes.Add(dependencyNode);
                        if (dependentPackage == null) {
                            tree.ChildNodes.Add(dependencyNode);
                        } else {
                            dependentPackages.Add(dependentPackage);
                        }
                    }
                }

                tree.ChildNodes.Add(BuildDependencyTree(repository, dependentPackages, ignoreNodes));
            }

            return tree;
        }

        protected static bool EqualNodes(DependencyNode node1, DependencyNode node2) {
            return node1.Id == node2.Id && node1.Version == node2.Version;
        }
    }
}

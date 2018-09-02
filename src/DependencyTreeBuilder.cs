﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Aspenlaub.Net.GitHub.CSharp.Shatilaya.Entities;
using Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces;
using NuGet.Common;
using NuGet.Protocol;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya {
    public class DependencyTreeBuilder : IDependencyTreeBuilder {
        public IDependencyNode BuildDependencyTree(string packagesFolder) {
            var logger = new NullLogger();
            var repository = new FindLocalPackagesResourceV2(packagesFolder);
            var packages = repository.GetPackages(logger, CancellationToken.None);
            return BuildDependencyTree(repository, packages, new List<DependencyNode>());
        }

        protected IDependencyNode BuildDependencyTree(FindLocalPackagesResource repository, IEnumerable<LocalPackageInfo> packages, IList<DependencyNode> ignoreNodes) {
            var logger = new NullLogger();
            var tree = new DependencyNode();
            foreach (var package in packages) {
                tree.Id = package.Identity.Id;
                tree.Version = package.Identity.Version.ToString();
                if (ignoreNodes.Any(n => EqualNodes(n, tree))) {
                    continue;
                }

                IList<LocalPackageInfo> dependentPackages = new List<LocalPackageInfo>();
                // ReSharper disable once LoopCanBeConvertedToQuery
                foreach (var dependencySet in package.Nuspec.GetDependencyGroups()) {
                    // ReSharper disable once LoopCanBePartlyConvertedToQuery
                    foreach (var dependentPackage in dependencySet.Packages.SelectMany(d => repository.FindPackagesById(d.Id, logger, CancellationToken.None))) {
                        var dependencyNode = new DependencyNode { Id = dependentPackage.Identity.Id, Version = dependentPackage.Identity.Version.Version.ToString() };
                        if (ignoreNodes.Any(n => EqualNodes(n, dependencyNode))) {
                            continue;
                        }

                        ignoreNodes.Add(dependencyNode);
                        dependentPackages.Add(dependentPackage);
                    }
                }

                if (!dependentPackages.Any()) { continue; }

                tree.ChildNodes.Add(BuildDependencyTree(repository, dependentPackages, ignoreNodes));
            }

            return tree;
        }

        protected static bool EqualNodes(DependencyNode node1, DependencyNode node2) {
            return node1.Id == node2.Id && node1.Version == node2.Version;
        }
    }
}

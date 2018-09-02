﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using Aspenlaub.Net.GitHub.CSharp.PeghStandard.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya {
    public class PackageConfigsScanner : IPackageConfigsScanner {
        public IDictionary<string, string> DependencyIdsAndVersions(string projectFolder, bool includeTest, IErrorsAndInfos errorsAndInfos) {
            var dependencyIdsAndVersions = new Dictionary<string, string>();
            foreach (var fileName in Directory.GetFiles(projectFolder, "packages.config", SearchOption.AllDirectories).Where(f => includeTest || !f.Contains(@"Test\"))) {
                try {
                    var document = XDocument.Load(fileName);
                    foreach (var element in document.XPathSelectElements("/packages/package")) {
                        var id = element.Attribute("id")?.Value;
                        var version = element.Attribute("version")?.Value;
                        if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(version) && !errorsAndInfos.Errors.Any()) {
                            errorsAndInfos.Errors.Add(string.Format(Texts.InvalidXmlFile, fileName));
                            continue;
                        }

                        if (element.Attributes("developmentDependency").Any(a => a.Value.ToLower() == "true")) { continue; }

                        if (dependencyIdsAndVersions.ContainsKey(id) && dependencyIdsAndVersions[id] == version) { continue; }

                        if (dependencyIdsAndVersions.ContainsKey(id)) {
                            errorsAndInfos.Errors.Add(string.Format(Texts.PackageVersionClashDueToFile, fileName, id, version, dependencyIdsAndVersions[id]));
                            continue;
                        }

                        dependencyIdsAndVersions[id] = version;
                    }
                } catch {
                    if (!errorsAndInfos.Errors.Any()) {
                        errorsAndInfos.Errors.Add(string.Format(Texts.InvalidXmlFile, fileName));
                    }
                }
            }

            foreach (var fileName in Directory.GetFiles(projectFolder, "*.csproj", SearchOption.AllDirectories).Where(f => includeTest || !f.Contains(@"Test\"))) {
                try {
                    var document = XDocument.Load(fileName);
                    foreach (var element in document.XPathSelectElements("/Project/ItemGroup/PackageReference")) {
                        var id = element.Attribute("Include")?.Value;
                        var version = element.Attribute("Version")?.Value;
                        if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(version) && !errorsAndInfos.Errors.Any()) {
                            errorsAndInfos.Errors.Add(string.Format(Texts.InvalidXmlFile, fileName));
                            continue;
                        }

                        if (dependencyIdsAndVersions.ContainsKey(id) && dependencyIdsAndVersions[id] == version) { continue; }

                        if (dependencyIdsAndVersions.ContainsKey(id)) {
                            errorsAndInfos.Errors.Add(string.Format(Texts.PackageVersionClashDueToFile, fileName, id, version, dependencyIdsAndVersions[id]));
                            continue;
                        }

                        dependencyIdsAndVersions[id] = version;
                    }
                } catch {
                    if (!errorsAndInfos.Errors.Any()) {
                        errorsAndInfos.Errors.Add(string.Format(Texts.InvalidXmlFile, fileName));
                    }
                }
            }

            return dependencyIdsAndVersions;
        }
    }
}

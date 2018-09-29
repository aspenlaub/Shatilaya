﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using Aspenlaub.Net.GitHub.CSharp.PeghStandard.Components;
using Aspenlaub.Net.GitHub.CSharp.PeghStandard.Entities;
using Aspenlaub.Net.GitHub.CSharp.PeghStandard.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces;
using IComponentProvider = Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces.IComponentProvider;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya {
    public class ObsoletePackageFinder : IObsoletePackageFinder {
        protected IComponentProvider ComponentProvider { get; }

        public ObsoletePackageFinder(IComponentProvider componentProvider) {
            ComponentProvider = componentProvider;
        }

        public void FindObsoletePackages(string solutionFolder, IErrorsAndInfos errorsAndInfos) {
            var dependencyIdsAndVersions = ComponentProvider.PackageConfigsScanner.DependencyIdsAndVersions(solutionFolder, true, errorsAndInfos);
            if (!Directory.Exists(solutionFolder + @"\packages\")) { return; }

            var folders = Directory.GetDirectories(solutionFolder + @"\packages\").ToList().Where(f => !f.Contains("OctoPack") && !f.Contains("CodeAnalysis")).ToList();
            var okayFolders = new List<string>();
            foreach (var dependencyIdAndVersion in dependencyIdsAndVersions) {
                okayFolders.AddRange(folders.Where(f => f.Contains(dependencyIdAndVersion.Key) && f.Contains(dependencyIdAndVersion.Value)));
            }

            folders = folders.Where(f => !okayFolders.Contains(f)).ToList();
            if (!folders.Any()) { return; }

            foreach (var folder in folders.Select(f => new Folder(f))) {
                var deleter = new FolderDeleter();
                foreach (var file in new[] {@"dll", @"pdb", @"nupkg", @"_"}.SelectMany(extension => Directory.GetFiles(folder.FullName, $"*.{extension}", SearchOption.AllDirectories))) {
                    File.Delete(file);
                }

                if (deleter.CanDeleteFolder(folder)) {
                    deleter.DeleteFolder(folder);
                    errorsAndInfos.Infos.Add(string.Format(Texts.ObsoleteFolderDeleted, folder.FullName));
                } else {
                    errorsAndInfos.Errors.Add(string.Format(Texts.FolderIsObsolete, folder.FullName));
                }
            }
        }
    }
}

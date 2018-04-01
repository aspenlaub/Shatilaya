using System.Collections.Generic;
using System.IO;
using System.Linq;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Components;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
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

            var folders = Directory.GetDirectories(solutionFolder + @"\packages\").ToList().Where(f => !f.Contains("OctoPack")).ToList();
            var okayFolders = new List<string>();
            foreach (var dependencyIdAndVersion in dependencyIdsAndVersions) {
                okayFolders.AddRange(folders.Where(f => f.Contains(dependencyIdAndVersion.Key) && f.Contains(dependencyIdAndVersion.Value)));
            }

            folders = folders.Where(f => !okayFolders.Contains(f)).ToList();
            if (!folders.Any()) { return; }

            foreach (var folder in folders.Select(f => new Folder(f))) {
                var deleter = new FolderDeleter();
                if (deleter.CanDeleteFolder(folder)) {
                    deleter.DeleteFolder(folder);
                    errorsAndInfos.Infos.Add(string.Format(Properties.Resources.ObsoleteFolderDeleted, folder.FullName));
                } else {
                    errorsAndInfos.Errors.Add(string.Format(Properties.Resources.FolderIsObsolete, folder.FullName));
                }
            }
        }
    }
}

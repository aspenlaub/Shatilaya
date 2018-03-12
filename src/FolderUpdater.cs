using System;
using System.IO;
using System.Linq;
using Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya {
    public class FolderUpdater : IFolderUpdater {
        private const int MinimumBinaryFileSizeInBytes = 4000;

        public void UpdateFolder(IFolder sourceFolder, IFolder destinationFolder, FolderUpdateMethod folderUpdateMethod) {
            if (folderUpdateMethod != FolderUpdateMethod.Assemblies) {
                throw new NotImplementedException("Update method is not implemented");
            }

            if (!destinationFolder.Exists()) {
                Directory.CreateDirectory(destinationFolder.FullName);
            }
            foreach (var sourceFileInfo in Directory.GetFiles(sourceFolder.FullName, "*.*").Select(f => new FileInfo(f))) {
                var destinationFileInfo = new FileInfo(destinationFolder.FullName + '\\' + sourceFileInfo.Name);
                if (File.Exists(destinationFileInfo.FullName)) {
                    if (sourceFileInfo.Length == 0 && destinationFileInfo.Length == 0) { continue; }

                    if (sourceFileInfo.Length == destinationFileInfo.Length) {
                        var sourceContents = File.ReadAllBytes(sourceFileInfo.FullName);
                        var destinationContents = File.ReadAllBytes(destinationFileInfo.FullName);
                        if (sourceContents.Length == destinationContents.Length) {
                            var differences = sourceContents.Where((t, i) => t != destinationContents[i]).Count();
                            if (differences == 0) {
                                continue;
                            }
                            if (IsBinary(sourceFileInfo.Name) && differences < 50 && sourceFileInfo.Length >= MinimumBinaryFileSizeInBytes) {
                                continue;
                            }
                        }
                    }
                }

                File.Copy(sourceFileInfo.FullName, destinationFileInfo.FullName, true);
                File.SetLastWriteTime(destinationFileInfo.FullName, sourceFileInfo.LastWriteTime);
            }
        }

        protected static bool IsBinary(string fileName) {
            return fileName.EndsWith(@".exe") || fileName.EndsWith(@".dll") || fileName.EndsWith(@".pdb");
        }
    }
}

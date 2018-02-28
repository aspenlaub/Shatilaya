/* V=103 */
using System;
using System.Linq;
using System.IO;
using Aspenlaub.Net.GitHub.CSharp.Shatilaya.Entities;
using Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya {
    public class FolderDeleter : IFolderDeleter {
        protected readonly string Obj = @"\obj", CTemp = @"C:\Temp\", Git = @"\.git";
        protected readonly int MinimumFolderNameLength = 20;

        public bool CanDeleteFolder(IFolder folder, out IFolderDeleteGates folderDeleteGates) {
            folderDeleteGates = new FolderDeleteGates {
                FolderNameIsLongEnough = folder.FullName.Length > MinimumFolderNameLength,
                EndsWithObj = folder.FullName.EndsWith(Obj),
                CTemp = folder.FullName.StartsWith(CTemp, StringComparison.OrdinalIgnoreCase),
                NotTooManyFilesInFolder = true,
                IsGitCheckOutFolder = Directory.Exists(folder.FullName + Git)
            };
            if (folderDeleteGates.IsGitCheckOutFolder) { return true; }
            if (!folderDeleteGates.FolderNameIsLongEnough) { return false; }
            if (folderDeleteGates.EndsWithObj) { return true; }
            if (folderDeleteGates.CTemp) { return true; }

            var directoryInfo = new DirectoryInfo(folder.FullName);
            var files = directoryInfo.GetFiles("*.*", SearchOption.AllDirectories).ToList();
            folderDeleteGates.NotTooManyFilesInFolder = files.Count <= 10;

            return folderDeleteGates.NotTooManyFilesInFolder;
        }

        public bool CanDeleteFolder(IFolder folder) {
            IFolderDeleteGates folderDeleteGates;
            return CanDeleteFolder(folder, out folderDeleteGates);
        }

        public bool CanDeleteFolderDoubleCheck(IFolder folder) {
            if (Directory.Exists(folder.FullName + @"\.git")) { return true; }
            if (folder.FullName.Length <= MinimumFolderNameLength) { return false; }
            if (folder.FullName.EndsWith(Obj)) { return true; }
            if (folder.FullName.StartsWith(CTemp, StringComparison.OrdinalIgnoreCase)) { return true; }

            var directoryInfo = new DirectoryInfo(folder.FullName);
            var files = directoryInfo.GetFiles("*.*", SearchOption.AllDirectories).ToList();

            return files.Count <= 10;
        }

        public void DeleteFolder(IFolder folder) {
            if (!CanDeleteFolder(folder)) {
                throw new ArgumentException("This folder cannot be deleted, use CanDeleteDirectoy to find out why");
            }
            if (!CanDeleteFolderDoubleCheck(folder)) {
                throw new ArgumentException("Double check on CanDeleteDirectoy FAILED");
            }

            Directory.Delete(folder.FullName, true);
        }
    }
}

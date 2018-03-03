﻿using System;
using System.Linq;
using System.IO;
using Aspenlaub.Net.GitHub.CSharp.Shatilaya.Entities;
using Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya {
    public class FolderDeleter : IFolderDeleter {
        protected readonly string Obj = @"\obj", CTemp = @"C:\Temp\";
        protected readonly int MinimumFolderNameLength = 20;

        public bool CanDeleteFolder(IFolder folder, out IFolderDeleteGates folderDeleteGates) {
            folderDeleteGates = new FolderDeleteGates {
                FolderNameIsLongEnough = folder.FullName.Length > MinimumFolderNameLength,
                EndsWithObj = folder.FullName.EndsWith(Obj),
                CTemp = folder.FullName.StartsWith(CTemp, StringComparison.OrdinalIgnoreCase),
                NotTooManyFilesInFolder = true,
                IsGitCheckOutFolder = Directory.Exists(folder.FullName + GitUtilities.GitSubFolder)
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
            if (Directory.Exists(folder.FullName + GitUtilities.GitSubFolder)) { return true; }
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

            MakeFilesDeletable(folder);
            Directory.Delete(folder.FullName, true);
        }

        protected void MakeFilesDeletable(IFolder folder) {
            if (!folder.HasSubFolder(GitUtilities.GitSubFolder)) { return; }

            folder = new Folder(folder.FullName + GitUtilities.GitSubFolder);
            var directoryInfo = new DirectoryInfo(folder.FullName) { Attributes = FileAttributes.Normal };
            directoryInfo.Attributes = directoryInfo.Attributes & ~FileAttributes.Hidden;
            foreach (var fileInfo in directoryInfo.GetFileSystemInfos("*", SearchOption.AllDirectories).Where(fileName => (fileName.Attributes & FileAttributes.Normal) == 0)) {
                fileInfo.Attributes = FileAttributes.Normal;
            }
        }
    }
}

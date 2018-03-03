﻿using System.IO;
using Aspenlaub.Net.GitHub.CSharp.Shatilaya.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Test {
    [TestClass]
    public class FolderExtensionsTest {
        [TestMethod]
        public void CanCheckIfExists() {
            var sut = new Folder(Path.GetTempPath());
            Assert.IsTrue(sut.Exists());
            sut = new Folder(@"c:\blabla\tata\googoo");
            Assert.IsFalse(sut.Exists());
        }

        [TestMethod]
        public void CanCheckIfSubFolderExists() {
            var sut = new Folder(Path.GetTempPath());
            const string subFolder = @"\FolderExtensionsTest";
            if (Directory.Exists(sut.FullName + subFolder)) {
                Directory.Delete(sut.FullName + subFolder);
            }
            Assert.IsFalse(sut.HasSubFolder(subFolder));
            Directory.CreateDirectory(sut.FullName + subFolder);
            Assert.IsTrue(sut.HasSubFolder(subFolder));
            Directory.Delete(sut.FullName + subFolder);
        }

        [TestMethod]
        public void CanGetParentFolder() {
            var sut = new Folder(@"c:\temp\folderextensionstest");
            var parentFolder = sut.ParentFolder();
            Assert.AreEqual(@"c:\temp", parentFolder.FullName);
            parentFolder = parentFolder.ParentFolder();
            Assert.AreEqual(@"c:", parentFolder.FullName);
            parentFolder = parentFolder.ParentFolder();
            Assert.IsNull(parentFolder);
        }
    }
}

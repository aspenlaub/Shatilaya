using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using Aspenlaub.Net.GitHub.CSharp.Shatilaya.Entities;
using Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TechTalk.SpecFlow;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Test {
    [Binding]
    public class FolderUpdaterSteps {
        protected IFolder SourceFolder, DestinationFolder;
        protected string EmptySourceFileFullName, EmptyDestinationFileFullName;
        protected IDictionary<int, string> SourceFiles, DestinationFiles;
        protected IDictionary<string, DateTime> LastWriteTimeBeforeUpdate;
        protected IFolderUpdater Sut;

        public FolderUpdaterSteps() {
            SourceFolder = new Folder(Path.GetTempPath() + nameof(FolderUpdaterSteps) + @"\Source");
            DestinationFolder = new Folder(Path.GetTempPath() + nameof(FolderUpdaterSteps) + @"\Destination");
            EmptySourceFileFullName = SourceFolder.FullName + @"\empty.txt";
            EmptyDestinationFileFullName = DestinationFolder.FullName + @"\empty.txt";
            SourceFiles = new Dictionary<int, string>();
            DestinationFiles = new Dictionary<int, string>();
            Sut = new FolderUpdater();
        }

        [AfterScenario("FolderUpdater")]
        public void CleanUp() {
            if (SourceFolder.Exists()) {
                Directory.Delete(SourceFolder.FullName, true);
            }
            if (DestinationFolder.Exists()) {
                Directory.Delete(DestinationFolder.FullName, true);
            }
        }

        [Given(@"I have an empty source and an empty destination folder beneath the user's temp folder")]
        public void GivenIHaveAnEmptySourceAndAnEmptyDestinationFolderBeneathTheUserSTempFolder() {
            CleanUp();
            Directory.CreateDirectory(SourceFolder.FullName);
            Directory.CreateDirectory(DestinationFolder.FullName);
        }

        [Given(@"The destination folder does not exist")]
        public void GivenTheDestinationFolderDoesNotExist() {
            if (!DestinationFolder.Exists()) { return; }

            Directory.Delete(DestinationFolder.FullName, true);
        }

        [Given(@"I place an empty file into the source folder")]
        public void GivenIPlaceAnEmptyFileIntoTheSourceFolder() {
            File.WriteAllText(EmptySourceFileFullName, "");
        }

        [Given(@"I place a (.*) kilobyte file into the source folder")]
        public void GivenIPlaceAKilobyteFileIntoTheSourceFolder(int p0) {
            var bytes = new List<byte>();
            byte b = 0;
            for (var i = 0; i < p0 * 1000; i++) {
                bytes.Add(b);
                b = (byte)((b + 1) % 256);
            }

            if (!SourceFiles.ContainsKey(p0)) {
                SourceFiles[p0] = SourceFolder.FullName + @"\" + p0 + "kilobytes.dll";
            }
            File.WriteAllBytes(SourceFiles[p0], bytes.ToArray());
        }

        [Given(@"I place a (.*) kilobyte file with (.*) differences into the destintion folder")]
        public void GivenIPlaceAKilobyteFileWithDifferencesIntoTheDestintionFolder(int p0, int p1) {
            var bytes = new List<byte>();
            byte b = 0;
            for (var i = 0; i < p0 * 1000; i++) {
                if (p1 == 0 || i % 10 != 0) {
                    bytes.Add(b);
                } else {
                    bytes.Add((byte)((b + 3) % 256));
                    p1 --;
                }
                b = (byte)((b + 1) % 256);
            }

            if (!DestinationFiles.ContainsKey(p0)) {
                DestinationFiles[p0] = DestinationFolder.FullName + @"\" + p0 + "kilobytes.dll";
            }
            File.WriteAllBytes(DestinationFiles[p0], bytes.ToArray());
        }

        [When(@"I update the destination folder")]
        public void WhenIUpdateTheDestinationFolder() {
            LastWriteTimeBeforeUpdate = new Dictionary<string, DateTime>();
            if (DestinationFolder.Exists()) {
                foreach (var fileName in Directory.GetFiles(DestinationFolder.FullName, "*.*")) {
                    LastWriteTimeBeforeUpdate[fileName] = File.GetLastWriteTime(fileName);
                }
            }

            Sut.UpdateFolder(SourceFolder, DestinationFolder, FolderUpdateMethod.Assemblies);
        }

        [When(@"I overwrite the empty file in the source folder")]
        public void WhenIOverwriteTheEmptyFileInTheSourceFolder() {
            File.WriteAllText(EmptySourceFileFullName, "");
        }

        [When(@"I wait two seconds")]
        public void WhenIWaitTwoSeconds() {
            Thread.Sleep(TimeSpan.FromSeconds(2));
        }

        [Then(@"the destination folder exists")]
        public void ThenTheDestinationFolderExists() {
            Assert.IsTrue(DestinationFolder.Exists());
        }

        [Then(@"the empty file is present in the destination folder")]
        public void ThenTheEmptyFileIsPresentInTheDestinationFolder() {
            File.Exists(EmptyDestinationFileFullName);
        }

        [Then(@"the last write times of the empty source and the empty destination files are identical")]
        public void ThenTheLastWriteTimesOfTheEmptySourceAndTheEmptyDestinationFilesIsIdentical() {
            Assert.AreEqual(File.GetLastWriteTime(EmptySourceFileFullName), File.GetLastWriteTime(EmptyDestinationFileFullName));
        }

        [Then(@"the last write time of the empty destination file is unchanged")]
        public void ThenTheLastWriteTimeOfTheEmptyDestinationFileIsUnchanged() {
            Assert.AreEqual(LastWriteTimeBeforeUpdate[EmptyDestinationFileFullName], File.GetLastWriteTime(EmptyDestinationFileFullName));
        }

        [Then(@"the (.*) kilobyte destination file is identical to the (.*) kilobyte source file")]
        public void ThenTheKilobyteDestinationFileIsIdenticalToTheKilobyteSourceFile(int p0, int p1) {
            Assert.IsTrue(File.Exists(SourceFiles[p1]));
            Assert.IsTrue(File.Exists(DestinationFiles[p0]));
            Assert.IsTrue(File.ReadAllBytes(SourceFiles[p1]).SequenceEqual(File.ReadAllBytes(DestinationFiles[p0])));
        }

        [Then(@"the last write times of the (.*) kilobyte source and the (.*) kilobyte destination files are identical")]
        public void ThenTheLastWriteTimesOfTheKilobyteSourceAndTheKilobyteDestinationFilesAreIdentical(int p0, int p1) {
            Assert.IsTrue(File.Exists(SourceFiles[p0]));
            Assert.IsTrue(File.Exists(DestinationFiles[p1]));
            Assert.AreEqual(File.GetLastWriteTime(SourceFiles[p0]), File.GetLastWriteTime(DestinationFiles[p1]));
        }

        [Then(@"the last write time of the (.*) kilobyte destination file is unchanged")]
        public void ThenTheLastWriteTimeOfTheKilobyteDestinationFileIsUnchanged(int p0) {
            Assert.AreEqual(LastWriteTimeBeforeUpdate[DestinationFiles[p0]], File.GetLastWriteTime(DestinationFiles[p0]));
        }
    }
}
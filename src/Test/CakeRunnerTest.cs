using System.IO;
using System.Linq;
using Aspenlaub.Net.GitHub.CSharp.PeghStandard.Components;
using Aspenlaub.Net.GitHub.CSharp.PeghStandard.Entities;
using Aspenlaub.Net.GitHub.CSharp.PeghStandard.Extensions;
using Aspenlaub.Net.GitHub.CSharp.PeghStandard.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using IComponentProvider = Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces.IComponentProvider;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Test {
    [TestClass]
    public class CakeRunnerTest {
        protected static ICakeRunner Sut;
        protected static string CakeExeFileFullName;
        protected static IFolder ScriptsFolder;
        protected const string SuccessCakeUrl = "https://raw.githubusercontent.com/aspenlaub/Shatilaya/master/src/Test/success.cake";
        protected const string FailureCakeUrl = "https://raw.githubusercontent.com/aspenlaub/Shatilaya/master/src/Test/failure.cake";
        protected const string ThisIsNotCake = @"This is not a cake!";

        [ClassInitialize]
        public static void Initialize(TestContext context) {
            DeleteFolder(CakeFolder());
            ICakeInstaller cakeInstaller = new CakeInstaller();
            cakeInstaller.InstallCake(CakeFolder(), out var errorsAndInfos);
            Assert.IsFalse(errorsAndInfos.AnyErrors(), string.Join("\r\n", errorsAndInfos.Errors));
            CakeExeFileFullName = cakeInstaller.CakeExeFileFullName(CakeFolder());

            ScriptsFolder = CakeScriptsFolder();
            DeleteFolder(ScriptsFolder);
            Directory.CreateDirectory(ScriptsFolder.FullName);
            using (var webClient = new System.Net.WebClient()) {
                webClient.DownloadFile(SuccessCakeUrl, ScriptsFolder.FullName + @"\success.cake");
                webClient.DownloadFile(FailureCakeUrl, ScriptsFolder.FullName + @"\failure.cake");
            }

            var componentProviderMock = new Mock<IComponentProvider>();
            componentProviderMock.SetupGet(c => c.ProcessRunner).Returns(new ProcessRunner());
            Sut = new CakeRunner(componentProviderMock.Object);
        }

        [ClassCleanup]
        public static void Cleanup() {
            DeleteFolder(CakeFolder());
            DeleteFolder(CakeScriptsFolder());
        }

        private static void DeleteFolder(IFolder folder) {
            if (!folder.Exists()) {
                return;
            }

            var deleter = new FolderDeleter();
            deleter.DeleteFolder(folder);
        }

        protected static IFolder CakeFolder() {
            return new Folder(Path.GetTempPath() + @"Cake");
        }

        protected static IFolder CakeScriptsFolder() {
            return new Folder(Path.GetTempPath() + nameof(CakeRunnerTest));
        }

        [TestMethod]
        public void CanCallScriptWithoutErrors() {
            var errorsAndInfos = new ErrorsAndInfos();

            Sut.CallCake(CakeExeFileFullName, ScriptsFolder.FullName + @"\success.cake", errorsAndInfos);
            Assert.IsFalse(errorsAndInfos.Errors.Any());
            Assert.IsTrue(errorsAndInfos.Infos.Any(m => m.Contains(@"Task")));
            Assert.IsTrue(errorsAndInfos.Infos.Any(m => m.Contains(@"Duration")));
            Assert.IsTrue(errorsAndInfos.Infos.Any(m => m.Contains(@"00:00:00")));
        }

        [TestMethod]
        public void CanCallScriptWithErrors() {
            var errorsAndInfos = new ErrorsAndInfos();

            Sut.CallCake(CakeExeFileFullName, ScriptsFolder.FullName + @"\failure.cake", errorsAndInfos);
            Assert.AreEqual(1, errorsAndInfos.Errors.Count);
            Assert.AreEqual("This is not a cake!", errorsAndInfos.Errors[0]);
            Assert.IsTrue(errorsAndInfos.Infos.Any(m => m.Contains(@"Task")));
            Assert.IsTrue(errorsAndInfos.Infos.Any(m => m.Contains(@"Duration")));
            Assert.IsTrue(errorsAndInfos.Infos.Any(m => m.Contains(@"00:00:00")));
            Assert.IsFalse(errorsAndInfos.Infos.Any(m => m.Contains(ThisIsNotCake)));
        }

        [TestMethod]
        public void CanCallScriptAgainstNonExistingTargetButGetAnError() {
            var errorsAndInfos = new ErrorsAndInfos();
            Sut.CallCake(CakeExeFileFullName, ScriptsFolder.FullName + @"\success.cake", "NonExistingTarget", errorsAndInfos);
            Assert.AreEqual(2, errorsAndInfos.Errors.Count);
            Assert.IsTrue(errorsAndInfos.Errors.Any(e => e.Contains("The target 'NonExistingTarget' was not found")));
        }

        [TestMethod]
        public void CanCallScriptAgainstAlternativeTarget() {
            var errorsAndInfos = new ErrorsAndInfos();
            Sut.CallCake(CakeExeFileFullName, ScriptsFolder.FullName + @"\success.cake", "AlternativeTarget", errorsAndInfos);
            Assert.IsFalse(errorsAndInfos.Errors.Any(), string.Join("\r\n", errorsAndInfos));
            Assert.IsTrue(errorsAndInfos.Infos.Any(i => i.Contains("This is an alternative target")));
        }
    }
}

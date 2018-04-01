using System.IO;
using System.Linq;
using System.Xml.Linq;
using LibGit2Sharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Aspenlaub.Net.GitHub.CSharp.Pegh;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Test {
    [TestClass]
    public class ProjectFactoryTest {
        protected static TestTargetFolder PakledConsumerTarget = new TestTargetFolder(nameof(ProjectFactoryTest), "PakledConsumer");

        protected XDocument Document;

        [TestInitialize]
        public void Initialize() {
            PakledConsumerTarget.Delete();
        }

        [TestCleanup]
        public void TestCleanup() {
            PakledConsumerTarget.Delete();
        }

        [TestMethod]
        public void CanLoadProject() {
            var gitUtilities = new GitUtilities();
            var errorsAndInfos = new ErrorsAndInfos();
            const string url = "https://github.com/aspenlaub/PakledConsumer.git";
            gitUtilities.Clone(url, PakledConsumerTarget.Folder(), new CloneOptions { BranchName = "master" }, errorsAndInfos);
            Assert.IsFalse(errorsAndInfos.Errors.Any(), string.Join("\r\n", errorsAndInfos.Errors));
            var solutionFileFullName = PakledConsumerTarget.Folder().SubFolder("src").FullName + @"\" + PakledConsumerTarget.SolutionId + ".sln";
            var projectFileFullName = PakledConsumerTarget.Folder().SubFolder("src").FullName + @"\" + PakledConsumerTarget.SolutionId + ".csproj";
            Assert.IsTrue(File.Exists(projectFileFullName));
            var sut = new ProjectFactory();
            var project = sut.Load(solutionFileFullName, projectFileFullName, errorsAndInfos);
            Assert.IsFalse(errorsAndInfos.Errors.Any(), string.Join("\r\n", errorsAndInfos.Errors));
            Assert.IsNotNull(project);
            Assert.AreEqual(projectFileFullName, project.ProjectFileFullName);
            Assert.AreEqual(PakledConsumerTarget.SolutionId, project.ProjectName);
            Assert.AreEqual("14.0", project.ToolsVersion);
            Assert.AreEqual(3, project.PropertyGroups.Count);
            var rootNamespace = "";
            foreach (var propertyGroup in project.PropertyGroups) {
                Assert.IsNotNull(propertyGroup);
                Assert.AreEqual(propertyGroup.AssemblyName, propertyGroup.RootNamespace);
                if (propertyGroup.Condition == "") {
                    rootNamespace = propertyGroup.RootNamespace;
                    Assert.IsTrue(propertyGroup.AssemblyName.StartsWith("Aspenlaub.Net.GitHub.CSharp." + PakledConsumerTarget.SolutionId), $"Unexpected assembly name \"{propertyGroup.AssemblyName}\"");
                    Assert.AreEqual("", propertyGroup.UseVsHostingProcess);
                    Assert.AreEqual("", propertyGroup.GenerateBuildInfoConfigFile);
                    Assert.AreEqual("", propertyGroup.IntermediateOutputPath);
                    Assert.AreEqual("", propertyGroup.OutputPath);
                } else {
                    Assert.AreEqual("", propertyGroup.AssemblyName);
                    if (propertyGroup.Condition.Contains("Debug|")) {
                        Assert.AreEqual("", propertyGroup.UseVsHostingProcess);
                        Assert.AreEqual(@"..\temp\obj\Debug\", propertyGroup.IntermediateOutputPath);
                        Assert.AreEqual(@"..\..\" + PakledConsumerTarget.SolutionId + @"Bin\Debug\", propertyGroup.OutputPath);
                    } else {
                        Assert.AreEqual("false", propertyGroup.UseVsHostingProcess);
                        Assert.AreEqual(@"..\temp\obj\Release\", propertyGroup.IntermediateOutputPath);
                        Assert.AreEqual(@"..\..\" + PakledConsumerTarget.SolutionId + @"Bin\Release\", propertyGroup.OutputPath);
                    }
                    Assert.AreEqual("false", propertyGroup.GenerateBuildInfoConfigFile);
                }
            }

            Assert.AreEqual(2, project.ReferencedDllFiles.Count);
            Assert.IsTrue(project.ReferencedDllFiles.Any(f => f.EndsWith(".Pakled.dll")));
            Assert.IsTrue(project.ReferencedDllFiles.Any(f => f.EndsWith(".Json.dll")));

            Assert.AreEqual(rootNamespace, project.RootNamespace);

            projectFileFullName = PakledConsumerTarget.Folder().SubFolder("src").FullName + @"\Test\" + PakledConsumerTarget.SolutionId + ".Test.csproj";
            Assert.IsTrue(File.Exists(projectFileFullName));
            project = sut.Load(solutionFileFullName, projectFileFullName, errorsAndInfos);
            Assert.IsFalse(errorsAndInfos.Errors.Any(), string.Join("\r\n", errorsAndInfos.Errors));
            Assert.IsNotNull(project);
            Assert.AreEqual(projectFileFullName, project.ProjectFileFullName);
            Assert.AreEqual(PakledConsumerTarget.SolutionId + ".Test", project.ProjectName);
        }
    }
}

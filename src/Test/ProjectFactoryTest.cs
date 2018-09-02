using System.IO;
using System.Linq;
using Aspenlaub.Net.GitHub.CSharp.PeghStandard.Entities;
using Aspenlaub.Net.GitHub.CSharp.PeghStandard.Extensions;
using Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces;
using LibGit2Sharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Test {
    [TestClass]
    public class ProjectFactoryTest {
        protected static TestTargetFolder PakledConsumerTarget = new TestTargetFolder(nameof(ProjectFactoryTest), "PakledConsumer");
        protected static TestTargetFolder ChabStandardTarget = new TestTargetFolder(nameof(ProjectFactoryTest), "ChabStandard");

        protected IComponentProvider ComponentProvider;

        public ProjectFactoryTest() {
            var componentProviderMock = new Mock<IComponentProvider>();
            componentProviderMock.SetupGet(c => c.ProcessRunner).Returns(new ProcessRunner());
            componentProviderMock.SetupGet(c => c.CakeRunner).Returns(new CakeRunner(componentProviderMock.Object));
            ComponentProvider = componentProviderMock.Object;
        }

        [ClassInitialize]
        public static void ClassInitialize(TestContext context) {
            PakledConsumerTarget.DeleteCakeFolder();
            PakledConsumerTarget.CreateCakeFolder();
            ChabStandardTarget.DeleteCakeFolder();
            ChabStandardTarget.CreateCakeFolder();
        }

        [ClassCleanup]
        public static void ClassCleanup() {
            PakledConsumerTarget.DeleteCakeFolder();
            ChabStandardTarget.DeleteCakeFolder();
        }

        [TestInitialize]
        public void Initialize() {
            PakledConsumerTarget.Delete();
            ChabStandardTarget.Delete();
        }

        [TestCleanup]
        public void TestCleanup() {
            PakledConsumerTarget.Delete();
            ChabStandardTarget.Delete();
        }

        [TestMethod]
        public void CanLoadPakledConsumerProject() {
            var gitUtilities = new GitUtilities();
            var errorsAndInfos = new ErrorsAndInfos();
            const string url = "https://github.com/aspenlaub/PakledConsumer.git";
            gitUtilities.Clone(url, PakledConsumerTarget.Folder(), new CloneOptions { BranchName = "master" }, true, errorsAndInfos);
            Assert.IsFalse(errorsAndInfos.Errors.Any(), string.Join("\r\n", errorsAndInfos.Errors));

            var latestBuildCakeScriptProvider = new LatestBuildCakeScriptProvider();
            var cakeScript = latestBuildCakeScriptProvider.GetLatestBuildCakeScript();
            cakeScript = CakeBuildUtilities.UseLocalShatilayaAssemblies(cakeScript);
            var cakeScriptFileFullName = PakledConsumerTarget.Folder().FullName + @"\build.cake";
            File.WriteAllText(cakeScriptFileFullName, cakeScript);

            PakledConsumerTarget.RunBuildCakeScript(ComponentProvider, "IgnoreOutdatedBuildCakePendingChangesAndDoNotPush", errorsAndInfos);
            Assert.IsFalse(errorsAndInfos.Errors.Any(), string.Join("\r\n", errorsAndInfos.Errors));

            var solutionFileFullName = PakledConsumerTarget.Folder().SubFolder("src").FullName + @"\" + PakledConsumerTarget.SolutionId + ".sln";
            var projectFileFullName = PakledConsumerTarget.Folder().SubFolder("src").FullName + @"\" + PakledConsumerTarget.SolutionId + ".csproj";
            Assert.IsTrue(File.Exists(projectFileFullName));
            var sut = new ProjectFactory();
            var project = sut.Load(solutionFileFullName, projectFileFullName, errorsAndInfos);
            var projectLogic = new ProjectLogic();
            Assert.IsFalse(projectLogic.IsANetStandardOrCoreProject(project));
            Assert.IsTrue(projectLogic.DoAllNetStandardOrCoreConfigurationsHaveNuspecs(project));
            Assert.IsFalse(errorsAndInfos.Errors.Any(), string.Join("\r\n", errorsAndInfos.Errors));
            Assert.IsNotNull(project);
            Assert.AreEqual(projectFileFullName, project.ProjectFileFullName);
            Assert.AreEqual(PakledConsumerTarget.SolutionId, project.ProjectName);
            Assert.AreEqual("v4.6", project.TargetFramework);
            Assert.AreEqual(3, project.PropertyGroups.Count);
            var rootNamespace = "";
            foreach (var propertyGroup in project.PropertyGroups) {
                Assert.IsNotNull(propertyGroup);
                Assert.AreEqual(propertyGroup.AssemblyName, propertyGroup.RootNamespace);
                if (propertyGroup.Condition == "") {
                    rootNamespace = propertyGroup.RootNamespace;
                    Assert.IsTrue(propertyGroup.AssemblyName.StartsWith("Aspenlaub.Net.GitHub.CSharp." + PakledConsumerTarget.SolutionId), $"Unexpected assembly name \"{propertyGroup.AssemblyName}\"");
                    Assert.AreEqual("", propertyGroup.UseVsHostingProcess);
                    Assert.AreEqual("false", propertyGroup.GenerateBuildInfoConfigFile);
                    Assert.AreEqual("", propertyGroup.IntermediateOutputPath);
                    Assert.AreEqual("", propertyGroup.OutputPath);
                    Assert.AreEqual("", propertyGroup.AppendTargetFrameworkToOutputPath);
                    Assert.AreEqual("", propertyGroup.AllowUnsafeBlocks);
                    Assert.AreEqual("", propertyGroup.NuspecFile);
                    Assert.AreEqual("", propertyGroup.Deterministic);
                    Assert.AreEqual("", propertyGroup.GenerateAssemblyInfo);
                } else {
                    Assert.AreEqual("", propertyGroup.AssemblyName);
                    if (propertyGroup.Condition.Contains("Debug|")) {
                        Assert.AreEqual("", propertyGroup.UseVsHostingProcess);
                        Assert.AreEqual(@"bin\Debug\", propertyGroup.OutputPath);
                    } else {
                        Assert.AreEqual("false", propertyGroup.UseVsHostingProcess);
                        Assert.AreEqual(@"bin\Release\", propertyGroup.OutputPath);
                    }
                    Assert.AreEqual("", propertyGroup.GenerateBuildInfoConfigFile);
                    Assert.AreEqual("", propertyGroup.AppendTargetFrameworkToOutputPath);
                    Assert.AreEqual("", propertyGroup.AllowUnsafeBlocks);
                    Assert.AreEqual("", propertyGroup.NuspecFile);
                    Assert.AreEqual("", propertyGroup.Deterministic);
                    Assert.AreEqual("", propertyGroup.GenerateAssemblyInfo);
                }
            }

            Assert.AreEqual(2, project.ReferencedDllFiles.Count);
            Assert.IsTrue(project.ReferencedDllFiles.All(f => File.Exists(f)), "File/-s not found:\r\n" + string.Join("\r\n", project.ReferencedDllFiles.Where(f => !File.Exists(f))));
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

        [TestMethod]
        public void CanLoadChabStandardProject() {
            var gitUtilities = new GitUtilities();
            var errorsAndInfos = new ErrorsAndInfos();
            const string url = "https://github.com/aspenlaub/ChabStandard.git";
            gitUtilities.Clone(url, ChabStandardTarget.Folder(), new CloneOptions { BranchName = "master" }, true, errorsAndInfos);
            Assert.IsFalse(errorsAndInfos.Errors.Any(), string.Join("\r\n", errorsAndInfos.Errors));

            var latestBuildCakeScriptProvider = new LatestBuildCakeScriptProvider();
            var cakeScript = latestBuildCakeScriptProvider.GetLatestBuildCakeScript();
            cakeScript = CakeBuildUtilities.UseLocalShatilayaAssemblies(cakeScript);
            var cakeScriptFileFullName = ChabStandardTarget.Folder().FullName + @"\build.cake";
            File.WriteAllText(cakeScriptFileFullName, cakeScript);

            var solutionFileFullName = ChabStandardTarget.Folder().SubFolder("src").FullName + @"\" + ChabStandardTarget.SolutionId + ".sln";
            var projectFileFullName = ChabStandardTarget.Folder().SubFolder("src").FullName + @"\" + ChabStandardTarget.SolutionId + ".csproj";
            Assert.IsTrue(File.Exists(projectFileFullName));
            var sut = new ProjectFactory();
            var project = sut.Load(solutionFileFullName, projectFileFullName, errorsAndInfos);
            var projectLogic = new ProjectLogic();
            Assert.IsTrue(projectLogic.IsANetStandardOrCoreProject(project));
            Assert.IsTrue(projectLogic.DoAllNetStandardOrCoreConfigurationsHaveNuspecs(project));
            Assert.IsFalse(errorsAndInfos.Errors.Any(), string.Join("\r\n", errorsAndInfos.Errors));
            Assert.IsNotNull(project);
            Assert.AreEqual(projectFileFullName, project.ProjectFileFullName);
            Assert.AreEqual(ChabStandardTarget.SolutionId, project.ProjectName);
            Assert.AreEqual("netstandard2.0", project.TargetFramework);
            Assert.AreEqual(3, project.PropertyGroups.Count);
            var rootNamespace = "";
            foreach (var propertyGroup in project.PropertyGroups) {
                Assert.IsNotNull(propertyGroup);
                Assert.AreEqual(propertyGroup.AssemblyName, propertyGroup.RootNamespace);
                if (propertyGroup.Condition == "") {
                    rootNamespace = propertyGroup.RootNamespace;
                    Assert.IsTrue(propertyGroup.AssemblyName.StartsWith("Aspenlaub.Net.GitHub.CSharp." + ChabStandardTarget.SolutionId), $"Unexpected assembly name \"{propertyGroup.AssemblyName}\"");
                    Assert.AreEqual("", propertyGroup.UseVsHostingProcess);
                    Assert.AreEqual("false", propertyGroup.GenerateBuildInfoConfigFile);
                    Assert.AreEqual("", propertyGroup.IntermediateOutputPath);
                    Assert.AreEqual("", propertyGroup.OutputPath);
                    Assert.AreEqual("false", propertyGroup.AppendTargetFrameworkToOutputPath);
                    Assert.AreEqual("", propertyGroup.AllowUnsafeBlocks);
                    Assert.AreEqual("", propertyGroup.NuspecFile);
                    Assert.AreEqual("false", propertyGroup.Deterministic);
                    Assert.AreEqual("false", propertyGroup.GenerateAssemblyInfo);
                } else {
                    Assert.AreEqual("", propertyGroup.AssemblyName);
                    if (propertyGroup.Condition.Contains("Debug|")) {
                        Assert.AreEqual("", propertyGroup.UseVsHostingProcess);
                        Assert.AreEqual("", propertyGroup.OutputPath);
                        Assert.AreEqual("", propertyGroup.AppendTargetFrameworkToOutputPath);
                        Assert.AreEqual("", propertyGroup.AllowUnsafeBlocks);
                        Assert.AreEqual("", propertyGroup.NuspecFile);
                    } else {
                        Assert.AreEqual("", propertyGroup.UseVsHostingProcess);
                        Assert.AreEqual("", propertyGroup.OutputPath);
                        Assert.AreEqual("", propertyGroup.AppendTargetFrameworkToOutputPath);
                        Assert.AreEqual("", propertyGroup.AllowUnsafeBlocks);
                        Assert.AreEqual("ChabStandard.nuspec", propertyGroup.NuspecFile);
                    }
                    Assert.AreEqual("", propertyGroup.GenerateBuildInfoConfigFile);
                    Assert.AreEqual("", propertyGroup.Deterministic);
                    Assert.AreEqual("", propertyGroup.GenerateAssemblyInfo);
                }
            }

            Assert.AreEqual(0, project.ReferencedDllFiles.Count);

            Assert.AreEqual(rootNamespace, project.RootNamespace);

            projectFileFullName = ChabStandardTarget.Folder().SubFolder("src").FullName + @"\Test\" + ChabStandardTarget.SolutionId + ".Test.csproj";
            Assert.IsTrue(File.Exists(projectFileFullName));
            project = sut.Load(solutionFileFullName, projectFileFullName, errorsAndInfos);
            Assert.IsFalse(errorsAndInfos.Errors.Any(), string.Join("\r\n", errorsAndInfos.Errors));
            Assert.IsNotNull(project);
            Assert.AreEqual(projectFileFullName, project.ProjectFileFullName);
            Assert.AreEqual(ChabStandardTarget.SolutionId + ".Test", project.ProjectName);
        }
    }
}

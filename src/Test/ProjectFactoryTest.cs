using System.IO;
using System.Linq;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Extensions;
using Aspenlaub.Net.GitHub.CSharp.Shatilaya.Extensions;
using Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces;
using LibGit2Sharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Test {
    [TestClass]
    public class ProjectFactoryTest {
        protected static TestTargetFolder PakledConsumerCoreTarget = new TestTargetFolder(nameof(ProjectFactoryTest), "PakledConsumerCore");
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
            PakledConsumerCoreTarget.DeleteCakeFolder();
            PakledConsumerCoreTarget.CreateCakeFolder();
            ChabStandardTarget.DeleteCakeFolder();
            ChabStandardTarget.CreateCakeFolder();
        }

        [ClassCleanup]
        public static void ClassCleanup() {
            PakledConsumerCoreTarget.DeleteCakeFolder();
            ChabStandardTarget.DeleteCakeFolder();
        }

        [TestInitialize]
        public void Initialize() {
            PakledConsumerCoreTarget.Delete();
            ChabStandardTarget.Delete();
        }

        [TestCleanup]
        public void TestCleanup() {
            PakledConsumerCoreTarget.Delete();
            ChabStandardTarget.Delete();
        }

        [TestMethod]
        public void CanLoadPakledConsumerProject() {
            var gitUtilities = new GitUtilities();
            var errorsAndInfos = new ErrorsAndInfos();
            const string url = "https://github.com/aspenlaub/PakledConsumerCore.git";
            gitUtilities.Clone(url, PakledConsumerCoreTarget.Folder(), new CloneOptions { BranchName = "master" }, true, errorsAndInfos);
            Assert.IsFalse(errorsAndInfos.Errors.Any(), errorsAndInfos.ErrorsPlusRelevantInfos());

            var latestBuildCakeScriptProvider = new LatestBuildCakeScriptProvider();
            var cakeScript = latestBuildCakeScriptProvider.GetLatestBuildCakeScript();
            cakeScript = CakeBuildUtilities.UseLocalShatilayaAssemblies(cakeScript);
            var cakeScriptFileFullName = PakledConsumerCoreTarget.Folder().FullName + @"\" + "build.cake";
            File.WriteAllText(cakeScriptFileFullName, cakeScript);

            PakledConsumerCoreTarget.RunBuildCakeScript(ComponentProvider, "IgnoreOutdatedBuildCakePendingChangesAndDoNotPush", errorsAndInfos);
            Assert.IsFalse(errorsAndInfos.Errors.Any(), errorsAndInfos.ErrorsPlusRelevantInfos());

            var solutionFileFullName = PakledConsumerCoreTarget.Folder().SubFolder("src").FullName + @"\" + PakledConsumerCoreTarget.SolutionId + ".sln";
            var projectFileFullName = PakledConsumerCoreTarget.Folder().SubFolder("src").FullName + @"\" + PakledConsumerCoreTarget.SolutionId + ".csproj";
            Assert.IsTrue(File.Exists(projectFileFullName));
            var sut = new ProjectFactory();
            var project = sut.Load(solutionFileFullName, projectFileFullName, errorsAndInfos);
            var projectLogic = new ProjectLogic();
            Assert.IsTrue(projectLogic.IsANetStandardOrCoreProject(project));
            Assert.IsTrue(projectLogic.DoAllNetStandardOrCoreConfigurationsHaveNuspecs(project));
            Assert.IsFalse(errorsAndInfos.Errors.Any(), errorsAndInfos.ErrorsPlusRelevantInfos());
            Assert.IsNotNull(project);
            Assert.AreEqual(projectFileFullName, project.ProjectFileFullName);
            Assert.AreEqual(PakledConsumerCoreTarget.SolutionId, project.ProjectName);
            Assert.AreEqual("netcoreapp2.2", project.TargetFramework);
            Assert.AreEqual(3, project.PropertyGroups.Count);
            var rootNamespace = "";
            foreach (var propertyGroup in project.PropertyGroups) {
                Assert.IsNotNull(propertyGroup);
                Assert.AreEqual(propertyGroup.AssemblyName, propertyGroup.RootNamespace);
                if (propertyGroup.Condition == "") {
                    rootNamespace = propertyGroup.RootNamespace;
                    Assert.IsTrue(propertyGroup.AssemblyName.StartsWith("Aspenlaub.Net.GitHub.CSharp." + PakledConsumerCoreTarget.SolutionId), $"Unexpected assembly name \"{propertyGroup.AssemblyName}\"");
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
                        Assert.AreEqual("", propertyGroup.NuspecFile);
                    } else {
                        Assert.AreEqual("", propertyGroup.UseVsHostingProcess);
                        Assert.AreEqual("", propertyGroup.OutputPath);
                        Assert.AreEqual("PakledConsumerCore.nuspec", propertyGroup.NuspecFile);
                    }
                    Assert.AreEqual("", propertyGroup.GenerateBuildInfoConfigFile);
                    Assert.AreEqual("", propertyGroup.AppendTargetFrameworkToOutputPath);
                    Assert.AreEqual("", propertyGroup.AllowUnsafeBlocks);
                    Assert.AreEqual("", propertyGroup.Deterministic);
                    Assert.AreEqual("", propertyGroup.GenerateAssemblyInfo);
                }
            }

            Assert.AreEqual(1, project.ReferencedDllFiles.Count);
            Assert.IsTrue(project.ReferencedDllFiles.Any(f => f.EndsWith("System")));

            Assert.AreEqual(rootNamespace, project.RootNamespace);

            projectFileFullName = PakledConsumerCoreTarget.Folder().SubFolder("src").FullName + @"\Test\" + PakledConsumerCoreTarget.SolutionId + ".Test.csproj";
            Assert.IsTrue(File.Exists(projectFileFullName));
            project = sut.Load(solutionFileFullName, projectFileFullName, errorsAndInfos);
            Assert.IsFalse(errorsAndInfos.Errors.Any(), errorsAndInfos.ErrorsPlusRelevantInfos());
            Assert.IsNotNull(project);
            Assert.AreEqual(projectFileFullName, project.ProjectFileFullName);
            Assert.AreEqual(PakledConsumerCoreTarget.SolutionId + ".Test", project.ProjectName);
        }

        [TestMethod]
        public void CanLoadChabStandardProject() {
            var gitUtilities = new GitUtilities();
            var errorsAndInfos = new ErrorsAndInfos();
            const string url = "https://github.com/aspenlaub/ChabStandard.git";
            gitUtilities.Clone(url, ChabStandardTarget.Folder(), new CloneOptions { BranchName = "master" }, true, errorsAndInfos);
            Assert.IsFalse(errorsAndInfos.Errors.Any(), errorsAndInfos.ErrorsPlusRelevantInfos());

            var latestBuildCakeScriptProvider = new LatestBuildCakeScriptProvider();
            var cakeScript = latestBuildCakeScriptProvider.GetLatestBuildCakeScript();
            cakeScript = CakeBuildUtilities.UseLocalShatilayaAssemblies(cakeScript);
            var cakeScriptFileFullName = ChabStandardTarget.Folder().FullName + @"\" + "build.cake";
            File.WriteAllText(cakeScriptFileFullName, cakeScript);

            var solutionFileFullName = ChabStandardTarget.Folder().SubFolder("src").FullName + @"\" + ChabStandardTarget.SolutionId + ".sln";
            var projectFileFullName = ChabStandardTarget.Folder().SubFolder("src").FullName + @"\" + ChabStandardTarget.SolutionId + ".csproj";
            Assert.IsTrue(File.Exists(projectFileFullName));
            var sut = new ProjectFactory();
            var project = sut.Load(solutionFileFullName, projectFileFullName, errorsAndInfos);
            var projectLogic = new ProjectLogic();
            Assert.IsTrue(projectLogic.IsANetStandardOrCoreProject(project));
            Assert.IsTrue(projectLogic.DoAllNetStandardOrCoreConfigurationsHaveNuspecs(project));
            Assert.IsFalse(errorsAndInfos.Errors.Any(), errorsAndInfos.ErrorsPlusRelevantInfos());
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
            Assert.IsFalse(errorsAndInfos.Errors.Any(), errorsAndInfos.ErrorsPlusRelevantInfos());
            Assert.IsNotNull(project);
            Assert.AreEqual(projectFileFullName, project.ProjectFileFullName);
            Assert.AreEqual(ChabStandardTarget.SolutionId + ".Test", project.ProjectName);
        }
    }
}

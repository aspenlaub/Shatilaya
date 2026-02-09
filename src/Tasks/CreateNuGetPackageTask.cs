using System;
using System.Linq;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Extensions;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Protch.Interfaces;
using Autofac;
using Cake.Common.Tools.DotNet;
using Cake.Common.Tools.DotNet.Pack;
using Cake.Frosting;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Tasks;

[TaskName("CreateNuGetPackage")]
[TaskDescription("Create nuget package in the master Release binaries folder")]
public class CreateNuGetPackageTask : FrostingTask<ShatilayaContext> {
    public override bool ShouldRun(ShatilayaContext context) {
        return context.IsMasterOrBranchWithPackages && context.CreateAndPushPackages;
    }

    public override void Run(ShatilayaContext context) {
        var projectErrorsAndInfos = new ErrorsAndInfos();
        IProjectFactory projectFactory = context.Container.Resolve<IProjectFactory>();
        IProjectLogic projectLogic = context.Container.Resolve<IProjectLogic>();
        IProject project = projectFactory.Load(context.SolutionFileFullName, context.SolutionFileFullName.Replace(".slnx", ".csproj"), projectErrorsAndInfos);
        if (!projectLogic.DoAllConfigurationsHaveNuspecs(project)) {
            throw new Exception("The release configuration needs a NuspecFile entry" +
                "\r\n" +
                context.SolutionFileFullName +
                "\r\n" +
                context.SolutionFileFullName.Replace(".slnx", ".csproj"));
        }

        if (projectErrorsAndInfos.Errors.Any()) {
            throw new Exception(projectErrorsAndInfos.ErrorsToString());
        }

        IFolder folder = context.MasterBinReleaseFolder;
        if (!folder.LastWrittenFileFullName().EndsWith("nupkg")) {
            var settings = new DotNetPackSettings {
                Configuration = "Release",
                NoBuild = true,
                NoRestore = true,
                IncludeSymbols = false,
                OutputDirectory = context.MasterBinReleaseFolder.FullName
            };
            context.DotNetPack("./src/" + context.SolutionId + ".csproj", settings);
        }
    }
}
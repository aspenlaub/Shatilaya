using System;
using System.IO;
using System.Linq;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Extensions;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Protch.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Skladasu.Entities;
using Aspenlaub.Net.GitHub.CSharp.Skladasu.Extensions;
using Autofac;
using Cake.Common.Diagnostics;
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
        context.Information("Creating nuget package in the master Release binaries folder");
        var errorsAndInfos = new ErrorsAndInfos();
        IProjectFactory projectFactory = context.Container.Resolve<IProjectFactory>();
        IProjectLogic projectLogic = context.Container.Resolve<IProjectLogic>();
        IProject project = projectFactory.Load(context.SolutionFileFullName, context.SolutionFileFullName.Replace(".slnx", ".csproj"), errorsAndInfos);
        if (!projectLogic.DoAllConfigurationsHaveNuspecs(project)) {
            throw new Exception("The release configuration needs a NuspecFile entry" +
                "\r\n" +
                context.SolutionFileFullName +
                "\r\n" +
                context.SolutionFileFullName.Replace(".slnx", ".csproj"));
        }

        if (errorsAndInfos.Errors.Any()) {
            throw new Exception(errorsAndInfos.ErrorsToString());
        }

        context.Information("All configurations have nuspec files");
        IFolder folder = context.MasterBinReleaseFolder;
        if (folder.LastWrittenFileFullName().EndsWith("nupkg")) {
            context.Information("Last written file name ends with nupkg");
            return;
        }

        string csProjFileFullName = context.RepositoryFolder.FullName + @"\src\" + context.SolutionId + ".csproj";
        var settings = new DotNetPackSettings {
            Configuration = "Release",
            NoBuild = true,
            NoRestore = true,
            IncludeSymbols = false,
            OutputDirectory = context.MasterBinReleaseFolder.FullName,
            Verbosity = DotNetVerbosity.Diagnostic
        };
        context.Information($"Creating package for {csProjFileFullName}");
        context.Information($"Creating package in {settings.OutputDirectory.FullPath.Replace('/', '\\')}");
        context.DotNetPack(csProjFileFullName, settings);

        string lastWrittenFileName = folder.LastWrittenFileFullName();
        if (lastWrittenFileName.EndsWith("nupkg")) {
            context.Information($"Package {lastWrittenFileName} was created");
            return;
        }

        if (!File.Exists(settings.OutputDirectory.FullPath + "/packageicon.png")) {
            throw new Exception($"No package was created. packageicon.png not found in {settings.OutputDirectory.FullPath.Replace('/', '\\')}");
        }

        if (!File.ReadAllText(csProjFileFullName).Contains("IsPackable")) {
            throw new Exception($"No package was created. Add <IsPackable>true</IsPackable> to {csProjFileFullName}");
        }

        throw new Exception("Last written file name should end with nupkg");
    }
}
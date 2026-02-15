using System;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Components;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Extensions;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Autofac;
using Cake.Common.Diagnostics;
using Cake.Common.Tools.MSBuild;
using Cake.Core.Diagnostics;
using Cake.Frosting;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Tasks;

[TaskName("ReleaseBuildCsProjToTemp")]
[TaskDescription("Build solution's main project file in Release into a temporary folder")]
public class ReleaseBuildCsProjToTempTask : FrostingTask<ShatilayaContext> {
    public override void Run(ShatilayaContext context) {
        context.Information("Building solution in Release into a temporary folder");
        string solutionFileFullName = context.SolutionFileFullNameWithinOrOutsideSrc;
        context.Information("Solution is: " + solutionFileFullName);
        string csProjFileFullName = solutionFileFullName.Replace(".slnx", ".csproj");
        if (solutionFileFullName == csProjFileFullName) {
            throw new Exception("Solution and project file are identical");
        }
        context.Information("Project file is: " + csProjFileFullName);
        IFolder solutionFolder = context.SolutionFolderWithinOrOutsideSrc;
        context.Information("Solution folder is: " + solutionFolder.FullName);
        IFolder tempFolder = solutionFolder.SubFolder("temp").SubFolder("bin").SubFolder("Release");
        context.Information($"Output folder is: {tempFolder.FullName}");
        if (tempFolder.Exists()) {
            context.Information("Output folder exists, cleaning up");
            IContainer container = new ContainerBuilder().UsePegh("Shatilaya").Build();
            IFolderDeleter deleter = container.Resolve<IFolderDeleter>();
            deleter.DeleteFolder(tempFolder.ParentFolder());
        }
        tempFolder.CreateIfNecessary();
        tempFolder.CreateIfNecessary();
        context.MSBuild(csProjFileFullName, settings
            => settings
               .SetConfiguration("Release")
               .SetVerbosity(Verbosity.Minimal)
               .WithProperty("Platform", "Any CPU")
               .WithProperty("OutDir", tempFolder.FullName)
        );

    }
}
using Aspenlaub.Net.GitHub.CSharp.Pegh.Extensions;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Shatilaya.Extensions;
using Autofac;
using Cake.Common.Diagnostics;
using Cake.Common.Tools.MSBuild;
using Cake.Frosting;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Tasks;

[TaskName("ReleaseBuildToTemp")]
[TaskDescription("Build solution in Release into a temporary folder")]
[IsDependentOn(typeof(InitializeContextForMsBuildTask))]
public class ReleaseBuildToTempTask : FrostingTask<ShatilayaContext> {
    public override void Run(ShatilayaContext context) {
        context.Information("Building solution in Release into a temporary folder");
        string solutionFileFullName = context.SolutionFileFullNameWithinOrOutsideSrc;
        context.Information("Solution is: " + solutionFileFullName);
        IFolder solutionFolder = context.SolutionFolderWithinOrOutsideSrc;
        context.Information("Solution folder is: " + solutionFolder.FullName);
        IFolder tempFolder = solutionFolder.SubFolder("temp").SubFolder("bin").SubFolder("Release");
        context.Information($"Output folder is: {tempFolder.FullName}");
        if (tempFolder.Exists()) {
            context.Information("Output folder exists, cleaning up");
            IFolderDeleter deleter = context.Container.Resolve<IFolderDeleter>();
            deleter.DeleteFolder(tempFolder.ParentFolder());
        }
        tempFolder.CreateIfNecessary();
        tempFolder.CreateIfNecessary();
        context.MSBuild(solutionFileFullName, settings
            => settings
               .WithShatilayaCommonBuildSettings(context)
               .SetConfiguration("Release")
               .WithProperty("OutDir", tempFolder.FullName)
        );

    }
}
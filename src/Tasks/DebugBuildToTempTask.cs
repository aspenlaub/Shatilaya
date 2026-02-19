using Aspenlaub.Net.GitHub.CSharp.Pegh.Extensions;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Shatilaya.Extensions;
using Autofac;
using Cake.Common.Diagnostics;
using Cake.Common.Tools.MSBuild;
using Cake.Frosting;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Tasks;

[TaskName("DebugBuildToTemp")]
[TaskDescription("Build solution in Debug into a temporary folder")]
[IsDependentOn(typeof(InitializeContextTask))]
public class DebugBuildToTempTask : FrostingTask<ShatilayaContext> {
    public override void Run(ShatilayaContext context) {
        context.Information("Building solution in Debug into a temporary folder");
        string solutionFileFullName = context.SolutionFileFullNameWithinOrOutsideSrc;
        context.Information("Solution is: " + solutionFileFullName);
        IFolder solutionFolder = context.SolutionFolderWithinOrOutsideSrc;
        context.Information("Solution folder is: " + solutionFolder.FullName);
        IFolder tempFolder = solutionFolder.SubFolder("temp").SubFolder("bin").SubFolder("Debug");
        context.Information($"Output folder is: {tempFolder.FullName}");
        if (tempFolder.Exists()) {
            context.Information("Output folder exists, cleaning up");
            IFolderDeleter deleter = context.Container.Resolve<IFolderDeleter>();
            deleter.DeleteFolder(tempFolder.ParentFolder());
        }
        tempFolder.CreateIfNecessary();
        context.MSBuild(solutionFileFullName, settings
            => settings
               .WithShatilayaCommonBuildSettings(context)
               .SetConfiguration("Debug")
               .WithProperty("OutDir", tempFolder.FullName)
        );

    }
}
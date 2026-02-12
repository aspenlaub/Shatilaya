using System.IO;
using Aspenlaub.Net.GitHub.CSharp.Nuclide;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Extensions;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Protch.Interfaces;
using Autofac;
using Cake.Common.Diagnostics;
using Cake.Common.Tools.DotNet;
using Cake.Common.Tools.DotNet.Test;
using Cake.Core.IO.Arguments;
using Cake.Frosting;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Tasks;

[TaskName("RunTestsOnDebugArtifactsToTemp")]
[TaskDescription("Run unit and integration tests on Debug artifacts, log in temporary folder")]
public class RunTestsOnDebugArtifactsToTempTask : FrostingTask<ShatilayaContext> {
    public override void Run(ShatilayaContext context) {
        IContainer container = new ContainerBuilder().UseNuclideProtchGittyAndPegh("Shatilaya").Build();
        IProjectFactory projectFactory = container.Resolve<IProjectFactory>();
        var errorsAndInfos = new ErrorsAndInfos();
        string[] projectFileFullNames = Directory.GetFiles(context.SolutionFolderWithinOrOutsideSrc.FullName, "*Test.csproj");
        IFolder resultsFolder = context.SolutionFolderWithinOrOutsideSrc.SubFolder("TestResults");
        if (resultsFolder.Exists()) {
            IFolderDeleter deleter = context.Container.Resolve<IFolderDeleter>();
            deleter.DeleteFolder(resultsFolder);
        }
        foreach(string projectFileFullName in projectFileFullNames) {
            IProject project = projectFactory.Load(context.SolutionFileFullNameWithinOrOutsideSrc, projectFileFullName, errorsAndInfos);
            string logFileName = @"\TestResults -" + project.ProjectName + ".trx";
            context.Information($"Output file is: {logFileName}");
            string loggerArgValue = "trx;LogFileName=" + logFileName.Replace("\\", "\\\\");
            var loggerArg = new TextArgument($"--logger \"{loggerArgValue}\"");
            var dotNetTestSettings = new DotNetTestSettings {
                    Configuration = "Debug", NoRestore = true, NoBuild = true,
                    ArgumentCustomization = args => {
                        args.Append(loggerArg);
                        return args;
                    }
            };
            context.DotNetTest(projectFileFullName, dotNetTestSettings);
        }
    }
}

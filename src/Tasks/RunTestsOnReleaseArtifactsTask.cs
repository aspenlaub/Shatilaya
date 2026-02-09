using System;
using System.Linq;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Extensions;
using Aspenlaub.Net.GitHub.CSharp.Protch.Interfaces;
using Autofac;
using Cake.Common.Diagnostics;
using Cake.Common.IO;
using Cake.Common.Tools.DotNet;
using Cake.Common.Tools.DotNet.Test;
using Cake.Core.IO;
using Cake.Core.IO.Arguments;
using Cake.Frosting;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Tasks;

[TaskName("RunTestsOnReleaseArtifacts")]
[TaskDescription("Run unit and integration tests on Release artifacts")]
public class RunTestsOnReleaseArtifactsTask : FrostingTask<ShatilayaContext> {
    public override void Run(ShatilayaContext context) {
        context.Information("Running unit and integration tests on Release artifacts");
        IProjectFactory projectFactory = context.Container.Resolve<IProjectFactory>();
        IProjectLogic projectLogic = context.Container.Resolve<IProjectLogic>();
        FilePathCollection projectFiles = context.GetFiles(context.RepositoryFolder.FullName + @"\src\**\*Test.csproj");
        if (projectFiles.Count == 0) {
            throw new Exception("No test projects found");
        }
        foreach (FilePath projectFile in projectFiles) {
            var errorsAndInfos = new ErrorsAndInfos();
            IProject project = projectFactory.Load(context.SolutionFileFullName, projectFile.FullPath, errorsAndInfos);
            if (errorsAndInfos.Errors.Any()) {
                throw new Exception(errorsAndInfos.ErrorsToString());
            }
            if (projectLogic.TargetsOldFramework(project)) {
                throw new Exception("Project targets a .net framework that is no longer supported");
            }
            context.Information("Running tests in " + projectFile.FullPath);
            string logFileName = context.TestResultsFolder + @"\TestResults-" + project.ProjectName + ".trx";
            var textArgument = new TextArgument("--logger \"trx;LogFileName=" + logFileName + "\"");
            var dotNetTestSettings = new DotNetTestSettings {
                Configuration = "Release",
                NoRestore = true,
                NoBuild = true,
                DiagnosticOutput = true,
                Verbosity = DotNetVerbosity.Diagnostic,
                ArgumentCustomization = args => {
                    args.Append(textArgument);
                    return args;
                }
            };
            context.DotNetTest(projectFile.FullPath, dotNetTestSettings);
        }
        context.CleanDirectory(context.TestResultsFolder.FullName);
        context.DeleteDirectory(context.TestResultsFolder.FullName, new DeleteDirectorySettings { Recursive = false, Force = false });
    }
}
using System;
using System.IO;
using System.Linq;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Extensions;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Protch.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Skladasu.Entities;
using Autofac;
using Cake.Common.Diagnostics;
using Cake.Common.Tools.DotNet;
using Cake.Common.Tools.DotNet.Test;
using Cake.Core.IO.Arguments;
using Cake.Frosting;
using IContainer = Autofac.IContainer;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Tasks;

[TaskName("RunTestsOnReleaseArtifactsToTemp")]
[TaskDescription("Run unit and integration tests on Release artifacts, log in temporary folder")]
public class RunTestsOnReleaseArtifactsToTempTask : FrostingTask<ShatilayaContext> {
    public override void Run(ShatilayaContext context) {
        IContainer container = context.Container;
        IProjectFactory projectFactory = container.Resolve<IProjectFactory>();
        var errorsAndInfos = new ErrorsAndInfos();
        var projectFileFullNames = Directory.GetFiles(context.SolutionFolderWithinOrOutsideSrc.FullName, "*Test.csproj", SearchOption.AllDirectories).ToList();
        if (projectFileFullNames.Count == 0) {
            throw new Exception($"No test projects found under \"{context.SolutionFolderWithinOrOutsideSrc.FullName}\"");
        }
        IFolder resultsFolder = context.SolutionFolderWithinOrOutsideSrc.SubFolder("TestResults");
        if (resultsFolder.Exists()) {
            IFolderDeleter deleter = container.Resolve<IFolderDeleter>();
            deleter.DeleteFolder(resultsFolder);
        }
        foreach (string projectFileFullName in projectFileFullNames) {
            IProject project = projectFactory.Load(context.SolutionFileFullNameWithinOrOutsideSrc, projectFileFullName, errorsAndInfos);
            string logFileName = resultsFolder.FullName + @"\TestResults-" + project.ProjectName + ".trx";
            context.Information($"Output file is: {logFileName}");
            string loggerArgValue = "trx;LogFileName=" + logFileName.Replace("\\", "\\\\");
            var loggerArg = new TextArgument($"--logger \"{loggerArgValue}\"");
            var dotNetTestSettings = new DotNetTestSettings {
                Configuration = "Release",
                NoRestore = true,
                NoBuild = true,
                ArgumentCustomization = args => {
                    args.Append(loggerArg);
                    return args;
                }
            };
            context.DotNetTest(projectFileFullName, dotNetTestSettings);
        }
    }
}

using System;
using System.Linq;
using Aspenlaub.Net.GitHub.CSharp.Gitty.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Skladasu.Entities;
using Aspenlaub.Net.GitHub.CSharp.Skladasu.Extensions;
using Autofac;
using Cake.Core.Diagnostics;
using Cake.Frosting;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Tasks;

[TaskName("CheckForVulnerabilities")]
[TaskDescription("Checking nuget package vulnerabilities")]
[IsDependentOn(typeof(InitializeContextTask))]
public class CheckForVulnerabilitiesTask : FrostingTask<ShatilayaContext> {
    private const string _dotNetExecutableFileName = "dotnet";

    public override void Run(ShatilayaContext context) {
        context.Log.Information($"Checking {context.SolutionFolderWithinOrOutsideSrc} for vulnerabilities");
        IProcessRunner runner = context.Container.Resolve<IProcessRunner>();
        var errorsAndInfos = new ErrorsAndInfos();
        string arguments = $"list \"{context.SolutionFileFullNameWithinOrOutsideSrc}\" package --vulnerable --include-transitive";
        runner.RunProcess(_dotNetExecutableFileName, arguments, context.SolutionFolderWithinOrOutsideSrc, errorsAndInfos);
        if (errorsAndInfos.Errors.Any()) {
            throw new Exception(errorsAndInfos.ErrorsToString());
        }

        var givenProjectInfos = errorsAndInfos.Infos.Where(i => i.Contains("given project", StringComparison.InvariantCultureIgnoreCase)).ToList();
        if (givenProjectInfos.Count == 0) {
            throw new Exception("Expected information messages regarding the 'given project'");
        }
        foreach (string info in givenProjectInfos) {
            if (!info.Contains("has no vulnerable", StringComparison.InvariantCultureIgnoreCase)) {
                throw new Exception($"Expected to find 'has no vulnerable' in message {info}");
            }
            context.Log.Information(info);
        }
    }
}
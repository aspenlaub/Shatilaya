using System;
using System.Linq;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Shatilaya.Components;
using Aspenlaub.Net.GitHub.CSharp.Skladasu.Entities;
using Aspenlaub.Net.GitHub.CSharp.Skladasu.Extensions;
using Cake.Frosting;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Tasks;

[TaskName("EnsureSolutionJs")]
[TaskDescription("Create or update solution.js")]
[IsDependentOn(typeof(CleanRestorePullTask))]
public class EnsureSolutionJsTask : AsyncFrostingTask<ShatilayaContext> {
    public override async Task RunAsync(ShatilayaContext context) {
        var errorsAndInfos = new ErrorsAndInfos();
        await SolutionCakeConverter.ConvertSolutionCakeAsync(context.RepositoryFolder, errorsAndInfos);
        if (errorsAndInfos.Errors.Any()) {
            throw new Exception(errorsAndInfos.ErrorsToString());
        }
    }
}

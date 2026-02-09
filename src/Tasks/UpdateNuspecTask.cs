using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Gitty.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Nuclide.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Extensions;
using Autofac;
using Cake.Common.Diagnostics;
using Cake.Frosting;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Tasks;

[TaskName("UpdateNuspec")]
[TaskDescription("Update nuspec if necessary")]
public class UpdateNuspecTask : AsyncFrostingTask<ShatilayaContext> {
    public override async Task RunAsync(ShatilayaContext context) {
        context.Information("Updating nuspec if necessary");
        var errorsAndInfos = new ErrorsAndInfos();
        string headTipIdSha = context.Container.Resolve<IGitUtilities>().HeadTipIdSha(context.RepositoryFolder);
        await context.Container.Resolve<INuSpecCreator>().CreateNuSpecFileIfRequiredOrPresentAsync(true,
            context.SolutionFileFullName, context.CurrentGitBranch,
            new List<string> { headTipIdSha }, errorsAndInfos);
        if (errorsAndInfos.Errors.Any()) {
            throw new Exception(errorsAndInfos.ErrorsToString());
        }
    }
}
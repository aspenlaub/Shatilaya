using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Components;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Seoa.Extensions;
using Aspenlaub.Net.GitHub.CSharp.Skladasu.Entities;
using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Test;

[TestClass]
public class MsBuildToolsCheck  {
    [TestMethod]
    public async Task MsBuildToolsVersionsAreKnown() {
        List<string> knownVersions = [
            "2022", "18"
        ];

        IContainer container = new ContainerBuilder().UsePeghWithoutCsLambdaCompiler("Shatilaya").Build();
        var errorsAndInfos = new ErrorsAndInfos();
        IFolder visualStudioFolder = await container.Resolve<IFolderResolver>().ResolveAsync("$(VisualStudio)", errorsAndInfos);
        Assert.That.ThereWereNoErrors(errorsAndInfos);
        var unknownVersions = Directory.GetDirectories(visualStudioFolder.FullName)
             .Select(v => v.Substring(visualStudioFolder.FullName.Length + 1))
             .Where(v => !knownVersions.Contains(v))
             .ToList();
        Assert.IsEmpty(unknownVersions, $"Unknown version: {unknownVersions[0]}");
        IFolder msBuildToolsFolder = await container.Resolve<IFolderResolver>().ResolveAsync("$(MsBuildTools)", errorsAndInfos);
        Assert.That.ThereWereNoErrors(errorsAndInfos);
        Assert.IsTrue(msBuildToolsFolder.FullName.StartsWith(visualStudioFolder.FullName));

    }
}

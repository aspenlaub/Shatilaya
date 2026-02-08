using System.Collections.Generic;
using System.Linq;
using Aspenlaub.Net.GitHub.CSharp.Nuclide.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Extensions;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.CakeFrosting.Test;

[TestClass]
public class ShatilayaContextTest : ShatilayaCakeFrostingTestBase {
    [TestMethod]
    public void CanCreateShatilayaContext() {
        IFolder folder = PakledTarget.Folder();
        var cakeArguments = new Dictionary<string, string> {
            [ "repository" ] = folder.FullName,
            [ "target" ] = "LittleThings"
        };
        IServiceCollection serviceCollection = new ServiceCollection().AddCakeCoreFakes(
            configureLog: log => log.Verbosity = Verbosity.Diagnostic,
            configureArguments: args => args.SetArguments(cakeArguments.ToLookup(x => x.Key, x => x.Value)));
        ServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();
        ICakeContext context = serviceProvider.GetService<ICakeContext>();
        var shatilayaContext = new ShatilayaContext(context);
        Assert.IsNotNull(shatilayaContext);
        Assert.AreEqual(folder.FullName, shatilayaContext.RepositoryFolder.FullName);
        Assert.AreEqual("LittleThings", shatilayaContext.Target);
        string expectedSolutionFileFullName = folder.SubFolder("src").FullName + @"\Pakled.slnx";
        Assert.AreEqual(expectedSolutionFileFullName, shatilayaContext.SolutionFileFullName);
        Assert.AreEqual(PakledTarget.SolutionId, shatilayaContext.SolutionId);
        IFolder expectedDebugBinFolder = folder.SubFolder(@"src\bin\Debug");
        Assert.AreEqual(expectedDebugBinFolder.FullName, shatilayaContext.DebugBinFolder.FullName);
        IFolder expectedReleaseBinFolder = folder.SubFolder(@"src\bin\Release");
        Assert.AreEqual(expectedReleaseBinFolder.FullName, shatilayaContext.ReleaseBinFolder.FullName);
        Assert.AreEqual(folder.SubFolder("TestResults").FullName, shatilayaContext.TestResultsFolder.FullName);
        Assert.AreEqual(folder.SubFolder("temp").FullName, shatilayaContext.TempFolder.FullName);
        Assert.AreEqual(NugetFeed.AspenlaubLocalFeed, shatilayaContext.MainNugetFeedId);
        IFolder expectedMasterDebugBinFolder = folder.ParentFolder().SubFolder(@"PakledBin\Debug");
        Assert.AreEqual(expectedMasterDebugBinFolder.FullName, shatilayaContext.MasterBinDebugFolder.FullName);
        IFolder expectedMasterReleaseBinFolder = folder.ParentFolder().SubFolder(@"PakledBin\Release");
        Assert.AreEqual(expectedMasterReleaseBinFolder.FullName, shatilayaContext.MasterBinReleaseFolder.FullName);
        IFolder expectedMasterReleaseCandidateBinFolder = folder.ParentFolder().SubFolder(@"PakledBin\ReleaseCandidate");
        Assert.AreEqual(expectedMasterReleaseCandidateBinFolder.FullName, shatilayaContext.MasterReleaseCandidateBinFolder.FullName);
    }
}

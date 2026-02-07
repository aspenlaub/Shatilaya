using System;
using Cake.Common.Tools.NuGet;
using Cake.Common.Tools.NuGet.Restore;
using Cake.Frosting;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.CakeFrosting.Tasks;

[TaskName("Restore")]
[TaskDescription("Restore nuget packages")]
public class RestoreTask : FrostingTask<ShatilayaContext> {
    public override void Run(ShatilayaContext context) {
        string configFile = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\NuGet\nuget.config";
        if (!System.IO.File.Exists(configFile)) {
            throw new Exception(string.Format("Nuget configuration file \"{0}\" not found", configFile));
        }
        context.NuGetRestore(context.SolutionFileFullName, new NuGetRestoreSettings { ConfigFile = configFile });
    }
}
using System.IO;
using System.Linq;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Extensions;
using Cake.Common.Diagnostics;
using Cake.Common.IO;
using Cake.Frosting;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.CakeFrosting.Tasks;

[TaskName("CleanObjectFolders")]
[TaskDescription("Clean object folders")]
public class CleanObjectFoldersTask : FrostingTask<ShatilayaContext> {
    public override void Run(ShatilayaContext context) {
        context.Information("Cleaning object folders");
        foreach (string objFolder in Directory.GetDirectories(context.RepositoryFolder.SubFolder("src").FullName,
                "obj", SearchOption.AllDirectories).ToList()) {
            context.CleanDirectory(objFolder);
            context.DeleteDirectory(objFolder, new DeleteDirectorySettings { Recursive = false, Force = false });
        }
    }
}
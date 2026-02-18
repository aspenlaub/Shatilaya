using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Fusion;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Shatilaya.Components;
using Aspenlaub.Net.GitHub.CSharp.Skladasu.Entities;
using Aspenlaub.Net.GitHub.CSharp.Skladasu.Extensions;
using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Test;

[TestClass]
public class SolutionCakeConverterTest {
    [TestMethod]
    public async Task CanConvertSolutionCakes() {
        IContainer container = FusionContainerBuilder.CreateContainerUsingFusionNuclideProtchAndGitty("Shatilaya");
        var errorsAndInfos = new ErrorsAndInfos();
        string folderName = (await container.Resolve<IFolderResolver>().ResolveAsync("$(GitHub)", errorsAndInfos)).FullName + '\\';
        Assert.IsEmpty(errorsAndInfos.Errors, errorsAndInfos.ErrorsToString());
        var fileNames = Directory.GetFiles(folderName, "solution.cake", SearchOption.AllDirectories).ToList();
        folderName = (await container.Resolve<IFolderResolver>().ResolveAsync("$(GitHub-PkgBranchTest)", errorsAndInfos)).FullName + '\\';
        Assert.IsEmpty(errorsAndInfos.Errors, errorsAndInfos.ErrorsToString());
        fileNames.AddRange(Directory.GetFiles(folderName, "solution.cake", SearchOption.AllDirectories).ToList());
        folderName = (await container.Resolve<IFolderResolver>().ResolveAsync("$(GitHub-DotnetNine)", errorsAndInfos)).FullName + '\\';
        Assert.IsEmpty(errorsAndInfos.Errors, errorsAndInfos.ErrorsToString());
        fileNames.AddRange(Directory.GetFiles(folderName, "solution.cake", SearchOption.AllDirectories).ToList());
        folderName = (await container.Resolve<IFolderResolver>().ResolveAsync("$(GitHub-DotnetTen)", errorsAndInfos)).FullName + '\\';
        Assert.IsEmpty(errorsAndInfos.Errors, errorsAndInfos.ErrorsToString());
        fileNames.AddRange(Directory.GetFiles(folderName, "solution.cake", SearchOption.AllDirectories).ToList());
        foreach (string fileName in fileNames) {
            errorsAndInfos = new ErrorsAndInfos();
            var folder = new Folder(fileName.Substring(0, fileName.LastIndexOf('\\')));
            await SolutionCakeConverter.ConvertSolutionCakeAsync(folder, errorsAndInfos);
            Assert.IsEmpty(errorsAndInfos.Errors, errorsAndInfos.ErrorsToString());
        }
    }
}

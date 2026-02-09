using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Fusion;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Extensions;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Test;

[TestClass]
public class SolutionCakeConverterTest {
    [TestMethod]
    public async Task CanConvertSolutionCakes() {
        const string startTag = "var solutionSpecialSettingsDictionary = new Dictionary<string, string> {";
        const string noTag = "solutionSpecialSettingsDictionary = new Dictionary<string, string>();";
        IContainer container = FusionContainerBuilder.CreateContainerUsingFusionNuclideProtchAndGitty("Shatilaya");
        var errorsAndInfos = new ErrorsAndInfos();
        string folder = (await container.Resolve<IFolderResolver>().ResolveAsync("$(GitHub)", errorsAndInfos)).FullName + '\\';
        Assert.IsEmpty(errorsAndInfos.Errors, errorsAndInfos.ErrorsToString());
        var fileNames = Directory.GetFiles(folder, "solution.cake", SearchOption.AllDirectories).ToList();
        folder = (await container.Resolve<IFolderResolver>().ResolveAsync("$(GitHub-PkgBranchTest)", errorsAndInfos)).FullName + '\\';
        Assert.IsEmpty(errorsAndInfos.Errors, errorsAndInfos.ErrorsToString());
        fileNames.AddRange(Directory.GetFiles(folder, "solution.cake", SearchOption.AllDirectories).ToList());
        folder = (await container.Resolve<IFolderResolver>().ResolveAsync("$(GitHub-DotnetNine)", errorsAndInfos)).FullName + '\\';
        Assert.IsEmpty(errorsAndInfos.Errors, errorsAndInfos.ErrorsToString());
        fileNames.AddRange(Directory.GetFiles(folder, "solution.cake", SearchOption.AllDirectories).ToList());
        folder = (await container.Resolve<IFolderResolver>().ResolveAsync("$(GitHub-DotnetTen)", errorsAndInfos)).FullName + '\\';
        Assert.IsEmpty(errorsAndInfos.Errors, errorsAndInfos.ErrorsToString());
        fileNames.AddRange(Directory.GetFiles(folder, "solution.cake", SearchOption.AllDirectories).ToList());
        foreach (string fileName in fileNames) {
            string contents = await File.ReadAllTextAsync(fileName);
            int pos = contents.IndexOf(startTag, StringComparison.OrdinalIgnoreCase);
            if (pos >= 0) {
                int pos2 = contents.IndexOf("};", pos + 1, StringComparison.OrdinalIgnoreCase);
                if (pos2 < 0) {
                    continue;
                }

                contents = contents.Substring(pos + startTag.Length - 1, pos2 - pos - startTag.Length + 2).Trim();
                contents = contents.Replace("{ \"", "\"");
                contents = contents.Replace("\", \"", "\": \"");
                contents = contents.Replace("\",  \"", "\": \"");
                contents = contents.Replace("\" }", "\",");
                pos = contents.LastIndexOf("\",", StringComparison.OrdinalIgnoreCase);
                if (pos < 0) {
                    continue;
                }

                contents = contents.Substring(0, pos + 1) + contents.Substring(pos + 2);
            } else if (contents.Contains(noTag, StringComparison.OrdinalIgnoreCase)) {
                contents = "{}";
            } else {
                continue;
            }

            string newFileName = fileName.Replace("solution.cake", "solution.json");
            if (fileName == newFileName) {
                continue;
            }
            if (File.Exists(newFileName) && await File.ReadAllTextAsync(newFileName) == contents) {
                continue;
            }

            try {
                Dictionary<string, string> _ = JsonSerializer.Deserialize<Dictionary<string, string>>(contents);
            } catch {
                continue;
            }

            await File.WriteAllTextAsync(newFileName, contents);
        }
    }
}

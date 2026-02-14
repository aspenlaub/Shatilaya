using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Components;

public class SolutionCakeConverter {
    private const string _startTag = "var solutionSpecialSettingsDictionary = new Dictionary<string, string> {";
    private const string _noTag = "solutionSpecialSettingsDictionary = new Dictionary<string, string>();";

    public static async Task ConvertSolutionCakeAsync(IFolder repositoryFolder, IErrorsAndInfos errorsAndInfos) {
        string solutionCakeFullFileName = repositoryFolder.FullName + @"\\solution.cake";
        if (!File.Exists(solutionCakeFullFileName)) {
            return;
        }

        string solutionJsonFileName = solutionCakeFullFileName.Replace("solution.cake", "solution.json");
        if (solutionCakeFullFileName == solutionJsonFileName) {
            errorsAndInfos.Errors.Add("Solution cake and js file names are identical");
            return;
        }

        string contents = await File.ReadAllTextAsync(solutionCakeFullFileName);
        int pos = contents.IndexOf(_startTag, StringComparison.OrdinalIgnoreCase);
        if (pos >= 0) {
            int pos2 = contents.IndexOf("};", pos + 1, StringComparison.OrdinalIgnoreCase);
            if (pos2 < 0) {
                errorsAndInfos.Errors.Add("Could not find dictionary in solution.cake");
                return;
            }

            contents = contents.Substring(pos + _startTag.Length - 1, pos2 - pos - _startTag.Length + 2).Trim();
            contents = contents.Replace("{ \"", "\"");
            contents = contents.Replace("\", \"", "\": \"");
            contents = contents.Replace("\",  \"", "\": \"");
            contents = contents.Replace("\" }", "\",");
            pos = contents.LastIndexOf("\",", StringComparison.OrdinalIgnoreCase);
            if (pos < 0) {
                errorsAndInfos.Errors.Add("Could not find dictionary in solution.cake");
                return;
            }

            contents = contents.Substring(0, pos + 1) + contents.Substring(pos + 2);
        } else if (contents.Contains(_noTag, StringComparison.OrdinalIgnoreCase)) {
            contents = "{}";
        } else {
            errorsAndInfos.Errors.Add("Could not find dictionary in solution.cake");
            return;
        }

        if (File.Exists(solutionJsonFileName) && await File.ReadAllTextAsync(solutionJsonFileName) == contents) {
            return;
        }

        try {
            Dictionary<string, string> _ = JsonSerializer.Deserialize<Dictionary<string, string>>(contents);
        } catch {
            errorsAndInfos.Errors.Add("Could not deserialize solution.js new contents");
            return;
        }

        await File.WriteAllTextAsync(solutionJsonFileName, contents);
    }
}
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Test {
    public class CakeBuildUtilities {
        public static void CopyLatestScriptFromShatilayaSolution(TestTargetFolder testTargetFolder) {
            ILatestBuildCakeScriptProvider latestBuildCakeScriptProvider = new LatestBuildCakeScriptProvider();
            var latestScriptWithoutBuildCakeCheck = latestBuildCakeScriptProvider.GetLatestBuildCakeScript();
            Assert.IsTrue(latestScriptWithoutBuildCakeCheck.Length > 120);
            Assert.IsTrue(latestScriptWithoutBuildCakeCheck.Contains("#load \"solution.cake\""));
            latestScriptWithoutBuildCakeCheck = UseLocalShatilayaAssemblies(latestScriptWithoutBuildCakeCheck);

            var currentScriptFileName = testTargetFolder.FullName() + @"\" + "build.cake";
            if (File.Exists(currentScriptFileName)) {
                var currentScript = File.ReadAllText(currentScriptFileName);
                if (Regex.Replace(latestScriptWithoutBuildCakeCheck, @"\s", "") == Regex.Replace(currentScript, @"\s", "")) { return; }
            }

            File.WriteAllText(currentScriptFileName, latestScriptWithoutBuildCakeCheck);
        }

        public static string UseLocalShatilayaAssemblies(string latestScriptWithoutBuildCakeCheck) {
            const string addShatilaya = @"#addin nuget:https://www.aspenlaub.net/nuget/?package=Aspenlaub.Net.GitHub.CSharp.Shatilaya";
            Assert.IsTrue(latestScriptWithoutBuildCakeCheck.Contains(addShatilaya));
            var assemblyLocation = Assembly.GetExecutingAssembly().Location;
            assemblyLocation = assemblyLocation.Substring(0, assemblyLocation.LastIndexOf('\\'));
            var addLocalShatilaya = Directory.GetFiles(assemblyLocation, "*.dll")
                .Where(assembly => assembly.EndsWith("Aspenlaub.Net.GitHub.CSharp.Shatilaya.dll"))
                .Select(assembly => $"#reference \"{assembly}\"").ToList();
            latestScriptWithoutBuildCakeCheck = latestScriptWithoutBuildCakeCheck.Replace(addShatilaya, string.Join("\r\n", addLocalShatilaya));
            return latestScriptWithoutBuildCakeCheck;
        }
    }
}

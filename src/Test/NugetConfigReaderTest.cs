using System.Collections.Generic;
using System.IO;
using System.Linq;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Shatilaya.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Test {
    [TestClass]
    public class NugetConfigReaderTest {
        protected string NugetConfigFileName = Path.GetTempPath() + nameof(NugetConfigReaderTest) + ".config";
        protected string Source = "hypothetical" + "source.net";
        protected string ApiKey = "thisisnotanapikey";

        [TestCleanup]
        public void Cleanup() {
            if (!File.Exists(NugetConfigFileName)) { return; }

            File.Delete(NugetConfigFileName);
        }

        [TestMethod]
        public void CanGetApiKey() {
            CreateNugetConfig();
            var sut = new NugetConfigReader();
            var errorsAndInfos = new ErrorsAndInfos();
            Assert.AreEqual(ApiKey, sut.GetApiKey(NugetConfigFileName, Source, errorsAndInfos));
            Assert.IsFalse(errorsAndInfos.Errors.Any(), errorsAndInfos.ErrorsPlusRelevantInfos());
            Assert.AreEqual("", sut.GetApiKey(NugetConfigFileName, Source + "t", errorsAndInfos));
            Assert.IsTrue(errorsAndInfos.Errors.Any(e => e.Contains("No apikey was found for this source")));
        }

        protected void CreateNugetConfig() {
            var contents = new List<string> {
                "<?xml version=\"1.0\" encoding=\"utf-8\"?>",
                "<configuration>",
                "<packageSources>",
                "<add key=\"" + Source + "\" value=\"https://www.hypothericalsource.net/nuget\" />",
                "</packageSources>",
                "<apikeys>",
                "<add key=\"https://www.hypothericalsource.net/nuget\" value=\"" + ApiKey + "\" />",
                "</apikeys>",
                "</configuration>"
            };

            File.WriteAllLines(NugetConfigFileName, contents);
        }
    }
}

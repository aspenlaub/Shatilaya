using System.IO;
using System.Reflection;
using Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya {
    public class LatestBuildCakeScriptProvider : ILatestBuildCakeScriptProvider {
        public string GetLatestBuildCakeScript() {
            var assembly = Assembly.GetExecutingAssembly();
            var stream = assembly.GetManifestResourceStream("Aspenlaub.Net.GitHub.CSharp.Shatilaya.build.cake");
            if (stream == null) { return ""; }

            var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }
    }
}

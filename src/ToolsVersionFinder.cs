using Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya {
    public class ToolsVersionFinder : IToolsVersionFinder { // ToDo: remove if proven obsolete
        protected IComponentProvider ComponentProvider;

        public ToolsVersionFinder(IComponentProvider componentProvider) {
            ComponentProvider = componentProvider;
        }

        public int LatestAvailableToolsVersion() {
            if (!ComponentProvider.ExecutableFinder.HaveVs7()) {
                return 14;
            }

            var toolsVersion = 14;
            for (var version = 15; ComponentProvider.ExecutableFinder.FindMsTestExe(version) != "" || ComponentProvider.ExecutableFinder.FindVsTestExe(version) != ""; version++) {
                toolsVersion = version;
            }

            return toolsVersion;
        }
    }
}

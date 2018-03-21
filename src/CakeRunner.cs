using System.IO;
using Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Shatilaya.Properties;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya {
    public class CakeRunner : ICakeRunner {
        protected IComponentProvider ComponentProvider;

        public CakeRunner(IComponentProvider componentProvider) {
            ComponentProvider = componentProvider;
        }

        public void CallCake(string cakeExeFullName, string scriptFileFullName, ErrorsAndInfos errorsAndInfos) {
            if (!File.Exists(cakeExeFullName)) {
                errorsAndInfos.Errors.Add(string.Format(Resources.FileNotFound, cakeExeFullName));
                return;
            }

            if (!File.Exists(scriptFileFullName)) {
                errorsAndInfos.Errors.Add(string.Format(Resources.FileNotFound, scriptFileFullName));
                return;
            }

            var scriptFileFolderFullName = scriptFileFullName.Substring(0, scriptFileFullName.LastIndexOf('\\'));
            var runner = ComponentProvider.ProcessRunner;
            runner.RunProcess(cakeExeFullName, "\"" + scriptFileFullName + "\" -mono", scriptFileFolderFullName, errorsAndInfos);
        }
    }
}

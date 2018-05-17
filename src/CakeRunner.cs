using System.IO;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces;
using IComponentProvider = Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces.IComponentProvider;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya {
    public class CakeRunner : ICakeRunner {
        protected IComponentProvider ComponentProvider;

        public CakeRunner(IComponentProvider componentProvider) {
            ComponentProvider = componentProvider;
        }

        public void CallCake(string cakeExeFullName, string scriptFileFullName, IErrorsAndInfos errorsAndInfos) {
            CallCake(cakeExeFullName, scriptFileFullName, "", errorsAndInfos);
        }

        public void CallCake(string cakeExeFullName, string scriptFileFullName, string target, IErrorsAndInfos errorsAndInfos) {
            if (!File.Exists(cakeExeFullName)) {
                errorsAndInfos.Errors.Add(string.Format(Properties.Resources.FileNotFound, cakeExeFullName));
                return;
            }

            if (!File.Exists(scriptFileFullName)) {
                errorsAndInfos.Errors.Add(string.Format(Properties.Resources.FileNotFound, scriptFileFullName));
                return;
            }

            var scriptFileFolderFullName = scriptFileFullName.Substring(0, scriptFileFullName.LastIndexOf('\\'));
            var runner = ComponentProvider.ProcessRunner;
            var arguments = "\"" + scriptFileFullName + "\" -mono";
            if (target != "") {
                arguments = arguments + " -target=" + target;
            }
            runner.RunProcess(cakeExeFullName, arguments, scriptFileFolderFullName, errorsAndInfos);
        }
    }
}

using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Shatilaya.Properties;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya {
    public class CakeRunner : ICakeRunner {
        public void CallCake(string cakeExeFullName, string scriptFileFullName, out IList<string> messages, out IList<string> errors) {
            messages = new List<string>();
            errors = new List<string>();

            if (!File.Exists(cakeExeFullName)) {
                errors.Add(string.Format(Resources.FileNotFound, cakeExeFullName));
                return;
            }

            if (!File.Exists(scriptFileFullName)) {
                errors.Add(string.Format(Resources.FileNotFound, scriptFileFullName));
                return;
            }

            var scriptFileFolderFullName = scriptFileFullName.Substring(0, scriptFileFullName.LastIndexOf('\\'));
            var cmd = new Process {
                StartInfo = {
                    FileName = cakeExeFullName,
                    Arguments = "\"" + scriptFileFullName + "\" -mono",
                    WorkingDirectory = scriptFileFolderFullName,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                }
            };
            cmd.Start();
            while (!cmd.StandardError.EndOfStream) {
                errors.Add(cmd.StandardError.ReadLine());
            }
            while (!cmd.StandardOutput.EndOfStream) {
                messages.Add(cmd.StandardOutput.ReadLine());
            }
        }
    }
}

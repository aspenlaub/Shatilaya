using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Shatilaya.Properties;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya {
    public class CakeRunner : ICakeRunner {
        public void CallCake(string cakeExeFullName, string scriptFileFullName, out IList<string> cakeMessages, out IList<string> cakeErrors) {
            var messages = new List<string>();
            var errors = new List<string>();
            cakeMessages = messages;
            cakeErrors = errors;

            if (!File.Exists(cakeExeFullName)) {
                errors.Add(string.Format(Resources.FileNotFound, cakeExeFullName));
                return;
            }

            if (!File.Exists(scriptFileFullName)) {
                errors.Add(string.Format(Resources.FileNotFound, scriptFileFullName));
                return;
            }

            var scriptFileFolderFullName = scriptFileFullName.Substring(0, scriptFileFullName.LastIndexOf('\\'));
            using (var process = CreateProcess(cakeExeFullName, scriptFileFullName, scriptFileFolderFullName)) {
                var outputWaitHandle = new AutoResetEvent(false);
                var errorWaitHandle = new AutoResetEvent(false);
                process.OutputDataReceived += (sender, e) => {
                    OnDataReceived(e, outputWaitHandle, messages);
                };
                process.ErrorDataReceived += (sender, e) => {
                    OnDataReceived(e, errorWaitHandle, errors);
                };
                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.WaitForExit();
                outputWaitHandle.WaitOne();
                errorWaitHandle.WaitOne();
            }
        }

        private static void OnDataReceived(DataReceivedEventArgs e, EventWaitHandle waitHandle, ICollection<string> messages) {
            if (e.Data == null) {
                waitHandle.Set();
                return;
            }

            messages.Add(e.Data);
        }

        private static Process CreateProcess(string cakeExeFullName, string scriptFileFullName, string scriptFileFolderFullName) {
            return new Process {
                StartInfo = {
                    FileName = cakeExeFullName,
                    Arguments = "\"" + scriptFileFullName + "\" -mono",
                    WorkingDirectory = scriptFileFolderFullName,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                }
            };
        }
    }
}

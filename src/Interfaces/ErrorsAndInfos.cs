using System.Collections.Generic;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces {
    public class ErrorsAndInfos {
        public List<string> Errors, Infos;

        public ErrorsAndInfos() {
            Errors = new List<string>();
            Infos = new List<string>();
        }
    }
}
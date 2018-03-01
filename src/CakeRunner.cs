using System.Collections.Generic;
using Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya {
    public class CakeRunner : ICakeRunner {
        public bool CallCake(string cakeExeFullName, string scriptFileFullName, out List<string> errors) {
            errors = new List<string>();
            return false;
        }
    }
}

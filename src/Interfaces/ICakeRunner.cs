using System.Collections.Generic;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces {
    public interface ICakeRunner {
        /// <summary>
        /// Call cake and run a specified script, return errors
        /// </summary>
        /// <param name="cakeExeFullName"></param>
        /// <param name="scriptFileFullName"></param>
        /// <param name="errors"></param>
        /// <returns></returns>
        bool CallCake(string cakeExeFullName, string scriptFileFullName, out List<string> errors);
    }
}

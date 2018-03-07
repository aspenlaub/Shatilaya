using System.Collections.Generic;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces {
    public interface ICakeRunner {
        /// <summary>
        /// Call cake and run build.cake in a specified folder, return errors
        /// </summary>
        /// <param name="cakeExeFullName"></param>
        /// <param name="scriptFileFullName"></param>
        /// <param name="messages"></param>
        /// <param name="errors"></param>
        void CallCake(string cakeExeFullName, string scriptFileFullName, out IList<string> messages, out IList<string> errors);
    }
}

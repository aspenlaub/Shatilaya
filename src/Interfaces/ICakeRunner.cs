using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces {
    public interface ICakeRunner {
        /// <summary>
        /// Call cake and run build.cake in a specified folder, return errors
        /// </summary>
        /// <param name="cakeExeFullName"></param>
        /// <param name="scriptFileFullName"></param>
        /// <param name="errorsAndInfos"></param>
        void CallCake(string cakeExeFullName, string scriptFileFullName, IErrorsAndInfos errorsAndInfos);
    }
}

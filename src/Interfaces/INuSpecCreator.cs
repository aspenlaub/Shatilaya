using System.Xml.Linq;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces {
    public interface INuSpecCreator {
        void CreateNuSpecFileIfRequiredOrPresent(bool required, string solutionFileFullName, IErrorsAndInfos errorsAndInfos);
        XDocument CreateNuSpec(string solutionFileFullName, IErrorsAndInfos errorsAndInfos);
    }
}

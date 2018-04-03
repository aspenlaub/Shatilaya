using System.Collections.Generic;
using System.Xml.Linq;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces {
    public interface INuSpecCreator {
        void CreateNuSpecFileIfRequiredOrPresent(bool required, string solutionFileFullName, IErrorsAndInfos errorsAndInfos);
        void CreateNuSpecFileIfRequiredOrPresent(bool required, string solutionFileFullName, IList<string> tags, IErrorsAndInfos errorsAndInfos);

        XDocument CreateNuSpec(string solutionFileFullName, IList<string> tags, IErrorsAndInfos errorsAndInfos);
    }
}

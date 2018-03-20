using System.Xml.Linq;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces {
    public interface INuSpecCreator {
        void CreateNuSpecFileIfRequiredOrPresent(bool required, string solutionFileFullName, ErrorsAndInfos errorsAndInfos);
        XDocument CreateNuSpec(string solutionFileFullName, ErrorsAndInfos errorsAndInfos);
    }
}

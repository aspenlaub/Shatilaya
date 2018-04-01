using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces {
    public interface INugetPackageToPushFinder {
        void FindPackageToPush(IFolder packageFolderWithBinaries, string solutionFileFullName, out string packageFileFullName, out string feedUrl, out string apiKey, IErrorsAndInfos errorsAndInfos);
    }
}

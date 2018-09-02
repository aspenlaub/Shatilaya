using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.PeghStandard.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces {
    public interface INugetPackageToPushFinder {
        Task<IPackageToPush> FindPackageToPushAsync(IFolder packageFolderWithBinaries, IFolder repositoryFolder, string solutionFileFullName, IErrorsAndInfos errorsAndInfos);
    }
}

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces {
    public interface IFolderUpdater {
        void UpdateFolder(IFolder sourceFolder, IFolder destinationFolder, FolderUpdateMethod folderUpdateMethod, ErrorsAndInfos errorsAndInfos);
    }

    public enum FolderUpdateMethod {
        Assemblies = 1
    }
}

using Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Entities {
    public class Folder : IFolder {
        public string FullName { get; }

        public Folder(string fullName) {
            if (fullName != "" && fullName[fullName.Length - 1] == '\\') {
                fullName = fullName.Substring(0, fullName.Length - 1);
            }

            FullName = fullName;
        }
    }
}

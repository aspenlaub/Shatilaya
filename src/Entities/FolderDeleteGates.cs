﻿using Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Entities {
    public class FolderDeleteGates : IFolderDeleteGates {
        public bool FolderNameIsLongEnough { get; set; }
        public bool EndsWithObj { get; set; }
        public bool NotTooManyFilesInFolder { get; set; }
        public bool CTemp { get; set; }
        public bool IsGitCheckOutFolder { get; set; }
        public bool UserTemp { get; set; }
    }
}

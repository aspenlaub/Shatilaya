﻿namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces {
    public interface IProcessRunner {
        void RunProcess(string executableFullName, string arguments, string workingFolder, ErrorsAndInfos errorsAndInfos);
    }
}
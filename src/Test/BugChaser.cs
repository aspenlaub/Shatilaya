using System.IO;
using Path = System.IO.Path;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Extensions;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Test;

public class BugChaser {
    public static void SaveChaseLog(string s) {
        IFolder logFolder = new Folder(Path.GetTempPath()).SubFolder("AspenlaubLogs").SubFolder("Bug Chase Log");
        logFolder.CreateIfNecessary();
        string logFileName;
        int n = 100;
        do {
            logFileName = logFolder.FullName + $"\\ChaseLog_{n}.txt";
            n++;
        } while (File.Exists(logFileName));
        File.WriteAllText(logFileName, s);
    }
}

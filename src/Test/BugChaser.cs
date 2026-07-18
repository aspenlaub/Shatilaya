using System.IO;
using Path = System.IO.Path;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Extensions;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Test;

public class BugChaser {
    public static void MakeChaseLogEntry(string s) {
        IFolder logFolder = new Folder(Path.GetTempPath()).SubFolder("AspenlaubLogs").SubFolder("Bug Chase Log");
        logFolder.CreateIfNecessary();
        string logFileName = logFolder.FullName + @"\ChaseLog.txt";
        if (File.Exists(logFileName)) {
            File.AppendAllText(logFileName, "\r\n" + s);
        } else {
            File.WriteAllText(logFileName, s);
        }
    }
}

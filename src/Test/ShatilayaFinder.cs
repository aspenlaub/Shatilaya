using System.IO;
using System.Reflection;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Test;

public class ShatilayaFinder {
    public static void FindShatilaya(out string executableFullName, out Folder workingFolder) {
        string assemblyLocation = Assembly.GetExecutingAssembly().Location;
        Assert.IsFalse(string.IsNullOrEmpty(assemblyLocation));
        workingFolder = new Folder(assemblyLocation.Substring(0, assemblyLocation.LastIndexOf('\\')));
        string[] executableFullNames = Directory.GetFiles(workingFolder.FullName, "*Shatilaya*.exe");
        Assert.HasCount(1, executableFullNames);
        executableFullName = executableFullNames[0];
    }
}

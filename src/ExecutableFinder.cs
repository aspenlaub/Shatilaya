﻿using System.IO;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Extensions;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces;
using Microsoft.Win32;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya {
    public class ExecutableFinder : IExecutableFinder { // ToDo: remove if proven obsolete
        protected const string RegistryKeyTemplate = @"SOFTWARE\WOW6432Node\Microsoft\VisualStudio\{0}.0\Setup";
        protected const string ValueName = "Dbghelp_path";
        protected const string PathToMsTest = @"\mstest.exe";
        protected const string PathToVsTest = @"\CommonExtensions\Microsoft\TestWindow\vstest.console.exe";
        protected const string Vs7RegistryKey = @"SOFTWARE\WOW6432Node\Microsoft\VisualStudio\SxS\VS7";
        protected const string Vs7SubFolder = @"Common7\IDE";

        public string FindMsTestExe(int toolsVersionNumber) {
            var folder = ToolsFolder(toolsVersionNumber);
            if (folder == null) { return ""; }

            var fullFileName = folder.FullName + PathToMsTest;
            return folder.Exists() && File.Exists(fullFileName) ? fullFileName : "";
        }

        public string FindVsTestExe(int toolsVersionNumber) {
            var folder = ToolsFolder(toolsVersionNumber);
            if (folder == null) { return ""; }

            var fullFileName = folder.FullName + PathToVsTest;
            return folder.Exists() && File.Exists(fullFileName) ? fullFileName : "";
        }

        public bool HaveVs7() {
            return Registry.LocalMachine.OpenSubKey(Vs7RegistryKey) != null;
        }

        protected IFolder ToolsFolder(int toolsVersionNumber) {
            var key = Registry.LocalMachine.OpenSubKey(Vs7RegistryKey);
            if (key != null) {
                var name = $"{toolsVersionNumber}.0";
                try {
                    return new Folder((string) key.GetValue(name)).SubFolder(Vs7SubFolder);
                } catch {
                    return null;
                }
            }

            key = Registry.LocalMachine.OpenSubKey(string.Format(RegistryKeyTemplate, toolsVersionNumber));
            return key == null ? null : new Folder((string) key.GetValue(ValueName));
        }
    }
}

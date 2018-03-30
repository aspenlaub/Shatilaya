﻿using System.Xml;
using Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Entities {
    public class PropertyGroup : IPropertyGroup {
        public XmlNode XmlNode { get; set; }
        public string AssemblyName { get; set; }
        public string Condition { get; set; }
        public string RootNamespace { get; set; }
        public string IntermediateOutputPath { get; set; }
        public string OutputPath { get; set; }
        public string UseVsHostingProcess { get; set; }
        public string GenerateBuildInfoConfigFile { get; set; }
    }
}
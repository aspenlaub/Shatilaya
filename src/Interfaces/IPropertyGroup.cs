﻿namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces {
    public interface IPropertyGroup {
        string AssemblyName { get; set; }
        string Condition { get; set; }
        string RootNamespace { get; set; }
        string IntermediateOutputPath { get; set; }
        string OutputPath { get; set; }
        string UseVsHostingProcess { get; set; }
        string GenerateBuildInfoConfigFile { get; set; }
        string AppendTargetFrameworkToOutputPath { get; set; }
        string AllowUnsafeBlocks { get; set; }
        string NuspecFile { get; set; }
    }
}

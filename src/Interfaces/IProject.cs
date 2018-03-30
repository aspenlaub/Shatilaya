using System.Collections.Generic;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces {
    public interface IProject {
        string ProjectFileFullName { get; set; }
        string ProjectName { get; set; }
        string ToolsVersion { get; set;  }
        string RootNamespace { get; set; }

        IList<string> ReferencedDllFiles { get; }
        IList<IPropertyGroup> PropertyGroups { get; }
    }
}

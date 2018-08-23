﻿using System.Linq;
using Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya {
    public class ProjectLogic : IProjectLogic {
        public bool IsANetStandardOrCoreProject(IProject project) {
            return project.TargetFramework.Contains("netstandard") || project.TargetFramework.Contains("netcore");
        }

        public bool DoAllNetStandardOrCoreConfigurationsHaveNuspecs(IProject project) {
            return !IsANetStandardOrCoreProject(project)
                   || project.PropertyGroups.Where(propertyGroup
                           => propertyGroup.Condition.Contains("Release"))
                       .All(propertyGroup => propertyGroup.NuspecFile == project.ProjectName + ".nuspec");
        }
    }
}

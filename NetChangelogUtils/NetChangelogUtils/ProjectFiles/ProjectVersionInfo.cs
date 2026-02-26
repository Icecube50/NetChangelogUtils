using NetChangelogUtils.ProjectFiles.Version;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetChangelogUtils.ProjectFiles
{
    public class ProjectVersionInfo
    {
        public string ProjectPath { get; set; }

        public SemanticVersion? Version { get; set; }
        public SemanticVersion? AssemblyVersion { get; set; }
        public SemanticVersion? FileVersion { get; set; }
        public string ProductName { get; set; }

        public bool HasVersionInfo => Version != null || FileVersion != null || AssemblyVersion != null;

        public override string ToString()
        {
            return $"{ProjectPath} | Version={Version}, Assembly={AssemblyVersion}, File={FileVersion}";
        }
    }
}

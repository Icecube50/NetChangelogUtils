using NetChangelogUtils.ProjectFiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetChangelogUtils.Git
{
    public class ProductReleaseContext
    {
        public ProductReleaseContext(ProjectVersionInfo versionInfo)
        {
            VersionInfo = versionInfo;
        }

        public ProjectVersionInfo VersionInfo { get; }
        public string LastTag { get; set; }
        public List<GitCommitInfo> Commits { get; set; } = new();
    }
}

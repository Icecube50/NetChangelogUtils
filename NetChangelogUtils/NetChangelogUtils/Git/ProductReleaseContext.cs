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
        public ProductReleaseContext(ProjectInfo project)
        {
            Project = project;
        }

        public ProjectInfo Project { get; }
        public string LastTag { get; set; }
        public List<ReleaseEntry> Commits { get; set; } = new();
    }
}

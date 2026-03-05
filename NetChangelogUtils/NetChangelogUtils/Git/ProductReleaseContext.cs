using NetChangelogUtils.ProjectFiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibGit2Sharp;

namespace NetChangelogUtils.Git
{
    public class ProductReleaseContext
    {
        public ProductReleaseContext(ProjectInfo project, Commit? lastTagCommit)
        {
            Project = project;
            LastTagCommit = lastTagCommit;
        }

        public readonly ProjectInfo Project;
        public readonly Commit? LastTagCommit;
        public readonly List<ReleaseEntry> Commits = new();
    }
}

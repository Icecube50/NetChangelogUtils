using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetChangelogUtils.Git
{
    public class GitCommitInfo
    {
        public string Sha { get; set; }
        public string Message { get; set; }
        public string Author { get; set; }
        public DateTimeOffset Date { get; set; }

        public List<ReleaseEntry> Entries { get; set; } = new();
    }

    public class ReleaseEntry
    {
        public string Category { get; set; }
        public List<string> Scopes { get; set; } = new();
        public string Description { get; set; }
    }
}

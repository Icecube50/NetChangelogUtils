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

        public string Category { get; set; }
        public List<string> Scopes { get; set; } = new();

        public override string ToString()
            => $"{Sha[..7]} | {Date:yyyy-MM-dd} | {Category}, {string.Join(',', Scopes)} | {Message}";
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetChangelogUtils.Git
{
    public static class CommitParser
    {
        public static void ParseCommit(GitCommitInfo commit)
        {
            var firstLine = commit.Message.Split('\n')[0];

            var match = System.Text.RegularExpressions.Regex.Match(
                firstLine,
                @"^(?<category>\w+)(<(?<scopes>[^>]+)>)?\s*(?<message>.*)$");

            if (!match.Success)
                return;

            commit.Category = match.Groups["category"].Value;

            if (match.Groups["scopes"].Success)
            {
                commit.Scopes = match.Groups["scopes"]
                    .Value
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.Trim())
                    .ToList();
            }
        }
    }
}

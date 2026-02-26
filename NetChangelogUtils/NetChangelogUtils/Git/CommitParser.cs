using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NetChangelogUtils.Git
{
    public static class CommitParser
    {
        private static readonly Regex EntryRegex =
            new Regex(
               @"^(?<category>\w+)(<(?<scopes>[^>]+)>)?\s+(?<description>.+)$",
               RegexOptions.Compiled);

        public static void ParseCommit(GitCommitInfo commit)
        {
            var lines = commit.Message
                .Replace("\r\n", "\n")
                .Split('\n');

            ReleaseEntry currentEntry = null;

            foreach (var rawLine in lines)
            {
                var line = rawLine.Trim();

                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var match = EntryRegex.Match(line);

                if (match.Success)
                {
                    // Start new entry
                    currentEntry = new ReleaseEntry
                    {
                        Category = match.Groups["category"].Value,
                        Description = match.Groups["description"].Value.Trim()
                    };

                    if (match.Groups["scopes"].Success)
                    {
                        currentEntry.Scopes = match.Groups["scopes"]
                            .Value
                            .Split(',', StringSplitOptions.RemoveEmptyEntries)
                            .Select(s => s.Trim())
                            .ToList();
                    }

                    commit.Entries.Add(currentEntry);
                }
                else if (currentEntry != null)
                {
                    // Continuation of previous description
                    currentEntry.Description += " " + line;
                }
            }
        }
    }
}

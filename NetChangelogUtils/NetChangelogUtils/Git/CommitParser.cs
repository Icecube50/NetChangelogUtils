using NetChangelogUtils.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using LibGit2Sharp;

namespace NetChangelogUtils.Git
{
    public static class CommitParser
    {
        private static Regex BuildEntryRegex(
             IEnumerable<string> keywords,
             IEnumerable<string> validScopes)
        {
            var keywordPattern = string.Join("|",
                keywords.Select(Regex.Escape));

            var scopePattern = string.Join("|",
                validScopes.Select(Regex.Escape));

            var pattern =
                $@"^(?<category>{keywordPattern})" +
                $@"(\s*<(?<scopes>(?:{scopePattern})(?:\s*,\s*(?:{scopePattern}))*)>\s*)?" +
                $@"\s+(?<description>[\s\S]+)$";

            return new Regex(
                pattern,
                RegexOptions.Compiled | RegexOptions.IgnoreCase);
        }

        public static GitCommitInfo ParseCommit(Commit commit, VersioningConfig config, IEnumerable<string> scopes)
        {
            var lines = commit.Message
                .Replace("\r\n", "\n")
                .Split('\n');

            ReleaseEntry currentEntry = null;

            var commitInfo = new GitCommitInfo
            {
               Sha     = commit.Sha,
               Message = commit.Message,
               Author  = commit.Author.Name,
               Date    = commit.Author.When
            };

            foreach (var rawLine in lines)
            {
                var line = rawLine.Trim();

                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var match = BuildEntryRegex(config.Keywords.Select(it => it.Keyword),
                                            scopes)
                           .Match(line);

                if (match.Success)
                {
                    // Start new entry
                    currentEntry = new ReleaseEntry
                    {
                        Category = match.Groups["category"].Value,
                        Description = match.Groups["description"].Value.Trim()
                    };

                    var keyword = config.ResolveKeywordRule(currentEntry.Category);
                    if (keyword == null)
                        throw new InvalidOperationException($"Keyword {currentEntry.Category} is not defined.");
                    

                    if (match.Groups["scopes"].Success)
                    {
                        currentEntry.Scopes = match.Groups["scopes"]
                            .Value
                            .Split(',', StringSplitOptions.RemoveEmptyEntries)
                            .Select(s => s.Trim())
                                                   .Select(s => s.ToLowerInvariant())
                            .ToList();
                    }

                    commitInfo.Entries.Add(currentEntry);
                }
                else if (currentEntry != null)
                {
                    // Continuation of previous description
                    currentEntry.Description += " " + line;
                }
            }

            return commitInfo;
        }
    }
}

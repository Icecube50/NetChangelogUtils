using LibGit2Sharp;
using NetChangelogUtils.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace NetChangelogUtils.Git
{
    public class GitHistoryService
    {
        private readonly Repository _repo;
        private readonly CliOptions _options;
        private readonly ChangelogUtilsConfig _config;

        public GitHistoryService(CliOptions options, Repository repo, ChangelogUtilsConfig config)
        {
            _repo = repo;
            _options = options;
            _config = config;
        }

        public void GetHistoryForProduct(ProductReleaseContext context, IEnumerable<string> productNames)
        {
            var lastTagCommit = GetLastProductTagCommit(context.Project.ProductName);

            context.LastTag = lastTagCommit?.Sha;

            var filter = new CommitFilter
            {
                IncludeReachableFrom = _repo.Head,
                ExcludeReachableFrom = lastTagCommit
            };

            var commits = _repo.Commits.QueryBy(filter);

            foreach (var commit in commits)
            {
                var info = new GitCommitInfo
                {
                    Sha = commit.Sha,
                    Message = commit.Message,
                    Author = commit.Author.Name,
                    Date = commit.Author.When
                };

                CommitParser.ParseCommit(info, _config.Versioning, productNames);

                context.Commits.AddRange(GetEntriesForProduct(info, context.Project.ProductName, _config.ScopeAliases));
            }
        }

        private Commit GetLastProductTagCommit(string productName)
        {
            var pattern = $"^{productName}_v\\d+\\.\\d+\\.\\d+$";

            var tag = _repo.Tags
                .Where(t =>
                    System.Text.RegularExpressions.Regex
                        .IsMatch(t.FriendlyName, pattern))
                .Select(t => t.Target.Peel<Commit>())
                .Where(c => _repo.Head.Commits.Contains(c))
                .OrderByDescending(c => c.Author.When)
                .FirstOrDefault();

            return tag;
        }

        private IEnumerable<ReleaseEntry> GetEntriesForProduct(
        GitCommitInfo commit,
        string productName,
        IEnumerable<ScopeAlias> aliases)
        {
            foreach (var entry in commit.Entries)
            {
                // No scope = affects all
                if (entry.Scopes == null || !entry.Scopes.Any())
                {
                    if(!_options.IgnoreUnscoped)
                        yield return entry;
                    continue;
                }

                if (entry.Scopes.Any(s =>
                    string.Equals(s, productName, StringComparison.OrdinalIgnoreCase)))
                {
                    yield return entry;
                    continue;
                }

                if (entry.Scopes.Any(s =>
                    aliases.Where(a => string.Equals(a.Alias, s, StringComparison.OrdinalIgnoreCase))
                    .Select(a => a.Products)
                    .Any(p => string.Equals(s, productName, StringComparison.OrdinalIgnoreCase))))
                {
                    yield return entry;
                }
            }
        }
    }
}

using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetChangelogUtils.Git
{
    public class GitHistoryService
    {
        private readonly Repository _repo;
        private readonly CliOptions _options;

        public GitHistoryService(CliOptions options, Repository repo)
        {
            _repo = repo;
            _options = options;
        }

        public void GetHistoryForProduct(ProductReleaseContext context)
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

                CommitParser.ParseCommit(info);

                context.Commits.AddRange(GetEntriesForProduct(info, context.Project.ProductName));
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
        string productName)
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
                }
            }
        }
    }
}

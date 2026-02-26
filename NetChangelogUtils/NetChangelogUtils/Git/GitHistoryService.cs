using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetChangelogUtils.Git
{
    public class GitHistoryService : IDisposable
    {
        private readonly Repository _repo;

        public GitHistoryService(string repositoryPath)
        {
            _repo = new Repository(repositoryPath);
        }

        public void GetHistoryForProduct(ProductReleaseContext context)
        {
            var lastTagCommit = GetLastProductTagCommit(context.VersionInfo.ProductName);

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

                if (CommitAffectsProduct(info, context.VersionInfo.ProductName))
                {
                    context.Commits.Add(info);
                }
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

        private bool CommitAffectsProduct(
            GitCommitInfo commit,
            string productName)
        {
            // No scope → affects all
            if (commit.Scopes == null || !commit.Scopes.Any())
                return true;

            return commit.Scopes
                .Any(scope => string.Equals(
                    scope,
                    productName,
                    StringComparison.OrdinalIgnoreCase));
        }

        public void Dispose()
        {
            _repo.Dispose();
        }
    }
}

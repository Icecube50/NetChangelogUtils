using LibGit2Sharp;
using NetChangelogUtils.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetChangelogUtils.ProjectFiles;
using static System.Formats.Asn1.AsnWriter;

namespace NetChangelogUtils.Git
{
    public static class GitHistoryExplorer
    {
        public static IEnumerable<ProductReleaseContext> GetHistoryForProducts(Repository repo, CliOptions options, ChangelogUtilsConfig config,
                                                 IList<ProjectInfo> projects)
        {
           var releaseContexts = new List<ProductReleaseContext>();
           foreach(var prj in projects)
           {
              var lastTagCommit = GetLastProductTagCommit(repo, prj.TagName);
              releaseContexts.Add(new ProductReleaseContext(prj, lastTagCommit));
           }

           var earliestTagCommit = releaseContexts.Select(it => it.LastTagCommit)
                                                    .Where(c => c != null)
                                                    .OrderBy(c => c.Author.When)
                                                    .FirstOrDefault();

            var filter = new CommitFilter
            {
                IncludeReachableFrom = repo.Head,
                ExcludeReachableFrom = earliestTagCommit
            };

            var commits = repo.Commits.QueryBy(filter);

            var scope = new List<string>();
            scope.AddRange(projects.Select(it => it.ProductName));
            scope.AddRange(config.ScopeAliases.Select(it => it.Alias));

            foreach (var commit in commits)
            {
                var info = CommitParser.ParseCommit(commit, config.Versioning, scope);

                foreach (var context in releaseContexts)
                {
                   // Skip commits older than that product's last tag
                   if (context.LastTagCommit      != null &&
                       commit.Author.When <= context.LastTagCommit.Author.When)
                      continue;

                   context.Commits.AddRange(GetEntriesForProduct(
                                                                 options,
                                                                 info,
                                                                 context.Project.ProductName,
                                                                 config.ScopeAliases));
                }
            }

            return releaseContexts;
        }

        private static Commit GetLastProductTagCommit(Repository repo, string tagName)
        {
            var pattern = $"^{tagName}_v\\d+\\.\\d+\\.\\d+$";

            var tag = repo.Tags
                .Where(t =>
                    System.Text.RegularExpressions.Regex
                        .IsMatch(t.FriendlyName, pattern))
                .Select(t => t.Target.Peel<Commit>())
                .Where(c => repo.ObjectDatabase.CalculateHistoryDivergence(repo.Head.Tip, c) != null)
                .OrderByDescending(c => c.Author.When)
                .FirstOrDefault();

            return tag;
        }
       
        private static IEnumerable<ReleaseEntry> GetEntriesForProduct(
           CliOptions options,
        GitCommitInfo commit,
        string productName,
        IEnumerable<ScopeAlias> aliases)
        {
            foreach (var entry in commit.Entries)
            {
                // No scope = affects all
                if (entry.Scopes == null || !entry.Scopes.Any())
                {
                    if(!options.IgnoreUnscoped)
                        yield return entry;
                    continue;
                }

                if (entry.Scopes.Any(s =>  ProjectInfo.CompareNames(s, productName)))
                {
                    yield return entry;
                    continue;
                }


                if (entry.Scopes.Any(s =>
                    aliases.Where(a => ProjectInfo.CompareNames(s, a.Alias))
                    .Select(a => a.Products)
                    .Any(p => p.Any(it => ProjectInfo.CompareNames(it, productName)))))
                {
                    yield return entry;
                }
            }
        }
    }
}

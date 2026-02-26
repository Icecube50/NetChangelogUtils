using LibGit2Sharp;
using NetChangelogUtils.Changelog;
using NetChangelogUtils.Git;
using NetChangelogUtils.ProjectFiles;
using NetChangelogUtils.Version;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetChangelogUtils
{
    public class NetChangelogService
    {
        public void Run(CliOptions options)
        {
            try
            {
                if (!Directory.Exists(options.Path))
                    throw new InvalidOperationException($"Invalid project path {options.Path}");

                var repoPath = Repository.Discover(options.Path);
                if (string.IsNullOrEmpty(repoPath))
                    throw new InvalidOperationException("Project not inside a git repository");

                using var repo = new Repository(repoPath);
                EnsureRepositoryIsClean(repo);

                var discovery = new ProjectDiscoveryService();
                var projects = discovery.Discover(options.Path);
                if (projects.Count == 0)
                    throw new InvalidOperationException("No valid project filed discovered");

                var products = GetProductReleaseContexts(options, repo, projects);

                var release = new ReleaseService(repo, new VersionStrategy(), new ChangelogGenerator());
                release.Release(options, products);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR | {ex.Message}");
            }
        }

        private static IEnumerable<ProductReleaseContext> GetProductReleaseContexts(CliOptions options, Repository repo, IEnumerable<ProjectInfo> projects)
        {
            var history = new GitHistoryService(options, repo);
            foreach (var project in projects)
            {
                var context = new ProductReleaseContext(project);
                history.GetHistoryForProduct(context);
                yield return context;
            }
        }

        private static void EnsureRepositoryIsClean(Repository repo)
        {
            var status = repo.RetrieveStatus();

            if (status.IsDirty)
            {
                throw new InvalidOperationException(
                    "Repository has uncommitted changes. Commit or stash before releasing.");
            }
        }
    }
}

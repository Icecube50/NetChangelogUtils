using LibGit2Sharp;
using NetChangelogUtils.Changelog;
using NetChangelogUtils.Config;
using NetChangelogUtils.Git;
using NetChangelogUtils.ProjectFiles;
using NetChangelogUtils.Version;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
              Console.WriteLine(options);
              Console.WriteLine($"Analyzing {options.Path}");

              // Load config or use default
              var config = ConfigLoader.LoadConfig(options);

              if(!Directory.Exists(options.Path))
                 throw new InvalidOperationException($"Invalid project path {options.Path}");

              // Find git repository
              var repoPath = Repository.Discover(options.Path);
              if(string.IsNullOrEmpty(repoPath))
                 throw new InvalidOperationException("Project not inside a git repository");

              // Ensure repository is in safe state
              using var repo = new Repository(repoPath);
              EnsureRepositoryIsClean(repo);

              var projects  = ProjectDiscovery.RunOn(options.Path);

              if(projects.Count == 0)
                 throw new InvalidOperationException("No valid project filed discovered");

              var products = GetProductReleaseContexts(options, repo, projects, config).ToList();

              Console.WriteLine("----- PREVIEWS -----");
              var updateService = new UpdateService(repo, new VersionStrategy(config.Versioning), new ChangelogGenerator(config), options);
              updateService.Update(products);
           }
           catch(Exception ex)
           {
              Console.WriteLine($"ERROR | {ex.Message}");
           }
        }

        private static IEnumerable<ProductReleaseContext> GetProductReleaseContexts(
            CliOptions options, Repository repo, IEnumerable<ProjectInfo> projects, ChangelogUtilsConfig config)
        {
            return GitHistoryExplorer.GetHistoryForProducts(repo, options, config, projects.ToList());
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

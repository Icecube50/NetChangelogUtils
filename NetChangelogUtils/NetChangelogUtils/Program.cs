using NetChangelogUtils.Changelog;
using NetChangelogUtils.Git;
using NetChangelogUtils.ProjectFiles;
using NetChangelogUtils.ProjectFiles.Version;

namespace NetChangelogUtils
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var path = Console.ReadLine();
            if (!Directory.Exists(path))
                return;

            var discovery = new ProjectDiscoveryService();
            var projects = discovery.Discover(path);
            var products = GetProductReleaseContexts(path, projects);

            var release = new ReleaseService(new VersionStrategy(), new ChangelogGenerator());
            foreach (var product in products)
            {
                release.ReleaseProduct(product, new CliOptions { DryRun = true });
            }
            
        }

        private static IEnumerable<ProductReleaseContext> GetProductReleaseContexts(string path, IEnumerable<ProjectVersionInfo> projects) 
        {
            using var history = new GitHistoryService(path);
            foreach (var project in projects)
            {
                var context = new ProductReleaseContext(project);
                history.GetHistoryForProduct(context);
                yield return context;
            }
        }
    }
}

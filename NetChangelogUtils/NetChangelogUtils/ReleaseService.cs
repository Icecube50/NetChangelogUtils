using LibGit2Sharp;
using NetChangelogUtils.Changelog;
using NetChangelogUtils.Git;
using NetChangelogUtils.ProjectFiles.Version;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetChangelogUtils
{
    public class ReleaseService
    {
        private readonly VersionStrategy _versionStrategy;
        private readonly ChangelogGenerator _changelog;

        public ReleaseService(
            VersionStrategy versionStrategy,
            ChangelogGenerator changelog)
        {
            _versionStrategy = versionStrategy;
            _changelog = changelog;
        }

        public void ReleaseProduct(
            ProductReleaseContext context,
            CliOptions options)
        {
            Console.WriteLine($"Detected {context.VersionInfo.ProductName}: ");
            Console.WriteLine($"\tVersion {context.VersionInfo.Version} ->  {_versionStrategy
                .CalculateNextVersion(context.VersionInfo.Version, context.Commits)}");
            Console.WriteLine($"\tFile Version {context.VersionInfo.FileVersion} ->  {_versionStrategy
                .CalculateNextVersion(context.VersionInfo.FileVersion, context.Commits)}");
            Console.WriteLine($"\tAssembly Version {context.VersionInfo.AssemblyVersion} ->  {_versionStrategy
                .CalculateNextVersion(context.VersionInfo.AssemblyVersion, context.Commits)}");
            Console.WriteLine();

            var changelog = _changelog.Generate(context);
            Console.WriteLine(changelog);
            Console.WriteLine();

            if (options.DryRun)
            {
                Console.WriteLine("DRY RUN — no changes applied.");
                return;
            }

            //_projectManager.UpdateVersion(context.ProductName, nextVersion.ToString());

            //CreateGitTag(context.ProductName, nextVersion.ToString());
        }

        private void CreateGitTag(string productName, string version)
        {
            // create tag: Product_v1.2.3
        }
    }
}

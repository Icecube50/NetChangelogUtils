using LibGit2Sharp;
using NetChangelogUtils.Changelog;
using NetChangelogUtils.Git;
using NetChangelogUtils.Version;
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
        private readonly Repository _repo;

        public ReleaseService( Repository repo,
            VersionStrategy versionStrategy,
            ChangelogGenerator changelog)
        {
            _repo = repo;
            _versionStrategy = versionStrategy;
            _changelog = changelog;
        }

        public void Release(CliOptions options, IEnumerable<ProductReleaseContext> context)
        {
            var releasePlans = CreateReleasePlans(context);

            if (options.DryRun)
            {
                CheckTags(releasePlans);
                return;
            }

            ApplyReleasePlans(releasePlans);
            var commit = CreateReleaseCommit(releasePlans);
            CreateTags(releasePlans, commit);
        }

        private IEnumerable<ReleasePlan> CreateReleasePlans(
            IEnumerable<ProductReleaseContext> context)
        {
            foreach (var contextItem in context)
            {
                yield return new ReleasePlan(contextItem, _versionStrategy, _changelog);
            }
        }

        private void ApplyReleasePlans(IEnumerable<ReleasePlan> plans)
        {
            foreach (var plan in plans)
                plan.Apply();
        }

        private Commit CreateReleaseCommit(IEnumerable<ReleasePlan> plans)
        {
            var signature = _repo.Config.BuildSignature(DateTimeOffset.Now);

            var message = BuildReleaseCommitMessage(plans);

            return _repo.Commit(message, signature, signature);
        }

        private string BuildReleaseCommitMessage(IEnumerable<ReleasePlan> plans)
        {
            var sb = new StringBuilder();
            sb.AppendLine("chore: release");
            sb.AppendLine();

            foreach (var plan in plans)
            {
                sb.AppendLine(
                    $"- {plan.Context.Project.ProductName} v{plan.ChangelogVersion()}");
            }

            return sb.ToString();
        }

        private void CreateTags(IEnumerable<ReleasePlan> plans, Commit commit)
        {
            foreach (var plan in plans)
            {
                var tagName = $"{plan.Context.Project.ProductName}_v{plan.ChangelogVersion()}";

                if (_repo.Tags[tagName] != null)
                    throw new InvalidOperationException(
                        $"Tag {tagName} already exists.");

                var signature = _repo.Config.BuildSignature(DateTimeOffset.Now);

                _repo.ApplyTag(
                    tagName,
                    commit.Sha,
                    signature,
                    $"Release {plan.Context.Project.ProductName} v{plan.ChangelogVersion()}");
            }
        }

        private void CheckTags(IEnumerable<ReleasePlan> plans)
        {
            foreach (var plan in plans)
            {
                var tagName = $"{plan.Context.Project.ProductName}_v{plan.ChangelogVersion()}";

                if (_repo.Tags[tagName] != null)
                {
                    Console.WriteLine($"Tag {tagName} already exists.");
                    continue;
                }

                Console.WriteLine($"Tag {tagName} can be created.");
            }
        }

        private class ReleasePlan
        {
            public ReleasePlan(ProductReleaseContext context, VersionStrategy strategy, ChangelogGenerator changelog)
            {
                Context = context;
                NewVersion = strategy.CalculateNextVersion(context.Project.Version, context.Commits);
                NewAssemblyVersion = strategy.CalculateNextVersion(context.Project.AssemblyVersion, context.Commits);
                NewFileVersion = strategy.CalculateNextVersion(context.Project.FileVersion, context.Commits);

                Console.WriteLine($"Detected {context.Project.ProductName}: ");
                if(NewVersion != null)
                    Console.WriteLine($"\tVersion {context.Project.Version} ->  {NewVersion}");

                if (NewFileVersion != null)
                    Console.WriteLine($"\tFile Version {context.Project.FileVersion} ->  {NewFileVersion}");

                if (NewAssemblyVersion != null)
                    Console.WriteLine($"\tAssembly Version {context.Project.AssemblyVersion} ->  {NewAssemblyVersion}");
                Console.WriteLine();

                Changelog = changelog.Generate(context.Project.ProductName, ChangelogVersion(), context.Commits);
                Console.WriteLine(Changelog);
                Console.WriteLine();
            }

            public SemanticVersion? NewVersion { get; }
            public SemanticVersion? NewAssemblyVersion { get; }
            public SemanticVersion? NewFileVersion { get; }
            public string Changelog { get; }
            public ProductReleaseContext Context { get; } 
            public SemanticVersion ChangelogVersion()
            {
                if(NewVersion != null)
                    return NewVersion;

                if (NewFileVersion != null)
                    return NewFileVersion;

                if (NewAssemblyVersion != null)
                    return NewAssemblyVersion;

                throw new InvalidOperationException("Has to define at least one version to be able to create release.");
            }

            public void Apply()
            {
                Context.Project.WriteChangelog(Changelog);
                Context.Project.UpdateCsproj(NewVersion, NewFileVersion, NewAssemblyVersion);
            }
        }
    }
}

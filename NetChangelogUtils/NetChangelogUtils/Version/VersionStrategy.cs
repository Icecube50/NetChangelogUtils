using NetChangelogUtils.Config;
using NetChangelogUtils.Git;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetChangelogUtils.Version
{
    public class VersionStrategy
    {
        private readonly VersioningConfig _config;
        public VersionStrategy(VersioningConfig config)
        {
            _config = config;
        }

        public SemanticVersion? CalculateNextVersion(
            SemanticVersion? currentVersion,
            IEnumerable<ReleaseEntry> commits)
        {
            if (currentVersion == null)
                return null;

            var highestBump = DetermineHighestBump(commits);

            return highestBump switch
            {
                VersionBump.Major => currentVersion.BumpMajor(),
                VersionBump.Minor => currentVersion.BumpMinor(),
                VersionBump.Patch => currentVersion.BumpPatch(),
                _ => currentVersion
            };
        }

        private VersionBump DetermineHighestBump(IEnumerable<ReleaseEntry> commits)
        {
            VersionBump result = VersionBump.None;

            foreach (var commit in commits)
            {
                var rule = _config.ResolveKeywordRule(commit.Category);
                if (rule != null
                && rule.Bump > result)
                    result = rule.Bump;
            }

            return result;
        }
    }
}

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
                var bump = commit.Category?.ToLower() switch
                {
                    "breaking" => VersionBump.Major,
                    "feat" => VersionBump.Minor,
                    "fix" => VersionBump.Patch,
                    _ => VersionBump.Patch
                };

                if (bump > result)
                    result = bump;
            }

            return result;
        }
    }
}

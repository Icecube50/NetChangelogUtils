using NetChangelogUtils.Git;
using NetChangelogUtils.Version;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetChangelogUtils.Changelog
{
    public class ChangelogGenerator
    {
        public string Generate(string product, SemanticVersion version, IEnumerable<ReleaseEntry> commits)
        {
            var grouped = commits
                .GroupBy(c => c.Category?.ToLower() ?? "other")
                .ToDictionary(g => g.Key, g => g.ToList());

            var sb = new StringBuilder();

            sb.AppendLine($"# {product}");
            sb.AppendLine();
            sb.AppendLine($"## v{version} ({DateTime.UtcNow:yyyy-MM-dd})");
            sb.AppendLine();

            AppendSection(sb, "Breaking Changes", grouped, "breaking");
            AppendSection(sb, "Features", grouped, "feat");
            AppendSection(sb, "Fixes", grouped, "fix");

            return sb.ToString();
        }

        private void AppendSection(
            StringBuilder sb,
            string title,
            Dictionary<string, List<ReleaseEntry>> grouped,
            string key)
        {
            if (!grouped.ContainsKey(key))
                return;

            sb.AppendLine($"### {title}");
            foreach (var commit in grouped[key])
            {
                var message = ExtractCleanMessage(commit.Description);
                sb.AppendLine($"- {message}");
            }
            sb.AppendLine();
        }

        private string ExtractCleanMessage(string message)
        {
            var firstLine = message.Split('\n')[0];
            var idx = firstLine.IndexOf('>');
            return idx >= 0 ? firstLine[(idx + 1)..].Trim() : firstLine;
        }
    }
}

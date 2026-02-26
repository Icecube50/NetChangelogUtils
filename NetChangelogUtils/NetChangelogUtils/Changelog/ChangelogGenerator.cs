using NetChangelogUtils.Git;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetChangelogUtils.Changelog
{
    public class ChangelogGenerator
    {
        public string Generate(ProductReleaseContext context)
        {
            var grouped = context.Commits
                .GroupBy(c => c.Category?.ToLower() ?? "other")
                .ToDictionary(g => g.Key, g => g.ToList());

            var sb = new StringBuilder();

            sb.AppendLine($"# {context.VersionInfo.ProductName}");
            sb.AppendLine();
            sb.AppendLine($"## v{context.VersionInfo.Version} ({DateTime.UtcNow:yyyy-MM-dd})");
            sb.AppendLine();

            AppendSection(sb, "Breaking Changes", grouped, "breaking");
            AppendSection(sb, "Features", grouped, "feat");
            AppendSection(sb, "Fixes", grouped, "fix");

            return sb.ToString();
        }

        private void AppendSection(
            StringBuilder sb,
            string title,
            Dictionary<string, List<GitCommitInfo>> grouped,
            string key)
        {
            if (!grouped.ContainsKey(key))
                return;

            sb.AppendLine($"### {title}");
            foreach (var commit in grouped[key])
            {
                var message = ExtractCleanMessage(commit.Message);
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

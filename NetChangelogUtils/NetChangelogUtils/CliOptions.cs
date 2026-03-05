using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetChangelogUtils
{
    public class CliOptions
    {
        public bool DryRun { get; set; }
        public bool IgnoreUnscoped { get; set; }
        public string Path { get; set; }
        public string ConfigFilePath { get; set; }

        /// <inheritdoc />
        public override string ToString()
        {
           var sb = new StringBuilder();
           sb.AppendLine("Running with these options:");
           sb.AppendLine($"\tDry-Run: {DryRun}");
           sb.AppendLine($"\tIgnore Unscoped: {IgnoreUnscoped}");
           sb.AppendLine($"\tProject: {Path}");
           sb.AppendLine($"\tConfig: {ConfigFilePath}");
           return sb.ToString();
        }
    }
}

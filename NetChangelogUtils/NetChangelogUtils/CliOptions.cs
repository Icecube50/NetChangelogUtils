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
        public bool PushTags { get; set; }
        public string Path { get; set; }
        public string ConfigFilePath { get; set; }
    }
}

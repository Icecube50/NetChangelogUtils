using NetChangelogUtils.Version;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetChangelogUtils.Config
{
    public class ChangelogUtilsConfig
    {
        public VersioningConfig Versioning { get; set; } = new();
        public List<ScopeAlias> ScopeAliases { get; set; } = new();
    }

    public class VersioningConfig
    {
        public VersionBump DefaultStrategy { get; set; } = VersionBump.Patch;
        public List<KeywordRule> Keywords { get; set; } = new();

        public KeywordRule? ResolveKeywordRule(string keyword)
        {
            return Keywords
                .FirstOrDefault(k =>
                    string.Equals(k.Keyword, keyword,
                        StringComparison.OrdinalIgnoreCase));
        }
    }

    public class KeywordRule
    {
        public string Keyword { get; set; }
        public VersionBump Bump { get; set; } = VersionBump.None;
        public string ChangelogSection { get; set; }
    }

    public class ScopeAlias
    {
        public string Alias { get; set; }
        public List<string> Products { get; set; } = new();
    }
}

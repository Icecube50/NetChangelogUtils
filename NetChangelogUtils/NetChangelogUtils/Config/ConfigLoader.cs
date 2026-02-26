using NetChangelogUtils.Version;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace NetChangelogUtils.Config
{
    public static class ConfigLoader
    {
        public static ChangelogUtilsConfig LoadConfig(CliOptions options)
        {
            if (!File.Exists(options.ConfigFilePath))
                return DefaultConfig;

            var json = File.ReadAllText(options.ConfigFilePath);
            return JsonSerializer.Deserialize<ChangelogUtilsConfig>(
                json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                })!;
        }

        private static ChangelogUtilsConfig DefaultConfig = new ChangelogUtilsConfig()
        {
            Versioning = new VersioningConfig()
            {
                DefaultStrategy = VersionBump.Patch,
                Keywords = [
                    new KeywordRule(){
                        Keyword = "Fix",
                        Bump = VersionBump.Patch,
                        ChangelogSection = "Fixes"
                    },
                    new KeywordRule(){
                        Keyword = "Feat",
                        Bump = VersionBump.Minor,
                        ChangelogSection = "Features"
                    },
                    new KeywordRule(){
                        Keyword = "Break",
                        Bump = VersionBump.Major,
                        ChangelogSection = "Breaking Changes"
                    },
                    ]
            },
            ScopeAliases = []
        };
    }
}

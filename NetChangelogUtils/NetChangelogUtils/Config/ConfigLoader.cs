using NetChangelogUtils.Version;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace NetChangelogUtils.Config
{
    public static class ConfigLoader
    {
       public static ChangelogUtilsConfig LoadConfig(CliOptions options)
       {
          if(!File.Exists(options.ConfigFilePath))
          {
             var defaultConf = DefaultConfig;
             using var file = File.Open(options.ConfigFilePath, FileMode.Create);
             var jsonOptions = new JsonSerializerOptions
             {
                PropertyNameCaseInsensitive = true,
                WriteIndented               = true,
             };
             jsonOptions.Converters.Add(new JsonStringEnumConverter());
             JsonSerializer.Serialize(file, defaultConf, jsonOptions);
             return defaultConf;
          }
          else
          {
             var json = File.ReadAllText(options.ConfigFilePath);
             var jsonOptions = new JsonSerializerOptions
             {
                PropertyNameCaseInsensitive = true,
             };
             jsonOptions.Converters.Add(new JsonStringEnumConverter());
             return JsonSerializer.Deserialize<ChangelogUtilsConfig>(json,
                                                                     jsonOptions)!;
          }
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

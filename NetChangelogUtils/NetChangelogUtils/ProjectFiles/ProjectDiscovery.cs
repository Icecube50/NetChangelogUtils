using NetChangelogUtils.Version;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace NetChangelogUtils.ProjectFiles
{
    public static class ProjectDiscovery
    {
        public static List<ProjectInfo> RunOn(string rootDirectory)
        {
            var results = new List<ProjectInfo>();

            var projectFiles = Directory.GetFiles(rootDirectory, "*.csproj", SearchOption.AllDirectories);

            foreach (var project in projectFiles)
            {
                var info = new ProjectInfo
                {
                    ProjectPath = project
                };

                ExtractFromCsproj(info);
                if (!info.HasVersionInfo)
                    continue;

                results.Add(info);
                Console.WriteLine($"Discovered: {info}");
            }

            Console.WriteLine();
            return results;
        }

        private static void ExtractFromCsproj(ProjectInfo info)
        {
            var doc = XDocument.Load(info.ProjectPath);
            var propertyGroups = doc.Descendants("PropertyGroup");

            info.Version = SemanticVersion.CreateFrom(propertyGroups.Elements("Version").FirstOrDefault()?.Value);
            info.AssemblyVersion = SemanticVersion.CreateFrom(propertyGroups.Elements("AssemblyVersion").FirstOrDefault()?.Value);
            info.FileVersion = SemanticVersion.CreateFrom(propertyGroups.Elements("FileVersion").FirstOrDefault()?.Value);
          
            var product = propertyGroups.Elements("Product").FirstOrDefault()?.Value;
            if (string.IsNullOrEmpty(product))
                product = Path.GetFileNameWithoutExtension(info.ProjectPath);
            info.ProductName = product;
        }
    }
}

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
    public class ProjectDiscoveryService
    {
        public List<ProjectInfo> Discover(string rootDirectory)
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
            }

            return results;
        }

        private void ExtractFromCsproj(ProjectInfo info)
        {
            var doc = XDocument.Load(info.ProjectPath);
            var propertyGroups = doc.Descendants("PropertyGroup");

            info.Version = SemanticVersion.CreateFrom(propertyGroups.Elements("Version").FirstOrDefault()?.Value);
            info.AssemblyVersion = SemanticVersion.CreateFrom(propertyGroups.Elements("AssemblyVersion").FirstOrDefault()?.Value);
            info.FileVersion = SemanticVersion.CreateFrom(propertyGroups.Elements("FileVersion").FirstOrDefault()?.Value);
          
            var product = propertyGroups.Elements("ProductName").FirstOrDefault()?.Value;
            if (string.IsNullOrEmpty(product))
                product = Path.GetFileNameWithoutExtension(info.ProjectPath);
            info.ProductName = product;
        }
    }
}

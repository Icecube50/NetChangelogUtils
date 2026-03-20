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
            var propertyGroups = GetPropertyGroups(doc);

            var ns = propertyGroups.FirstOrDefault()?.Name.Namespace ?? XNamespace.None;

            info.Version = SemanticVersion.CreateFrom(
                                                      propertyGroups.Elements(ns + "Version").FirstOrDefault()?.Value);

            info.AssemblyVersion = SemanticVersion.CreateFrom(
                                                              propertyGroups.Elements(ns + "AssemblyVersion").FirstOrDefault()?.Value);

            info.FileVersion = SemanticVersion.CreateFrom(
                                                          propertyGroups.Elements(ns + "FileVersion").FirstOrDefault()?.Value);

            var project = Path.GetFileNameWithoutExtension(info.ProjectPath);

            var product = propertyGroups.Elements(ns + "Product").FirstOrDefault()?.Value;
            if (string.IsNullOrEmpty(product))
               product = project;

            info.ProductName = product;
            info.ProjectName = project;
        }

        private static IEnumerable<XElement> GetPropertyGroups(XDocument doc)
        {
           if (doc.Root == null)
              return Enumerable.Empty<XElement>();

           XNamespace ns = doc.Root.Name.Namespace;
           return doc.Descendants(ns + "PropertyGroup");
        }
    }
}

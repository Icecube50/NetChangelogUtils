using NetChangelogUtils.Version;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace NetChangelogUtils.ProjectFiles
{
    public class ProjectInfo
    {
        public string ProjectPath { get; set; }

        public SemanticVersion? Version { get; set; }
        public SemanticVersion? AssemblyVersion { get; set; }
        public SemanticVersion? FileVersion { get; set; }
        public string ProductName { get; set; }

        public bool HasVersionInfo => Version != null || FileVersion != null || AssemblyVersion != null;

        public void WriteChangelog(string content)
        {
            var directory = Path.GetDirectoryName(ProjectPath);
            if (!Directory.Exists(directory))
                return;
            
            var path = Path.Combine(directory, "CHANGELOG.md");
                       
            if (File.Exists(path))
            {
                var existing = File.ReadAllText(path);
                File.WriteAllText(path, content + Environment.NewLine + existing);
            }
            else
            {
                File.WriteAllText(path, content);
            }
        }

        public void UpdateCsproj(SemanticVersion? version, SemanticVersion? fileVersion, SemanticVersion? assemblyVersion)
        {
            var doc = XDocument.Load(ProjectPath);
            var propertyGroup = doc.Descendants("PropertyGroup").First();

            if(version != null)
            {
                SetOrCreate(propertyGroup, "Version", version.ToString());
            }

            if (fileVersion != null)
            {
                SetOrCreate(propertyGroup, "FileVersion", fileVersion.ToString());
            }

            if (assemblyVersion != null)
            {
                SetOrCreate(propertyGroup, "AssemblyVersion", assemblyVersion.ToString());
            }

            doc.Save(ProjectPath);
        }

        private void SetOrCreate(XElement parent, string elementName, string value)
        {
            var element = parent.Element(elementName);
            if (element == null)
                parent.Add(new XElement(elementName, value));
            else
                element.Value = value;
        }

        public override string ToString()
        {
            return $"{ProjectPath} | Version={Version}, Assembly={AssemblyVersion}, File={FileVersion}";
        }
    }
}

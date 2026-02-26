using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace NetChangelogUtils.ProjectFiles.Version
{
    public class ProjectVersionManager
    {
        private readonly List<ProjectVersionInfo> _projects;

        public ProjectVersionManager(List<ProjectVersionInfo> projects)
        {
            _projects = projects;
        }

        public IEnumerable<ProjectVersionInfo> GetAll() => _projects;

        public void UpdateVersion(string projectPath, string newVersion)
        {
            var project = _projects.First(p => p.ProjectPath == projectPath);

            project.Version = newVersion;
            project.AssemblyVersion = newVersion;
            project.FileVersion = newVersion;
            project.InformationalVersion = newVersion;

            UpdateCsproj(project);
        }

        private void UpdateCsproj(ProjectVersionInfo project)
        {
            var doc = XDocument.Load(project.ProjectPath);
            var propertyGroup = doc.Descendants("PropertyGroup").First();

            SetOrCreate(propertyGroup, "Version", project.Version);
            SetOrCreate(propertyGroup, "AssemblyVersion", project.AssemblyVersion);
            SetOrCreate(propertyGroup, "FileVersion", project.FileVersion);
            SetOrCreate(propertyGroup, "InformationalVersion", project.InformationalVersion);

            doc.Save(project.ProjectPath);
        }

        private void SetOrCreate(XElement parent, string elementName, string value)
        {
            var element = parent.Element(elementName);
            if (element == null)
                parent.Add(new XElement(elementName, value));
            else
                element.Value = value;
        }
    }
}

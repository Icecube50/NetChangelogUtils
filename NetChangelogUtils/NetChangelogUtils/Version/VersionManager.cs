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

    }
}

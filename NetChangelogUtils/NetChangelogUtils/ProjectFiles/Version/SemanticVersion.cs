using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NetChangelogUtils.ProjectFiles.Version
{
    public class SemanticVersion
    {
        public static SemanticVersion? CreateFrom(string s)
        {
            if(string.IsNullOrEmpty(s)) 
                return null;

            
            if(!Regex.Match(s, "[0-9]" + Regex.Escape(".") + "[0-9]" + Regex.Escape(".") + "[0-9]").Success)
                return null;

            var version = s.Split('.');
            return new SemanticVersion(int.Parse(version[0]), int.Parse(version[1]), int.Parse(version[2]));
        }

        public int Major { get; }
        public int Minor { get; }
        public int Patch { get; }

        public SemanticVersion(int major, int minor, int patch)
        {
            Major = major;
            Minor = minor;
            Patch = patch;
        }

        public SemanticVersion BumpMajor()
            => new SemanticVersion(Major + 1, 0, 0);

        public SemanticVersion BumpMinor()
            => new SemanticVersion(Major, Minor + 1, 0);

        public SemanticVersion BumpPatch()
            => new SemanticVersion(Major, Minor, Patch + 1);

        public override string ToString()
            => $"{Major}.{Minor}.{Patch}";
    }
}

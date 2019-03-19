using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Semver;

namespace NupackageDllExtractorLib.FileUtils
{
    public static class SemVersionUtils
    {
        public static SemVersion GetValidSemVersion(string version)
        {
            Regex regex = new Regex(@"(?<major>0|[1-9]\d*)\.(?<minor>0|[1-9]\d*)\.(?<patch>0|[1-9]\d*)(?:-(?<prerelease>(?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*)(?:\.(?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*))*))?(?:\+(?<buildmetadata>[0-9a-zA-Z-]+(?:\.[0-9a-zA-Z-]+)*))?$");
            Match match = regex.Match(version);
            if (match.Success)
            {
                int major = Convert.ToInt32(match.Groups[1].Value);
                int minor = Convert.ToInt32(match.Groups[2].Value);
                int patch = Convert.ToInt32(match.Groups[3].Value);
                string[] spl = match.Groups[4].Value.Split('.');
                string preRelease = spl[0];
                string build = spl.Count() > 1 ? spl[1] : string.Empty;
                SemVersion semVersion = new SemVersion(major, minor, patch, preRelease, build);
                return semVersion;
            }
            else
            {
                return null;
            }
        }

        public static string GetLastVersionOfNugetPackages(IList<string> nupkgPathFiles)
        {
            string lastVersion = "";
            SemVersion latestSemVersion = new SemVersion(0);
            for (var i = 0; i < nupkgPathFiles.Count; i++)
            {
                SemVersion currentSemVersion = GetValidSemVersion(nupkgPathFiles[i]);
                int comparison = latestSemVersion.CompareTo(currentSemVersion);
                if (comparison < 0)
                {
                    latestSemVersion = currentSemVersion;
                }
            }
            lastVersion = latestSemVersion.ToString().Replace('+', '.');
            return lastVersion;
        }
    }
}

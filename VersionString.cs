using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Loader
{
    class VersionString : IComparable<VersionString>
    {
        public int Major;
        public int Minor;
        public int Third;
        public int Fourth;

        private const string VersionPattern = @"(\d+)\.(\d+)\.(\d+)\.(\d+)";
        public VersionString(string rawString)
        {
            var match = System.Text.RegularExpressions.Regex.Match(rawString.Trim(), VersionPattern);
            //var match = System.Text.RegularExpressions.Regex.Match("1,2","(1)");

            //TODO: handle exceptions
            Major = int.Parse(match.Groups[1].Value);
            Minor = int.Parse(match.Groups[2].Value);
            Third = int.Parse(match.Groups[3].Value);
            Fourth = int.Parse(match.Groups[4].Value);
        }

        public VersionString(int Major, int Minor, int Third, int Fourth)
        {
            this.Major = Major;
            this.Minor = Minor;
            this.Third = Third;
            this.Fourth = Fourth;
        }

        public override string ToString()
        {
            return string.Format("{0}.{1}.{2}.{3}", Major, Minor, Third, Fourth);
        }

        public int CompareTo(VersionString other)
        {
            if (other.Major != Major) {
                return Major.CompareTo(other.Major);
            }
            if (other.Minor != Minor) {
                return Minor.CompareTo(other.Minor);
            }
            if (other.Third != Third) {
                return Third.CompareTo(other.Third);
            }
            return Fourth.CompareTo(other.Fourth);
        }
    }
}

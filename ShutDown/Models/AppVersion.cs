using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace ShutDown.Models
{

    public class AppVersion : IComparable<AppVersion>
    {
        private static AppVersion _current = null;
        public string Text { get; } = "1.0.0";

        public int Major { get; } = 1;
        public int Minor { get; }
        public int Patch { get; }

        public static AppVersion CurrentVersion
        {
            get
            {
                if (_current != null)
                {
                    return _current;
                }

                Assembly asm = typeof(Extensions).Assembly;
                FileVersionInfo info = FileVersionInfo.GetVersionInfo(asm.Location);
                AppVersion version = new AppVersion(info.FileVersion);
                _current = version;
                return version;
            }
        }

        public AppVersion(string version)
        {
            if (version.Count(c => c == '.') >= 2 && !version.StartsWith("."))
            {
                string[] parts = version.Split('.');

                int major;
                int minor;
                int patch;

                if ((parts.Length == 3 || parts.Length == 4)
                    && int.TryParse(parts[0], out major)
                    && int.TryParse(parts[1], out minor)
                    && int.TryParse(parts[2], out patch))
                {
                    Major = major;
                    Minor = minor;
                    Patch = patch;
                }

                Text = $"{Major}.{Minor}.{Patch}";
            }
        }

        public int CompareTo(AppVersion other)
        {
            if (ReferenceEquals(other, null)) return 1;

            int majorCmp = Major.CompareTo(other.Major);
            if (majorCmp != 0) return majorCmp;

            int minorCmp = Minor.CompareTo(other.Minor);
            if (minorCmp != 0) return minorCmp;

            return Patch.CompareTo(other.Patch);
        }

        public override bool Equals(object obj)
        {
            var other = obj as AppVersion;
            if (ReferenceEquals(other, null)) return false;

            return Major == other.Major &&
                   Minor == other.Minor &&
                   Patch == other.Patch;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + Major.GetHashCode();
                hash = hash * 23 + Minor.GetHashCode();
                hash = hash * 23 + Patch.GetHashCode();
                return hash;
            }
        }

        public static bool operator ==(AppVersion left, AppVersion right)
        {
            if (left is null && right is null) return true;

            if (ReferenceEquals(left, right)) return true;
            if (ReferenceEquals(left, null)) return false;
            if (ReferenceEquals(right, null)) return false;
            return left.Equals(right);
        }

        public static bool operator !=(AppVersion left, AppVersion right)
        {
            return !(left == right);
        }

        public static bool operator >(AppVersion left, AppVersion right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator <(AppVersion left, AppVersion right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator >=(AppVersion left, AppVersion right)
        {
            return left.CompareTo(right) >= 0;
        }

        public static bool operator <=(AppVersion left, AppVersion right)
        {
            return left.CompareTo(right) <= 0;
        }

        public override string ToString()
        {
            return Text;
        }
    }
}
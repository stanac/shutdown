using System.Reflection;
using Microsoft.Win32;

namespace ShutDown.Data
{
    public static class StartWithWindowsAccessor
    {
        private static readonly string _execPath;
        private static RegistryKey _regKeyDir = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
        private const string _regKey = "shutdown_stanac";

        static StartWithWindowsAccessor()
        {
            _execPath = Assembly.GetExecutingAssembly().Location + " /winstart";
        }

        public static void Set(bool startWithWindows)
        {
            if (startWithWindows)
            {
                _regKeyDir.SetValue(_regKey, _execPath);
            }
            else
            {
                _regKeyDir.DeleteValue(_regKey, false);
            }
        }

        public static bool IsSet()
        {
            return _regKeyDir.GetValue(_regKey) != null;
        }
    }
}

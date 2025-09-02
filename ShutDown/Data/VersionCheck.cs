using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ShutDown.Models;

namespace ShutDown.Data
{
    public class VersionCheck : DataBase
    {
        private readonly string _filePath;

        public static VersionCheck Instance { get; } = new VersionCheck();

        public bool CheckForNewVersion { get; set; }
        public long LastCheckTimestamp { get; private set; }
        public string LastReleasedVersion { get; private set; }

        private VersionCheck()
        {
            _filePath = Path.Combine(FolderPath, "version-check");

            if (File.Exists(_filePath))
            {
                var lines = File.ReadAllLines(_filePath);

                foreach (var line in lines)
                {
                    if (line.Contains(":"))
                    {
                        var value = line.Split(new [] { ':' }, 2).Last();

                        if (line.StartsWith($"{nameof(CheckForNewVersion)}:"))
                        {
                            CheckForNewVersion = bool.Parse(value);
                        }
                        else if (line.StartsWith($"{nameof(LastCheckTimestamp)}:"))
                        {
                            LastCheckTimestamp = long.Parse(value);
                        }
                        else if (line.StartsWith($"{nameof(LastReleasedVersion)}:"))
                        {
                            LastReleasedVersion = value;
                        }
                    }
                }
            }
        }

        public void Save()
        {
            string s = $@"{nameof(CheckForNewVersion)}:{CheckForNewVersion}
{nameof(LastCheckTimestamp)}:{LastCheckTimestamp}
{nameof(LastReleasedVersion)}:{LastReleasedVersion}";

            File.WriteAllText(_filePath, s);
        }

        // ReSharper disable once AsyncVoidMethod
        public async void Check(Action<bool> callback)
        {
            if (!CheckForNewVersion)
            {
                callback(false);
                return;
            }
            
            if (LastReleasedVersion != null)
            {
                AppVersion v = new AppVersion(LastReleasedVersion);

                if (v > AppVersion.CurrentVersion)
                {
                    callback(true);
                    return;
                }
            }

            if (LastCheckTimestamp == 0)
            {
                LastCheckTimestamp = DateTimeOffset.UtcNow.AddDays(-8).ToUnixTimeSeconds();
            }

            if ((DateTimeOffset.UtcNow - DateTimeOffset.FromUnixTimeSeconds(LastCheckTimestamp)).TotalDays < 7.0)
            {
                callback(false);
                return;
            }

            AppVersion lastReleased = await GetLastReleasedVersion();

            LastCheckTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            LastReleasedVersion = lastReleased.Text;
            Save();

            callback(lastReleased > AppVersion.CurrentVersion);
        }

        private async Task<AppVersion> GetLastReleasedVersion()
        {
            AppVersion defaultVersion = new AppVersion("0.0.1");

            try
            {
                HttpClient client = new HttpClient();
                string releases = await client.GetStringAsync("https://stanac.github.io/shutdown/release_names.txt");

                AppVersion[] versions = releases.Split(new [] { '\n' }, StringSplitOptions.RemoveEmptyEntries)
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Select(x =>
                    {
                        try
                        {
                            return new AppVersion(x);
                        }
                        catch
                        {
                            return null;
                        }
                    })
                    .Where(x => x != null)
                    .OrderByDescending(x => x)
                    .ToArray();

                if (versions.Any())
                {
                    return versions.First();
                }

            }
            catch (Exception e)
            {
                Log.Error("Failed to fetch remote releases file", e);
            }

            return defaultVersion;
        }
    }
}

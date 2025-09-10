using ShutDown.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ShutDown.Data
{
    public class SettingsData : DataBase
    {
        public static SettingsData Instance { get; private set; }
        
        private static readonly string _filePath;
        
        public int MinMinutes { get; set; }
        public int MaxMinutes { get; set; }
        public int DefaultDelay { get; set; }
        public bool DefaultForce { get; set; }
        public ShutDownOperation DefaultOperation { get; set; }
        public bool CloseToTray { get; set; }
        public bool BlinkTrayIcon { get; set; }
        public List<PatternModel> Patterns { get; set; }
        public bool PreventShutDown { get; set; }
        public bool PreventLock { get; set; }
        public bool JiggleMouse { get; set; }

        private SettingsData()
        {
            Patterns = new List<PatternModel>();
        }

        static SettingsData()
        {
            _filePath = Path.Combine(FolderPath, "settings");
            
            try
            {
                if (!Directory.Exists(FolderPath))
                {
                    Directory.CreateDirectory(FolderPath);
                }

                if (!File.Exists(_filePath))
                {
                    var def = DefaultSettings();
                    def.Save();
                }
            }
            catch (Exception ex)
            {
                Log.LogErrorAndDisplayMessageBox("Failed to create settings dir/file.", ex);
            }

            LoadInstance();
        }

        private static void LoadInstance()
        {
            try
            {
                string[] lines = File.ReadAllLines(_filePath)
                    .Select(x => x.Trim())
                    .Where(x => x.Length > 0)
                    .ToArray();

                Instance = new SettingsData();
                bool blinkIsSet = false;

                foreach (var line in lines)
                {
                    if (line.StartsWith($"{nameof(MinMinutes)}:"))
                    {
                        Instance.MinMinutes = int.Parse(line.Split(':')[1]);
                    }
                    else if (line.StartsWith($"{nameof(MaxMinutes)}:"))
                    {
                        Instance.MaxMinutes = int.Parse(line.Split(':')[1]);
                    }
                    else if (line.StartsWith($"{nameof(DefaultDelay)}:"))
                    {
                        Instance.DefaultDelay = int.Parse(line.Split(':')[1]);
                    }
                    else if (line.StartsWith($"{nameof(DefaultForce)}:"))
                    {
                        Instance.DefaultForce = bool.Parse(line.Split(':')[1]);
                    }
                    else if (line.StartsWith($"{nameof(DefaultOperation)}:"))
                    {
                        Instance.DefaultOperation = (ShutDownOperation)Enum.Parse(typeof(ShutDownOperation), line.Split(':')[1]);
                    }
                    else if (line.StartsWith($"{nameof(CloseToTray)}:"))
                    {
                        Instance.CloseToTray = bool.Parse(line.Split(':')[1]);
                    }
                    else if (line.StartsWith($"{nameof(BlinkTrayIcon)}:"))
                    {
                        Instance.BlinkTrayIcon = bool.Parse(line.Split(':')[1]);
                        blinkIsSet = true;
                    }
                    else if (line.StartsWith($"{nameof(PreventShutDown)}:"))
                    {
                        Instance.PreventShutDown = bool.Parse(line.Split(':')[1]);
                    }
                    else if (line.StartsWith($"{nameof(PreventLock)}:"))
                    {
                        Instance.PreventLock = bool.Parse(line.Split(':')[1]);
                    }
                    else if (line.StartsWith($"{nameof(JiggleMouse)}:"))
                    {
                        Instance.JiggleMouse = bool.Parse(line.Split(':')[1]);
                    }
                    else if (line.StartsWith("p:"))
                    {
                        Instance.Patterns.Add(PatternModel.Parse(line));
                    }
                }
                
                if (!blinkIsSet)
                {
                    Instance.BlinkTrayIcon = true;
                    Instance.Save();
                }
            }
            catch
            {
                Instance = DefaultSettings(); 
            }
        }

        private static SettingsData DefaultSettings() => new SettingsData
        {
            MinMinutes = 5,
            MaxMinutes = 1440,
            DefaultDelay = 60,
            BlinkTrayIcon = true
        };

        public void Save()
        {
            string s = $@"
{nameof(MinMinutes)}:{MinMinutes}
{nameof(MaxMinutes)}:{MaxMinutes}
{nameof(DefaultDelay)}:{DefaultDelay}
{nameof(DefaultForce)}:{DefaultForce}
{nameof(DefaultOperation)}:{DefaultOperation}
{nameof(CloseToTray)}:{CloseToTray}
{nameof(BlinkTrayIcon)}:{BlinkTrayIcon}
{nameof(PreventShutDown)}:{PreventShutDown}
{nameof(PreventLock)}:{PreventLock}
{nameof(JiggleMouse)}:{JiggleMouse}
";
            foreach (var p in Patterns)
            {
                s += p.ToSerializableString() + @"
";
            }
            File.WriteAllText(_filePath, s.Trim());
        }
    }
}

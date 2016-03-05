using ShutDown.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ShutDown.Data
{
    public class Settings : DataBase
    {
        public static Settings Instance { get; private set; }
        
        private static readonly string _filePath;

        public int MinMinutes { get; set; }
        public int MaxMinutes { get; set; }
        public int DefaultDelay { get; set; }
        public bool DefaultForce { get; set; }
        public ShutDownOperation DefaultOperation { get; set; }
        public List<PatternModel> Patterns { get; set; }

        private Settings()
        {
            Patterns = new List<PatternModel>();
        }

        static Settings()
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
            } catch { }
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

                Instance = new Settings();

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
                    else if (line.StartsWith("p:"))
                    {
                        Instance.Patterns.Add(PatternModel.Parse(line));
                    }
                }
            }
            catch
            {
                Instance = DefaultSettings(); 
            }
        }

        private static Settings DefaultSettings() => new Settings
        {
            MinMinutes = 5,
            MaxMinutes = 1440,
            DefaultDelay = 60
        };

        public void Save()
        {
            string s = $@"
{nameof(MinMinutes)}:{MinMinutes}
{nameof(MaxMinutes)}:{MaxMinutes}
{nameof(DefaultDelay)}:{DefaultDelay}
{nameof(DefaultForce)}:{DefaultForce}
{nameof(DefaultOperation)}:{DefaultOperation}
";
            foreach (var p in Patterns)
            {
                s += p.ToSerializableString() + @"
";
            }
            File.WriteAllText(_filePath, s);
        }
    }
}

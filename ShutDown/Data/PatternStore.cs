using ShutDown.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ShutDown.Data
{
    public class PatternStore : DataBase
    {
        public static PatternStore Instance { get; }
        private static readonly string _filePath;

        private PatternStore()
        {
            // save default setting on first run
            if (!File.Exists(_filePath))
                File.WriteAllText(_filePath, @"p:U0QgNjBtaW4=;ShutDown;False;60;8e6cbfbc-75ad-463d-82b1-027b26d49168
p:Ri1TRCA2MG1pbg==;ShutDown;True;60;aba47f32-71ae-409e-8335-c38936ef0d51
p:U0QgMzBtaW4=;ShutDown;False;30;dc32f6f0-71ca-4536-ada4-0b57a3ff98f8
p:Ri1TRCAzMG1pbg==;ShutDown;True;30;90761887-f577-45cc-a111-4e0e8418080b
p:SCA2MG1pbg==;Hibernate;True;60;4bf47b6c-3afe-4018-ab03-5f8b1465eca6
p:SCAzMG1pbg==;Hibernate;True;30;016839b4-4a2d-4c4c-b948-ba22e29d4df3");
        }

        static PatternStore()
        {
            _filePath = Path.Combine(FolderPath, "patterns");
            Instance = new PatternStore();
        }

        public PatternModel[] GetPatterns()
        {
            try
            {
                string text = File.ReadAllText(_filePath);
                string[] lines = text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                PatternModel[] returnValue = new PatternModel[lines.Length];
                for (int i = 0; i < lines.Length; i++)
                {
                    returnValue[i] = PatternModel.Parse(lines[i]);
                }
                return returnValue;
            }
            catch (Exception ex)
            {
                Log.LogErrorAndDisplayMessageBox("Failed to retrieve patterns", ex);
                return new PatternModel[0];
            }
        }

        public void SetPatterns(IEnumerable<PatternModel> patterns)
        {
            try
            {
                string toWrite = string.Join(Environment.NewLine, patterns.Select(x => x.ToSerializableString()));
                File.WriteAllText(_filePath, toWrite);
            }
            catch (Exception ex)
            {
                Log.LogErrorAndDisplayMessageBox("Failed to save patterns", ex);
            }
        }
    }
}

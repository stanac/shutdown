using ShutDown.Data;
using ShutDown.Models;
using System;
using System.Diagnostics;
using System.IO;

namespace ShutDown
{
    public static class ShutDownHelper
    {
        public static void ExecuteShutDownOperation(ShutDownOperation operation, bool force)
        {
            try
            {
                string cmd = @"shutdown";
                string args = operation.GetCommandLineArgs(force);

                var start = new ProcessStartInfo(cmd, args);
                start.ErrorDialog = true;
                Debug.WriteLine("cmd: " + (cmd ?? "null"));
                Debug.WriteLine("args: " + (args ?? "null"));
                start.UseShellExecute = false;
                start.CreateNoWindow = true;
                using (Process proc = Process.Start(start))
                {
                    proc.WaitForExit();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                string operationName = force ? "forced " : "not forced ";
                operationName += operation.ToString().ToLower();
                Log.LogErrorAndDisplayMessageBox($"Failed to execute {operation}", ex);
            }
        }
    }
}

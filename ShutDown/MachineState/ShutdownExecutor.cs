using System.Diagnostics;
using ShutDown.Models;

namespace ShutDown.MachineState
{
    public class ShutdownExecutor : IExecutor
    {
        public void Execute(ShutDownOperation operation, bool force)
        {
            string cmd = @"shutdown";
            string args = operation.GetCommandLineArgs(force);

            var start = new ProcessStartInfo(cmd, args)
            {
                ErrorDialog = true
            };
            Debug.WriteLine("cmd: " + (cmd ?? "null"));
            Debug.WriteLine("args: " + (args ?? "null"));
            start.UseShellExecute = false;
            start.CreateNoWindow = true;
            using (Process proc = Process.Start(start))
            {
                proc?.WaitForExit();
            }
        }
    }
}

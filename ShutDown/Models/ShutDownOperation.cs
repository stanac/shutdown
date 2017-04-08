using System;

namespace ShutDown.Models
{
    public enum ShutDownOperation
    {
        ShutDown,
        SignOut,
        Restart,
        Hibernate,
        Sleep
    }

    public static class ShutDownOperationMethods
    {
        public static string GetOperationName(this ShutDownOperation op, bool force)
        {
            string opName;
            switch (op)
            {
                case ShutDownOperation.Hibernate:
                    opName = "Hibernate";
                    break;
                case ShutDownOperation.Restart:
                    opName = force ? "Forced restart" : "Restart";
                    break;
                case ShutDownOperation.ShutDown:
                    opName = force ? "Forced shut down" : "Shut down";
                    break;
                case ShutDownOperation.SignOut:
                    opName = "Log off";
                    break;
                case ShutDownOperation.Sleep:
                    opName = "Sleep";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return opName;
        }

        public static string GetCommandLineArgs(this ShutDownOperation operation, bool force)
        {
            string args;
            switch (operation)
            {
                case ShutDownOperation.Hibernate:
                    args = string.Empty;
                    break;
                case ShutDownOperation.Restart:
                    args = "-r";
                    break;
                case ShutDownOperation.ShutDown:
                    args = "-s";
                    break;
                case ShutDownOperation.SignOut:
                    args = "-l";
                    break;
                case ShutDownOperation.Sleep:
                    args = string.Empty;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (!string.IsNullOrEmpty(args))
            {
                args += (force ? " -f" : string.Empty) + " -t 0";
            }

            return args;
        }
    }
}

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
        public static string GetOperationName(this ShutDownOperation op, bool force, bool useShortName = false)
        {
            string opName;
            switch (op)
            {
                case ShutDownOperation.Hibernate:
                    opName = useShortName ? "H" : "Hibernate";
                    break;
                case ShutDownOperation.Restart:
                    if (useShortName)
                        opName = force ? "F-R" : "R";
                    else
                        opName = force ? "Forced restart" : "Restart";
                    break;
                case ShutDownOperation.ShutDown:
                    if (useShortName)
                        opName = force ? "F-SD" : "SD";
                    else
                        opName = force ? "Forced shut down" : "Shut down";
                    break;
                case ShutDownOperation.SignOut:
                    opName = useShortName ? "LO" : "Log off";
                    break;
                case ShutDownOperation.Sleep:
                    opName = useShortName ? "S" : "Sleep";
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

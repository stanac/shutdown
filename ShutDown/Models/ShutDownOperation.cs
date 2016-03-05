using System;

namespace ShutDown.Models
{
    public enum ShutDownOperation
    {
        ShutDown,
        SignOut,
        Restart,
        Hibernate
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
                    if (force)
                    {
                        opName = "Forced restart";
                    }
                    else
                    {
                        opName = "Restart";
                    }
                    break;
                case ShutDownOperation.ShutDown:
                    if (force)
                    {
                        opName = "Forced shut down";
                    }
                    else
                    {
                        opName = "Shut down";
                    }
                    break;
                case ShutDownOperation.SignOut:
                    opName = "Log off";
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
                    args = "-h";
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
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (force && operation != ShutDownOperation.Hibernate && operation != ShutDownOperation.SignOut)
            {
                args += " -f";
            }

            if (operation != ShutDownOperation.SignOut && operation != ShutDownOperation.Hibernate)
            {
                args += " -t 0";
            }

            return args;
        }
    }

}

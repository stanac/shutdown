using ShutDown.Data;
using System;
using System.Runtime.InteropServices;
using System.Windows.Threading;

namespace ShutDown
{
    public static class PreventShutDownHelper
    {
        private static object Sync = new object();
        private static bool _preventShutDown;
        private static bool _preventLock;

        static PreventShutDownHelper()
        {
            PreventShutDown = Settings.Instance.PreventShutDown;
            PreventLock = Settings.Instance.PreventLock;
            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick += (s, e) =>
            {
                EXECUTION_STATE state = EXECUTION_STATE.ES_CONTINUOUS;
                if (PreventLock)
                {
                    state |= EXECUTION_STATE.ES_SYSTEM_REQUIRED;
                    state |= EXECUTION_STATE.ES_DISPLAY_REQUIRED;
                }
                else if (PreventShutDown)
                {
                    state |= EXECUTION_STATE.ES_SYSTEM_REQUIRED;
                }
                SetThreadExecutionState(state);
            };
            timer.Interval = TimeSpan.FromSeconds(30);
            timer.Start();
        }

        public static bool PreventShutDown
        {
            get
            {
                lock (Sync)
                {
                    return _preventShutDown;
                }
            }
            set
            {
                lock (Sync)
                {
                    if (value != _preventShutDown)
                    {
                        Settings.Instance.PreventShutDown = value;
                        Settings.Instance.Save();
                        _preventShutDown = value;
                    }
                }
            }
        }

        public static bool PreventLock
        {
            get
            {
                lock (Sync)
                {
                    return _preventLock;
                }
            }
            set
            {
                lock (Sync)
                {
                    if (value != _preventLock)
                    {
                        Settings.Instance.PreventLock = value;
                        Settings.Instance.Save();
                        _preventLock = value;
                    }
                }
            }
        }

        #region native

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern EXECUTION_STATE SetThreadExecutionState(EXECUTION_STATE esFlags);

        [Flags]
        private enum EXECUTION_STATE : uint
        {
            ES_AWAYMODE_REQUIRED = 0x00000040,
            ES_CONTINUOUS = 0x80000000,
            ES_DISPLAY_REQUIRED = 0x00000002,
            ES_SYSTEM_REQUIRED = 0x00000001
        }

        #endregion
    }
}

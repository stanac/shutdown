using ShutDown.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace ShutDown
{
    public static class PreventShutDownHelper
    {
        private static object Sync = new object();
        private static bool _prevent;

        static PreventShutDownHelper()
        {
            Prevent = Settings.Instance.PreventShutDown;
            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick += (s, e) =>
            {
                EXECUTION_STATE state = EXECUTION_STATE.ES_CONTINUOUS;
                if (Prevent)
                {
                    state |= EXECUTION_STATE.ES_SYSTEM_REQUIRED;
                }
                SetThreadExecutionState(state);
            };
            timer.Interval = TimeSpan.FromSeconds(10);
            timer.Start();
        }

        public static bool Prevent
        {
            get
            {
                lock (Sync)
                {
                    return _prevent;
                }
            }
            set
            {
                lock (Sync)
                {
                    if (value != _prevent)
                    {
                        Settings.Instance.PreventShutDown = value;
                        Settings.Instance.Save();
                        _prevent = value;
                    }
                }
            }
        }
        
        #region native

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern EXECUTION_STATE SetThreadExecutionState(EXECUTION_STATE esFlags);

        [FlagsAttribute]
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

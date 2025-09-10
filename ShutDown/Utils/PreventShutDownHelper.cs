using System;
using System.Runtime.InteropServices;
using System.Windows.Threading;
using ShutDown.Data;

namespace ShutDown.Utils
{
    public static class PreventShutDownHelper
    {
        private static readonly DispatcherTimer Timer;

        static PreventShutDownHelper()
        {
            Timer = new DispatcherTimer();
            Timer.Tick += (s, e) =>
            {
                var preventShutDown = SettingsData.Instance.PreventShutDown;
                var preventLock = SettingsData.Instance.PreventLock;

                EXECUTION_STATE state = EXECUTION_STATE.ES_CONTINUOUS;

                if (preventLock)
                {
                    state |= EXECUTION_STATE.ES_SYSTEM_REQUIRED;
                    state |= EXECUTION_STATE.ES_DISPLAY_REQUIRED;
                }
                else if (preventShutDown)
                {
                    state |= EXECUTION_STATE.ES_SYSTEM_REQUIRED;
                }

                SetThreadExecutionState(state);
            };
            Timer.Interval = TimeSpan.FromSeconds(30);
            Timer.Start();
        }

        public static void Run()
        {
            // Do nothing, just make sure instance of time exists

            if (Timer == null) // should neve be null
            {
                Log.Error($"Tiles is null in {nameof(PreventShutDownHelper)}");
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

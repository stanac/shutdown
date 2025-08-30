using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using ShutDown.Data;

namespace ShutDown
{
    public partial class App : Application
    {
        private Mutex _mutex;
        private bool _isMutexOwnedByThisInstance;
        private EventWaitHandle _showExistingInstanceEvent;
        private Thread _listenerThread;

        public static bool IsDebug { get; private set; }
        public static string ProcessId { get; } = Process.GetCurrentProcess().Id.ToString();

        protected override void OnStartup(StartupEventArgs e)
        {
#if DEBUG
            IsDebug = true;
#endif
            if (!IsDebug)
            {
                IsDebug = e.Args.Contains("dbg");
            }

            // Use a GUID for unique Mutex and Event names
            string appGuid = ((GuidAttribute)Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(GuidAttribute), false).GetValue(0)).Value;
            string mutexId = $"Global\\Mutex_{appGuid}";
            string eventId = $"Global\\Event_{appGuid}";

            _mutex = new Mutex(true, mutexId, out _isMutexOwnedByThisInstance);

            if (!_isMutexOwnedByThisInstance)
            {
                try
                {
                    _showExistingInstanceEvent = EventWaitHandle.OpenExisting(eventId);
                    _showExistingInstanceEvent.Set();
                }
                catch (WaitHandleCannotBeOpenedException ex)
                {
                    Log.LogErrorAndDisplayMessageBox("Failed to open main window of existing app instance.", ex);
                }
                finally
                {
                    Log.Debug("Closing the app, another instance already running.");
                    Shutdown();
                }
            }
            else
            {
                _showExistingInstanceEvent = new EventWaitHandle(false, EventResetMode.AutoReset, eventId);
                _listenerThread = new Thread(ListenForShowMessage)
                {
                    IsBackground = true
                };
                _listenerThread.Start();

                base.OnStartup(e);
            }
        }

        private void ListenForShowMessage()
        {
            while (true)
            {
                try
                {
                    _showExistingInstanceEvent.WaitOne();
                    Dispatcher.Invoke(ShowAndActivateMainWindow);
                }
                catch (ObjectDisposedException)
                {
                    break;
                }
                catch (Exception)
                {
                    break;
                }
            }
        }

        private void ShowAndActivateMainWindow()
        {
            var mainWindow = Current.MainWindow;
            if (mainWindow != null)
            {
                if (!mainWindow.IsVisible)
                {
                    mainWindow.Show();
                }

                if (mainWindow.WindowState == WindowState.Minimized)
                {
                    mainWindow.WindowState = WindowState.Normal;
                }

                mainWindow.Activate();
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            if (_isMutexOwnedByThisInstance)
            {
                _showExistingInstanceEvent?.Dispose();

                _mutex?.ReleaseMutex();
                _mutex?.Dispose();
            }

            base.OnExit(e);
        }
    }
}
using System.Windows.Input;
using ShutDown.Data;
using ShutDown.Utils;

namespace ShutDown.Views.Settings
{
    public class SettingsViewModel : ObservableObject
    {
        public ObservableProperty<bool> StartWithWindows { get; }
        public ObservableProperty<bool> CloseToTray { get; }
        public ObservableProperty<bool> PreventShutDown { get; }
        public ObservableProperty<bool> PreventLock { get; }
        public ObservableProperty<bool> JiggleMouse { get; }
        public ObservableProperty<bool> BlinkTrayIcon { get; }
        public ObservableProperty<bool> CheckForNewVersion { get; }

        public ICommand HideSettingsCommend { get; } = new Command(() => Mediator.Instance.HideSettingsView());

        public SettingsViewModel()
        {
            StartWithWindows = new ObservableProperty<bool>(nameof(StartWithWindows), this, StartWithWindowsAccessor.IsSet(), StartWithWindowsAccessor.Set);
            CloseToTray = new ObservableProperty<bool>(nameof(CloseToTray), this, SettingsData.Instance.CloseToTray,
                value =>
                {
                    SettingsData.Instance.CloseToTray = value;
                    SettingsData.Instance.Save();
                });
            PreventShutDown = new ObservableProperty<bool>(nameof(PreventShutDown), this, SettingsData.Instance.PreventShutDown,
                value =>
                {
                    SettingsData.Instance.PreventShutDown = value;
                    SettingsData.Instance.Save();
                });
            PreventLock = new ObservableProperty<bool>(nameof(PreventLock), this, SettingsData.Instance.PreventLock,
                value =>
                {
                    SettingsData.Instance.PreventLock = value;
                    SettingsData.Instance.Save();
                });
            JiggleMouse = new ObservableProperty<bool>(nameof(JiggleMouse), this, SettingsData.Instance.JiggleMouse,
                value =>
                {
                    SettingsData.Instance.JiggleMouse = value;
                    SettingsData.Instance.Save();
                });
            BlinkTrayIcon = new ObservableProperty<bool>(nameof(BlinkTrayIcon), this, SettingsData.Instance.BlinkTrayIcon,
                value =>
                {
                    SettingsData.Instance.BlinkTrayIcon = value;
                    SettingsData.Instance.Save();
                });
            CheckForNewVersion = new ObservableProperty<bool>(nameof(CheckForNewVersion), this,
                VersionCheck.Instance.CheckForNewVersion,
                value =>
                {
                    VersionCheck.Instance.CheckForNewVersion = value;
                    VersionCheck.Instance.Save();

                    if (value)
                    {
                        Mediator.Instance.NewVersionCheckRequested();
                    }
                });
        }
    }

}
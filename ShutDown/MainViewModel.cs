using ShutDown.Data;
using ShutDown.Models;
using System.Windows.Input;
using System.Collections.ObjectModel;
using ShutDown.Utils;

namespace ShutDown
{
    public class MainViewModel : ObservableObject
    {
        internal const string WindowTitle = "Shut Down";
        
        public TrayIcon TheTrayIcon { get; } = new TrayIcon();

        public ICommand ShowSettingsCommand { get; }


        public MainViewModel()
        {
            MouseJigglerHelper.Start();

            Title = WindowTitle;

            SettingsVisible = new ObservableProperty<bool>(nameof(SettingsVisible), this, false);
            NewPatternVisible = new ObservableProperty<bool>(nameof(NewPatternVisible), this, false);
            ShowCurrentOperation = new ObservableProperty<bool>(nameof(ShowCurrentOperation), this, false);

            ShowSettingsCommand = new Command(() => SettingsVisible.Value = true);

            GlobalFunctions.Instance.HideSettingsView = () => SettingsVisible.Value = false;
            GlobalFunctions.Instance.NewVersionCheckRequested = CheckForNewVersion;
            GlobalFunctions.Instance.ShowNewPatternView = () => NewPatternVisible.Value = true;
            GlobalFunctions.Instance.HideNewPatternView = () => NewPatternVisible.Value = false;
            GlobalFunctions.Instance.HideCurrentOperationView = () => ShowCurrentOperation.Value = false;
            GlobalFunctions.Instance.ShowCurrentOperationView = () => ShowCurrentOperation.Value = true;
            
            foreach (var item in PatternStore.Instance.GetPatterns())
            {
                Patterns.Add(item);
            }

            CheckForNewVersion();
        }

        #region props

        public string Title { get; }

        public ObservableProperty<bool> SettingsVisible { get; }
        public ObservableProperty<bool> NewPatternVisible { get; }
        public ObservableProperty<bool> ShowCurrentOperation { get; }
        
        public string Version { get; } = AppVersion.CurrentVersion.Text;

        public bool IsNewVersionAvailable { get; set; }

        public ObservableCollection<PatternModel> Patterns { get; } = new ObservableCollection<PatternModel>();

        #endregion

        private void CheckForNewVersion()
        {
            VersionCheck.Instance.Check(isAvailable =>
            {
                if (IsNewVersionAvailable != isAvailable)
                {
                    IsNewVersionAvailable = isAvailable;
                    RaisePropertyChanged(nameof(IsNewVersionAvailable));
                }
            });
        }
    }
}

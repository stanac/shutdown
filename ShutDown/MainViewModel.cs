using ShutDown.Data;
using ShutDown.Models;
using System;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.Linq;
using ShutDown.Utils;

namespace ShutDown
{
    public class MainViewModel : ObservableObject
    {
        internal const string WindowTitle = "Shut Down";

        #region private

        private ShutDownOperation _operation = ShutDownOperation.ShutDown;
        private ICommand _selectOperationCommand;
        private ICommand _addDelayCommand;
        private ICommand _toggleForceCommand;
        private ICommand _startShutDownCommand;
        private ICommand _showSettingsCommand;
        
        private int _delayMinutes = 60;
        private string _shutDownRemainingTime;
        private string _operationName;
        
        #endregion

        public TrayIcon TheTrayIcon = new TrayIcon();
        
        public MainViewModel()
        {
            MouseJigglerHelper.Start();


            Title = WindowTitle;

            SettingsVisible = new ObservableProperty<bool>(nameof(SettingsVisible), this, false);
            PatternVisible = new ObservableProperty<bool>(nameof(PatternVisible), this, false);
            ShutDownInProgress = new ObservableProperty<bool>(nameof(ShutDownInProgress), this, false);

            GlobalFunctions.Instance.HideSettingsView = () => SettingsVisible.Value = false;
            GlobalFunctions.Instance.NewVersionCheckRequested = CheckForNewVersion;
            GlobalFunctions.Instance.StartFromPattern = StartFromPattern;
            GlobalFunctions.Instance.ShowNewPatternView = () => PatternVisible.Value = true;
            GlobalFunctions.Instance.HideNewPatternView = () => PatternVisible.Value = false;
            GlobalFunctions.Instance.HideCurrentOperationView = () => ShutDownInProgress.Value = false;
            GlobalFunctions.Instance.GetCurrentOperation = () => new OperationModel
            {
                DelayInMinutes = DelayMinutes,
                Force = Force,
                Operation = Operation
            };

            var settings = SettingsData.Instance;
            Operation = settings.DefaultOperation;
            DelayMinutes = settings.DefaultDelay;
            MinMinutes = settings.MinMinutes;
            MaxMinutes = settings.MaxMinutes;
            Force = settings.DefaultForce;

            foreach (var item in PatternStore.Instance.GetPatterns())
            {
                Patterns.Add(item);
            }

            CheckForNewVersion();
        }

        #region props

        public string Title { get; }

        public int MinMinutes { get; }
        public int MaxMinutes { get; }
        public bool Force { get; private set; }
        public string ShutDownRemainingTime
        {
            get => _shutDownRemainingTime;
            set
            {
                if (value != _shutDownRemainingTime)
                {
                    _shutDownRemainingTime = value;
                    RaisePropertyChanged(nameof(ShutDownRemainingTime));
                }
            }
        }
        public string OperationName
        {
            get => _operationName;
            set
            {
                if (value != _operationName)
                {
                    _operationName = value;
                    RaisePropertyChanged(nameof(OperationName));
                }
            }
        }
        public ObservableProperty<bool> SettingsVisible { get; }
        public ObservableProperty<bool> PatternVisible { get; }
        public ObservableProperty<bool> ShutDownInProgress { get; }
        
        public string Version { get; } = AppVersion.CurrentVersion.Text;

        public bool IsNewVersionAvailable { get; set; }

        public ShutDownOperation Operation
        {
            get => _operation;
            set
            {
                if (value != _operation)
                {
                    _operation = value;
                    RaisePropertyChanged(nameof(Operation));
                }
            }
        }

        public int DelayMinutes
        {
            get => _delayMinutes;
            set
            {
                if (value != _delayMinutes)
                {
                    _delayMinutes = value;
                    SettingsData.Instance.DefaultDelay = value;
                    SettingsData.Instance.Save();
                    RaisePropertyChanged(nameof(DelayMinutes));
                    RaisePropertyChanged(nameof(DelayText));
                }
            }
        }
        public string DelayText
        {
            get
            {
                var ts = TimeSpan.FromMinutes(DelayMinutes);
                return $"{Math.Floor(ts.TotalHours).ToString("00")}h {ts.Minutes.ToString("00")}min";
            }
        }

        public ObservableCollection<PatternModel> Patterns { get; } = new ObservableCollection<PatternModel>();

        #endregion

        #region commands

        public ICommand SelectOperationCommand => _selectOperationCommand ?? (_selectOperationCommand = new Command<string>(SelectOperation));

        public ICommand AddDelayCommand => _addDelayCommand ?? (_addDelayCommand = new Command<string>(AddDelay));

        public ICommand ToggleForceCommand => _toggleForceCommand ?? (_toggleForceCommand = new Command(ToggleForce));

        public ICommand StartShutDownCommand => _startShutDownCommand ?? (_startShutDownCommand = new Command(StartShutDown));

        public ICommand ShowSettingsCommand => _showSettingsCommand ?? (_showSettingsCommand = new Command(() => SettingsVisible.Value = true));

        private void SelectOperation(string operation)
        {
            Operation = (ShutDownOperation)Enum.Parse(typeof(ShutDownOperation), operation, true);
            SettingsData.Instance.DefaultOperation = Operation;
            SettingsData.Instance.Save();
        }

        private void AddDelay(string value)
        {
            int val = DelayMinutes + int.Parse(value);
            if (val < MinMinutes) val = MinMinutes;
            else if (val > MaxMinutes) val = MaxMinutes;
            DelayMinutes = val;
        }

        private void ToggleForce()
        {
            Force = !Force;
            RaisePropertyChanged(nameof(Force));
            SettingsData.Instance.DefaultForce = Force;
            SettingsData.Instance.Save();
        }

        private void StartShutDown()
        {
            GlobalFunctions.Instance.StartOperation();
            ShutDownInProgress.Value = true;
        }

        private void StartFromPattern(Guid id)
        {
            var pattern = PatternStore.Instance.GetPatterns().FirstOrDefault(x => x.Id == id);
            if (pattern != null)
            {
                Force = pattern.Force;
                Operation = pattern.Operation;
                DelayMinutes = pattern.DelayInMinutes;
                StartShutDown();
            }
        }

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

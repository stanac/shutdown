using ShutDown.Data;
using ShutDown.Models;
using System;
using System.Windows.Input;
using System.Windows.Threading;
using ShutDown.MachineState;
using System.Collections.ObjectModel;
using System.Windows;
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
        private ICommand _cancelShutDownCommand;
        private ICommand _showSettingsCommand;
        private ICommand _showNewPatternViewCommand;
        private ICommand _saveNewPatternCommand;
        private ICommand _cancelNewPatternCommand;
        private ICommand _deletePatternCommand;
        private ICommand _startFromPatternCommand;
        private readonly IModifyMachineStateService _modifyMachineStateService;
        private int _delayMinutes = 60;
        private bool _shutDownInProgress;
        private string _shutDownRemainingTime;
        private string _operationName;
        private DispatcherTimer _timer;
        private DateTime _startTime;
        private string _newPatternName;
        private bool _newPatternViewVisible;
        private string _newPatternDescription;
        
        #endregion

        public TrayIcon TheTrayIcon = new TrayIcon();
        public event EventHandler CloseApp;
        
        public MainViewModel()
        {
            MouseJigglerHelper.Start();


            Title = WindowTitle;

            SettingsVisible = new ObservableProperty<bool>(nameof(SettingsVisible), this, false);
            Mediator.Instance.HideSettingsView = () => SettingsVisible.Value = false;
            Mediator.Instance.NewVersionCheckRequested = CheckForNewVersion;

            var settings = SettingsData.Instance;
            Operation = settings.DefaultOperation;
            DelayMinutes = settings.DefaultDelay;
            MinMinutes = settings.MinMinutes;
            MaxMinutes = settings.MaxMinutes;
            Force = settings.DefaultForce;

            IExecutor shutdownExecutor = new ShutdownExecutor();
            IExecutor standbyExecutor = new StandbyExecutor();
            _modifyMachineStateService = new ModifyMachineStateService(shutdownExecutor, standbyExecutor);

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
        public bool ShutDownInProgress
        {
            get => _shutDownInProgress;
            set
            {
                if (value != _shutDownInProgress)
                {
                    _shutDownInProgress = value;
                    RaisePropertyChanged(nameof(ShutDownInProgress));
                    TheTrayIcon.SetShutDownInProgress(value);
                }
            }
        }
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
      
        public string Version { get; } = AppVersion.CurrentVersion.Text;

        public bool IsNewVersionAvailable { get; set; }

        public string NewPatternName
        {
            get => _newPatternName;
            set
            {
                _newPatternName = value ?? "";
                RaisePropertyChanged(nameof(NewPatternName));
            }
        }
        public string NewPatternDescription
        {
            get => _newPatternDescription;
            set
            {
                _newPatternDescription = value;
                RaisePropertyChanged(nameof(NewPatternDescription));
            }
        }

        public bool NewPatternViewVisible
        {
            get => _newPatternViewVisible;
            set
            {
                _newPatternViewVisible = value;
                RaisePropertyChanged(nameof(NewPatternViewVisible));
            }
        }
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

        public ICommand CancelShutDownCommand => _cancelShutDownCommand ?? (_cancelShutDownCommand = new Command(CancelShutDown));

        public ICommand ShowSettingsCommand => _showSettingsCommand ?? (_showSettingsCommand = new Command(() => SettingsVisible.Value = true));

        public ICommand ShowNewPatternViewCommand => _showNewPatternViewCommand ?? (_showNewPatternViewCommand = new Command(ShowNewPatternView));

        public ICommand SavePatternCommand => _saveNewPatternCommand ?? (_saveNewPatternCommand = new Command(SaveNewPattern));

        public ICommand CancelNewPatternCommand => _cancelNewPatternCommand ?? (_cancelNewPatternCommand = new Command(CancelNewPattern));

        public ICommand DeletePatternCommand => _deletePatternCommand ?? (_deletePatternCommand = new Command<Guid>(DeletePattern));

        public ICommand StartFromPatternCommand => _startFromPatternCommand ?? (_startFromPatternCommand = new Command<Guid>(StartFromPattern));

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
            _startTime = DateTime.Now;
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(100)
            };
            _timer.Tick += (s, e) => OnTimerTick();
            _timer.Start();
            string opName = Operation.GetOperationName(Force);

            opName += " in:";
            OperationName = opName;
            ShutDownInProgress = true;
        }

        private void CancelShutDown()
        {
            _timer.Stop();
            ShutDownInProgress = false;
        }

        private void OnTimerTick()
        {
            try
            {
                var now = DateTime.Now;
                TimeSpan remaining = now - _startTime;
                if (remaining.TotalSeconds > DelayMinutes * 60)
                {
                    _timer.Stop();
                    _modifyMachineStateService.ModifyMachineState(Operation, Force);
                    RaiseCloseApp();
                }
                else
                {
                    remaining = TimeSpan.FromMinutes(DelayMinutes) - remaining;
                    ShutDownRemainingTime = $"{remaining.Hours.ToString("00")} : {remaining.Minutes.ToString("00")} : {remaining.Seconds.ToString("00")}";
                }
            }
            catch
            {
                //
            }
        }

        private void RaiseCloseApp()
        {
            var handler = CloseApp;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        private void ShowNewPatternView()
        {
            var pattern = new PatternModel
            {
                DelayInMinutes = DelayMinutes,
                Force = Force,
                Name = NewPatternName,
                Operation = Operation
            };

            var existingPattern = Patterns.FirstOrDefault(x => x.Description == pattern.Description);
            if (existingPattern != null)
            {
                MessageBox.Show($"There is already a pattern with same setting called '{existingPattern.Name}'", "Warning", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                return;
            }


            NewPatternName = Operation.GetOperationName(Force, true) + " " + TimeSpan.FromMinutes(DelayMinutes).ToFormatedString();
            NewPatternDescription = Operation.GetOperationName(Force) + " " + TimeSpan.FromMinutes(DelayMinutes).ToFormatedString();
            NewPatternViewVisible = true;
        }

        private void SaveNewPattern()
        {
            if (string.IsNullOrWhiteSpace(NewPatternName))
            {
                MessageBox.Show("Please enter the pattern name.", "Warning", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                return;
            }

            var pattern = new PatternModel
            {
                DelayInMinutes = DelayMinutes,
                Force = Force,
                Name = NewPatternName,
                Operation = Operation
            };

            Patterns.Add(pattern);
            PatternStore.Instance.SetPatterns(Patterns);
            NewPatternViewVisible = false;
        }

        private void CancelNewPattern()
        {
            NewPatternViewVisible = false;
        }

        private void DeletePattern(Guid id)
        {
            var toRemove = Patterns.FirstOrDefault(x => x.Id == id);
            if (toRemove != null)
            {
                if (MessageBox.Show($"Are you sure you want to delete pattern '{toRemove.Name}'?", "Delete", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    Patterns.Remove(toRemove);
                    PatternStore.Instance.SetPatterns(Patterns);
                }
            }
        }

        private void StartFromPattern(Guid id)
        {
            var pattern = Patterns.FirstOrDefault(x => x.Id == id);
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

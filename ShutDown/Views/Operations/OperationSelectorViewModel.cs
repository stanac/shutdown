using ShutDown.Data;
using ShutDown.Models;
using ShutDown.Utils;
using System;
using System.Linq;
using System.Windows.Input;

namespace ShutDown.Views.Operations
{
    public class OperationSelectorViewModel : ObservableObject
    {
        public ObservableProperty<ShutDownOperation> Operation { get; }
        public ObservableProperty<bool> Force { get; }
        public ObservableProperty<int> DelayInMinutes { get; }
        public ObservableProperty<string> DelayText { get; }

        public ICommand SelectOperationCommand { get; }
        public ICommand AddDelayCommand { get; }
        public ICommand ToggleForceCommand { get; }
        public ICommand StartShutDownCommand { get; }

        public OperationSelectorViewModel()
        {
            Operation = new ObservableProperty<ShutDownOperation>(nameof(Operation), this, SettingsData.Instance.DefaultOperation, value =>
            {
                if (DelayText != null)
                {
                    DelayText.Value = FormatDelayText();
                }

                SettingsData.Instance.DefaultOperation = value;
                SettingsData.Instance.Save();
            });
            Force = new ObservableProperty<bool>(nameof(Force), this, SettingsData.Instance.DefaultForce, value =>
            {
                if (DelayText != null)
                {
                    DelayText.Value = FormatDelayText();
                }

                SettingsData.Instance.DefaultForce = value;
                SettingsData.Instance.Save();
            });
            DelayInMinutes = new ObservableProperty<int>(nameof(DelayInMinutes), this, SettingsData.Instance.DefaultDelay, value =>
            {
                if (DelayText != null)
                {
                    DelayText.Value = FormatDelayText();
                }

                SettingsData.Instance.DefaultDelay = value;
                SettingsData.Instance.Save();
            });
            DelayText = new ObservableProperty<string>(nameof(DelayText), this, FormatDelayText());

            GlobalFunctions.Instance.GetCurrentOperation = () => new OperationModel
            {
                DelayInMinutes = DelayInMinutes.Value,
                Force = Force.Value,
                Operation = Operation.Value
            };
            GlobalFunctions.Instance.StartFromPattern = StartFromPattern;
            
            SelectOperationCommand = new Command<string>(SelectOperation);
            AddDelayCommand = new Command<string>(AddDelay);
            ToggleForceCommand = new Command(() => { Force.Value = !Force.Value; });
            StartShutDownCommand = new Command(StartShutDown);
        }

        private void SelectOperation(string value)
        {
            Operation.Value = (ShutDownOperation)Enum.Parse(typeof(ShutDownOperation), value, true);
        }

        private string FormatDelayText()
        {
            var ts = TimeSpan.FromMinutes(DelayInMinutes.Value);
            return $"{Math.Floor(ts.TotalHours):00}h {ts.Minutes:00}min";
        }

        private void AddDelay(string value)
        {
            int val = DelayInMinutes.Value + int.Parse(value);
            if (val < SettingsData.Instance.MinMinutes) val = SettingsData.Instance.MinMinutes;
            else if (val > SettingsData.Instance.MaxMinutes) val = SettingsData.Instance.MaxMinutes;
            DelayInMinutes.Value = val;
        }

        private void StartFromPattern(Guid id)
        {
            var pattern = PatternStore.Instance.GetPatterns().FirstOrDefault(x => x.Id == id);
            if (pattern != null)
            {
                Force.Value = pattern.Force;
                Operation.Value = pattern.Operation;
                DelayInMinutes.Value = pattern.DelayInMinutes;
                StartShutDown();
            }
        }

        private void StartShutDown()
        {
            GlobalFunctions.Instance.StartOperation();
        }

    }
}

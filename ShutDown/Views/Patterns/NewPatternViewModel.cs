using ShutDown.Data;
using ShutDown.Models;
using ShutDown.Utils;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using MessageBox = System.Windows.MessageBox;

namespace ShutDown.Views.Patterns
{
    public class NewPatternViewModel: ObservableObject
    {
        public ObservableProperty<string> NewPatternName { get; }
        public ObservableProperty<string> NewPatternDescription { get; }
        public ICommand SavePatternCommand { get; }
        public ICommand CancelNewPatternCommand { get; }

        public NewPatternViewModel()
        {
            NewPatternName = new ObservableProperty<string>(nameof(NewPatternName), this, "");
            NewPatternDescription = new ObservableProperty<string>(nameof(NewPatternDescription), this, "");

            SavePatternCommand = new Command(SaveNewPattern);

            CancelNewPatternCommand = new Command(CancelNewPattern);
        }

        private void SaveNewPattern()
        {
            if (string.IsNullOrWhiteSpace(NewPatternName.Value))
            {
                MessageBox.Show("Please enter the pattern name.", "Warning", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                return;
            }

            var op = GlobalFunctions.Instance.GetCurrentOperation();

            if (op == null)
            {
                Log.LogErrorAndDisplayMessageBox("Failed to get current operation");
                return;
            }

            var pattern = new PatternModel
            {
                DelayInMinutes = op.DelayInMinutes,
                Force = op.Force,
                Name = NewPatternName.Value,
                Operation = op.Operation
            };

            var patterns = PatternStore.Instance.GetPatterns().ToList();
            patterns.Add(pattern);

            PatternStore.Instance.SetPatterns(patterns);
            GlobalFunctions.Instance.PatternListChanged();
            CancelNewPattern();
        }

        private void CancelNewPattern()
        {
            GlobalFunctions.Instance.HideNewPatternView();
        }

        internal void OnShown(bool isNowVisible)
        {
            var op = GlobalFunctions.Instance.GetCurrentOperation();
            NewPatternDescription.Value = op.Operation.GetOperationName(op.Force) + " " + TimeSpan.FromMinutes(op.DelayInMinutes).ToFormatedString();
            NewPatternName.Value = op.Operation.GetOperationName(op.Force, true) + " " + op.DelayInMinutes;
        }
    }
}

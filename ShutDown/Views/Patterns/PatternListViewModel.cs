using System;
using ShutDown.Data;
using ShutDown.Models;
using ShutDown.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace ShutDown.Views.Patterns
{
    public class PatternListViewModel: ObservableObject
    {
        public ObservableProperty<IReadOnlyList<PatternModel>> Patterns { get; }
        public ICommand ShowNewPatternViewCommand { get; }
        public ICommand DeletePatternCommand { get; }
        public ICommand StartFromPatternCommand { get; }

        public PatternListViewModel()
        {
            Patterns = new ObservableProperty<IReadOnlyList<PatternModel>>(
                nameof(Patterns), this, PatternStore.Instance.GetPatterns()
            );

            GlobalFunctions.Instance.PatternListChanged = () => Patterns.Value = PatternStore.Instance.GetPatterns();

            ShowNewPatternViewCommand = new Command(GlobalFunctions.Instance.ShowNewPatternView);
            DeletePatternCommand = new Command<Guid>(DeletePattern);
            StartFromPatternCommand = new Command<Guid>(GlobalFunctions.Instance.StartFromPattern);
        }

        private void DeletePattern(Guid id)
        {
            var patterns = PatternStore.Instance.GetPatterns();
            var toRemove = patterns.FirstOrDefault(x => x.Id == id);

            if (toRemove != null)
            {
                if (MessageBox.Show($"Are you sure you want to delete pattern '{toRemove.Name}'?", "Delete", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    patterns = patterns.Where(x => x.Id != id).ToArray();

                    PatternStore.Instance.SetPatterns(patterns);

                    GlobalFunctions.Instance.PatternListChanged();
                }
            }
        }
    }
}

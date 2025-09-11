using ShutDown.Data;
using ShutDown.MachineState;
using ShutDown.Models;
using ShutDown.Utils;
using System;
using System.Windows.Input;
using System.Windows.Threading;

namespace ShutDown.Views.Operations
{
    public class CurrentOperationViewModel: ObservableObject
    {
        public ICommand CancelShutDownCommand { get; }
        public ObservableProperty<string> OperationName { get; }
        public ObservableProperty<string> RemainingTime { get; }
        public ObservableProperty<bool> IsCanceledDoToTimeGap { get; }

        private DateTimeOffset _startTime;
        private DateTimeOffset _nextTick;
        private readonly DispatcherTimer _timer = new DispatcherTimer();
        private OperationModel _operation;
        private readonly IModifyMachineStateService _modifyMachineStateService;

        public CurrentOperationViewModel()
        {
            CancelShutDownCommand = new Command(Cancel);
            OperationName = new ObservableProperty<string>(nameof(OperationName), this, "");
            RemainingTime = new ObservableProperty<string>(nameof(RemainingTime), this, "");
            IsCanceledDoToTimeGap = new ObservableProperty<bool>(nameof(IsCanceledDoToTimeGap), this, false);

            IExecutor shutdownExecutor = new ShutdownExecutor();
            IExecutor standbyExecutor = new StandbyExecutor();
            _modifyMachineStateService = new ModifyMachineStateService(shutdownExecutor, standbyExecutor);

            GlobalFunctions.Instance.StartOperation = Start;
            GlobalFunctions.Instance.CancelOperationRequested = Cancel;

            _timer.Interval = TimeSpan.FromMilliseconds(100);
            _timer.Tick += (s, _) => OnTick();
        }

        private void OnTick()
        {
            if (_operation == null || IsCanceledDoToTimeGap.Value)
            {
                return;
            }

            try
            {
                var now = DateTimeOffset.UtcNow;
                TimeSpan remaining = now - _startTime;

                if (Math.Abs((_nextTick - now).TotalSeconds) > 5.0)
                {
                    IsCanceledDoToTimeGap.Value = true;
                    RemainingTime.Value = "Cancelled do to time gap";
                    _timer.Stop();
                    return;
                }

                _nextTick = now.Add(_timer.Interval);

                if (remaining.TotalSeconds > _operation.DelayInMinutes * 60)
                {
                    _timer.Stop();
                    _modifyMachineStateService.ModifyMachineState(_operation.Operation, _operation.Force);
                    GlobalFunctions.Instance.ExitApp();
                }
                else
                {
                    remaining = TimeSpan.FromMinutes(_operation.DelayInMinutes) - remaining;
                    RemainingTime.Value = $"{remaining.Hours:00} : {remaining.Minutes:00} : {remaining.Seconds:00}";
                }
            }
            catch
            {
                //
            }
        }

        private void Start()
        {
            IsCanceledDoToTimeGap.Value = false;
            _timer.Stop();
            _operation = null;

            _startTime = DateTimeOffset.UtcNow;
            _nextTick = DateTimeOffset.UtcNow;
            _timer.Start();
            _operation = GlobalFunctions.Instance.GetCurrentOperation();

            if (_operation == null)
            {
                Log.LogErrorAndDisplayMessageBox("Failed to get current operation");
                return;
            }

            string opName = _operation.Operation.GetOperationName(_operation.Force);

            opName += " in:";
            OperationName.Value = opName;
        }

        private void Cancel()
        {
            _timer.Stop();

            GlobalFunctions.Instance.HideCurrentOperationView();
        }
    }
}

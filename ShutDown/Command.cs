using System;
using System.Windows.Input;

namespace ShutDown
{
    public class Command : ICommand
    {
        private readonly Action _action;

        public Command(Action action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            _action = action;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _action();
        }
    }

    public class Command<T> : ICommand
    {
        private readonly Action<T> _action;

        public Command(Action<T> action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            _action = action;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _action((T)parameter);
        }
    }
}

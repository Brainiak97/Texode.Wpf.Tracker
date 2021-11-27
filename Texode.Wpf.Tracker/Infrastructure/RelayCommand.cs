using System;
using System.Windows.Input;

namespace Texode.Wpf.Tracker.Infrastructure
{
    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly bool _canExecute;

        public RelayCommand(Action executeMethod)
        {
            _execute = executeMethod;
        }

        public RelayCommand(Action executeMethod, bool canExecuteMethod)
        {
            _execute = executeMethod;
            _canExecute = canExecuteMethod;
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged(this, EventArgs.Empty);
        }

        bool ICommand.CanExecute(object parameter)
        {
            if (_canExecute)
            {
                return _canExecute;
            }
            if (_execute != null)
            {
                return true;
            }
            return false;
        }

        public event EventHandler CanExecuteChanged = delegate { };

        void ICommand.Execute(object parameter)
        {
            _execute?.Invoke();
        }
    }

    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T> _execute;
        private readonly bool _canExecute;

        public RelayCommand(Action<T> executeMethod)
        {
            _execute = executeMethod;
        }

        public RelayCommand(Action<T> executeMethod, bool canExecuteMethod)
        {
            _execute = executeMethod;
            _canExecute = canExecuteMethod;
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged(this, EventArgs.Empty);
        }

        bool ICommand.CanExecute(object parameter)
        {
            if (_canExecute)
            {
                return _canExecute;
            }
            if (_execute != null)
            {
                return true;
            }
            return false;
        }

        public event EventHandler CanExecuteChanged = delegate { };

        void ICommand.Execute(object parameter)
        {
            _execute?.Invoke((T)parameter);
        }
    }
}

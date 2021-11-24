using System;

namespace Texode.Wpf.Tracker.Infrastructure
{
    public interface ICommand
    {
        event EventHandler CanExecuteChanged;
        void Execute(object parameter);
        bool CanExecute(object parameter);
    }
}

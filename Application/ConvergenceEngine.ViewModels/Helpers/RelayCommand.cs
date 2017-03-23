using System;
using System.Windows.Input;

namespace ConvergenceEngine.ViewModels.Helpers {

    public class RelayCommand : ICommand {

        readonly Action<object> execute;
        readonly Predicate<object> canExecute;

        public RelayCommand(Action<object> execute)
            : this(execute, null) {
        }

        public RelayCommand(Action<object> execute, Predicate<object> canExecute = null) {
            this.execute = execute ?? throw new ArgumentNullException("execute");
            this.canExecute = canExecute;
        }

        public bool CanExecute(object parameter) {
            return canExecute == null ? true : canExecute.Invoke(parameter);
        }

        public event EventHandler CanExecuteChanged {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter) {
            execute.Invoke(parameter);
        }
    }
}
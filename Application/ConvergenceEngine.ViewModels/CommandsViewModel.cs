using ConvergenceEngine.ViewModels.Helpers;
using System;
using System.Windows;
using System.Windows.Input;

namespace ConvergenceEngine.ViewModels {

    public abstract class CommandsViewModel : ApplicationViewModel {

        private ICommand openFileCommand;
        private ICommand closeFileCommand;
        private ICommand exitApplicationCommand;

        public ICommand OpenFile {
            get { return openFileCommand; }
            set { Set(ref openFileCommand, value); }
        }
        public ICommand CloseFile {
            get { return closeFileCommand; }
            set { Set(ref closeFileCommand, value); }
        }
        public ICommand ExitApplication {
            get { return exitApplicationCommand; }
            set { Set(ref exitApplicationCommand, value); }
        }

        protected virtual void InitializeCommands() {
            OpenFile = new RelayCommand(ExecuteOpenFileCommand, CanExecuteOpenFileCommand);
            CloseFile = new RelayCommand(ExecuteCloseFileCommand, CanExecuteCloseFileCommand);
            ExitApplication = new RelayCommand(w => (w as Window)?.Close());
        }

        private bool CanExecuteOpenFileCommand(object obj) {
            throw new NotImplementedException();
        }

        private void ExecuteOpenFileCommand(object obj) {
            throw new NotImplementedException();
        }

        private void ExecuteCloseFileCommand(object obj) {
            throw new NotImplementedException();
        }

        private bool CanExecuteCloseFileCommand(object obj) {
            throw new NotImplementedException();
        }
    }
}
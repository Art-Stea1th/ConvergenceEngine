using SLAM.ViewModels.Helpers;
using System.Windows;
using System.Windows.Input;

namespace SLAM.ViewModels {

    public abstract class CommandsViewModel : ViewModelBase {

        private ICommand openFileCommand;
        private ICommand closeFileCommand;
        private ICommand nextFrameCommand;
        private ICommand prevFrameCommand;
        private ICommand exitApplicationCommand;

        private ICommand showRawDataWindowCommand;
        private ICommand showPointsDataWindowCommand;
        private ICommand showLinearDataWindowCommand;

        public ICommand OpenFile {
            get { return openFileCommand; }
            protected set { Set(ref openFileCommand, value); }
        }
        public ICommand CloseFile {
            get { return closeFileCommand; }
            protected set { Set(ref closeFileCommand, value); }
        }
        public ICommand NextFrame {
            get { return nextFrameCommand; }
            protected set { Set(ref nextFrameCommand, value); }
        }
        public ICommand PrevFrame {
            get { return prevFrameCommand; }
            protected set { Set(ref prevFrameCommand, value); }
        }
        public ICommand ExitApplication {
            get { return exitApplicationCommand; }
            protected set { Set(ref exitApplicationCommand, value); }
        }
        public ICommand ShowRawDataWindow {
            get { return showRawDataWindowCommand; }
            protected set { Set(ref showRawDataWindowCommand, value); }
        }
        public ICommand ShowPointsDataWindow {
            get { return showPointsDataWindowCommand; }
            protected set { Set(ref showPointsDataWindowCommand, value); }
        }
        public ICommand ShowLinearDataWindow {
            get { return showLinearDataWindowCommand; }
            protected set { Set(ref showLinearDataWindowCommand, value); }
        }

        protected void InitializeCommands() {
            OpenFile = new RelayCommand(ExecuteOpenFileCommand, CanExecuteOpenFileCommand);
            CloseFile = new RelayCommand(ExecuteCloseFileCommand, CanExecuteCloseFileCommand);
            NextFrame = new RelayCommand(ExecuteNextFrameCommand, CanExecuteNavigationsCommand);
            PrevFrame = new RelayCommand(ExecutePrevFrameCommand, CanExecuteNavigationsCommand);
            ExitApplication = new RelayCommand(w => ((Window)w).Close());
        }        

        protected abstract bool CanExecuteOpenFileCommand(object obj);
        protected abstract void ExecuteOpenFileCommand(object obj);

        protected abstract bool CanExecuteCloseFileCommand(object obj);
        protected abstract void ExecuteCloseFileCommand(object obj);

        protected abstract bool CanExecuteNavigationsCommand(object obj);
        protected abstract void ExecuteNextFrameCommand(object obj);
        protected abstract void ExecutePrevFrameCommand(object obj);

        protected virtual void ExecuteNewWindowCommand(object obj) {
            CreateWindow(obj as ViewModelBase, this);
        }

        protected virtual bool CanExecuteNewWindowCommand(object obj) {
            return !WindowExists(obj as ViewModelBase);
        }
    }
}
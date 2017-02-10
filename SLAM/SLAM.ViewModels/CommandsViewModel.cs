using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace SLAM.ViewModels {

    using Helpers;

    public abstract class CommandsViewModel : ApplicationDataViewModel {

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
            set { Set(ref openFileCommand, value); }
        }
        public ICommand CloseFile {
            get { return closeFileCommand; }
            set { Set(ref closeFileCommand, value); }
        }
        public ICommand NextFrame {
            get { return nextFrameCommand; }
            set { Set(ref nextFrameCommand, value); }
        }
        public ICommand PrevFrame {
            get { return prevFrameCommand; }
            set { Set(ref prevFrameCommand, value); }
        }
        public ICommand ExitApplication {
            get { return exitApplicationCommand; }
            set { Set(ref exitApplicationCommand, value); }
        }
        public ICommand ShowRawDataWindow {
            get { return showRawDataWindowCommand; }
            set { Set(ref showRawDataWindowCommand, value); }
        }
        public ICommand ShowPointsDataWindow {
            get { return showPointsDataWindowCommand; }
            set { Set(ref showPointsDataWindowCommand, value); }
        }
        public ICommand ShowLinearDataWindow {
            get { return showLinearDataWindowCommand; }
            set { Set(ref showLinearDataWindowCommand, value); }
        }

        protected virtual void InitializeCommands() {
            OpenFile = new RelayCommand(ExecuteOpenFileCommand, CanExecuteOpenFileCommand);
            CloseFile = new RelayCommand(ExecuteCloseFileCommand, CanExecuteCloseFileCommand);
            NextFrame = new RelayCommand(ExecuteNextFrameCommand, CanExecuteNavigationsCommand);
            PrevFrame = new RelayCommand(ExecutePrevFrameCommand, CanExecuteNavigationsCommand);
            ExitApplication = new RelayCommand(w => (w as Window)?.Close());
        }

        protected void AfterOpenCloseFile() {
            InitializeData();
            InitializeCommands();
        }

        private bool CanExecuteOpenFileCommand(object obj) {
            return ModelReady;
        }

        private async void ExecuteOpenFileCommand(object obj) {

            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.Title = "Open RAW Depth Stream Data file";
            openFileDialog.Multiselect = false;
            openFileDialog.DefaultExt = ".rdsd";
            openFileDialog.Filter = " RAW Depth Stream Data |*.rdsd| All Files |*.*";
            openFileDialog.ValidateNames = true;

            bool? result = openFileDialog.ShowDialog();

            if (result == true) {
                if (openFileDialog.FileName != currentFilePath) {
                    if (model.Start(openFileDialog.FileName)) {
                        CurrentFileName = openFileDialog.FileName;
                        await model.CalculateFramesCountAsync();
                        AfterOpenCloseFile();
                        CurrentFrame = 0;
                    }
                    else {
                        string fileName = Path.GetFileName(openFileDialog.FileName);
                        MessageBox.Show(
                            $"The content of \"{fileName}\" file corrupted, or the file has the wrong type.",
                            "Open RAW Depth Stream Data file",
                            MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                }
            }
        }

        private void ExecuteCloseFileCommand(object obj) {
            TotalFramesCount = 0;
            CurrentFileName = string.Empty;
            model.Stop();
            AfterOpenCloseFile();
        }

        private bool CanExecuteCloseFileCommand(object obj) {
            return TotalFramesCount > 0;
        }

        private void ExecutePrevFrameCommand(object obj) {
            if (CurrentFrame > 0) { --CurrentFrame; }
        }

        private void ExecuteNextFrameCommand(object obj) {
            if (CurrentFrame < TotalFramesCount && model.Ready) { ++CurrentFrame; }
        }

        private bool CanExecuteNavigationsCommand(object obj) {
            return TotalFramesCount > 0;
        }

        protected void ExecuteNewWindowCommand(object obj) {
            CreateWindow(obj as ViewModelBase, this);
        }

        protected bool CanExecuteNewWindowCommand(object obj) {
            return !WindowExists(obj as ViewModelBase);
        }
    }
}
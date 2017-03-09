using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace ConvergenceEngine.ViewModels {

    using Helpers;
    using Infrastructure;
    using Models.IO;
    using Models.Mapping;

    public abstract class CommandsViewModel : ApplicationViewModel {

        private ICommand openFileCommand;
        private ICommand closeFileCommand;
        private ICommand exitApplicationCommand;
        private ICommand startStopResetCommand;

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
        public ICommand StartStopReset {
            get { return startStopResetCommand; }
            set { Set(ref startStopResetCommand, value); }
        }

        protected virtual void InitializeCommands() {
            OpenFile = new RelayCommand(ExecuteOpenFileCommand, CanExecuteOpenFileCommand);
            CloseFile = new RelayCommand(ExecuteCloseFileCommand, CanExecuteCloseFileCommand);
            StartStopReset = new RelayCommand(ExecuteStartStopResetCommand, CanExecuteStartStopResetCommand);
            ExitApplication = new RelayCommand(w => (w as Window)?.Close());
        }

        private bool CanExecuteOpenFileCommand(object obj) {
            return DataProvider == null || DataProvider.State == DataProviderStates.Stopped;
        }
        private bool CanExecuteCloseFileCommand(object obj) {
            return DataProvider != null;
        }
        private bool CanExecuteStartStopResetCommand(object obj) {
            return DataProvider != null;
        }

        private void ExecuteOpenFileCommand(object obj) {

            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.Title = "Open RAW Depth Stream Data file";
            openFileDialog.Multiselect = false;
            openFileDialog.DefaultExt = ".rdsd";
            openFileDialog.Filter = " RAW Depth Stream Data |*.rdsd| All Files |*.*";
            openFileDialog.ValidateNames = true;

            bool? result = openFileDialog.ShowDialog();

            if (result == true) {
                Reset(true);
                DataProvider = KinectFileReader.CreateReader(openFileDialog.FileName);
                if (DataProvider == null) {
                    string fileName = Path.GetFileName(openFileDialog.FileName);
                    MessageBox.Show(
                        $"The content of \"{fileName}\" file corrupted, or the file has the wrong type.",
                        "Open RAW Depth Stream Data file",
                        MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else {
                    DataProvider.FPS = FpsCurrent;
                }
            }
        }
        private void ExecuteCloseFileCommand(object obj) {
            Reset(true);
        }
        private void ExecuteStartStopResetCommand(object obj) {
            if (Mapper == null) {
                Mapper = new Mapper();
                DataProvider?.Start();
                return;
            }
            if (ModelStarted) {
                DataProvider?.Stop();
            }
            else {
                var result = MessageBox.Show($"Are you sure you want to reset?", "Reset Map",
                        MessageBoxButton.YesNo, MessageBoxImage.Information, MessageBoxResult.No);
                if (result == MessageBoxResult.Yes) {
                    Reset();
                }
            }
        }

        private void Reset(bool full = false) {
            DataProvider?.Stop();
            if (full) {
                DataProvider = null;
            }
            Mapper = null;            
            FullFrame = null;
            TotalFrames = 0;
            UpdateStartStopResetButtonText();
        }        
    }
}
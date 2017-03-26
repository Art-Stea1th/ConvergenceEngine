using System.IO;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;

namespace ConvergenceEngine.ViewModels {

    using Helpers;
    using Infrastructure;
    using Models.IO;
    using Models.Mapping;

    public abstract class CommandsViewModel : ApplicationViewModel {

        private ICommand _openFileCommand;
        private ICommand _closeFileCommand;
        private ICommand _exitApplicationCommand;
        private ICommand _startStopResetCommand;

        public ICommand OpenFile {
            get => _openFileCommand;
            set => Set(ref _openFileCommand, value);
        }
        public ICommand CloseFile {
            get => _closeFileCommand;
            set => Set(ref _closeFileCommand, value);
        }
        public ICommand ExitApplication {
            get => _exitApplicationCommand;
            set => Set(ref _exitApplicationCommand, value);
        }
        public ICommand StartStopReset {
            get => _startStopResetCommand;
            set => Set(ref _startStopResetCommand, value);
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

            var openFileDialog = new OpenFileDialog() {
                Title = "Open RAW Depth Stream Data file",
                Multiselect = false,
                DefaultExt = ".rdsd",
                Filter = " RAW Depth Stream Data |*.rdsd| All Files |*.*",
                ValidateNames = true
            };

            if (openFileDialog.ShowDialog() == true) {
                var newDataProvider = KinectFileReader.CreateReader(openFileDialog.FileName);
                if (newDataProvider == null) {
                    string fileName = Path.GetFileName(openFileDialog.FileName);
                    MessageBox.Show(
                        $"The content of \"{fileName}\" file corrupted, or the file has the wrong type.",
                        "Open RAW Depth Stream Data file",
                        MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else {
                    Reset(true);
                    DataProvider = newDataProvider;
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
            FullFrame = null;
            Mapper = null;
            //MapperData = null;
            ActualSegments = null;
            MapSegments = null;
            TotalFrames = 0;
            UpdateStartStopResetButtonText();
        }        
    }
}
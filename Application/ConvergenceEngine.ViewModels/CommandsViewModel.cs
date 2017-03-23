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
            get => openFileCommand;
            set => Set(ref openFileCommand, value);
        }
        public ICommand CloseFile {
            get => closeFileCommand;
            set => Set(ref closeFileCommand, value);
        }
        public ICommand ExitApplication {
            get => exitApplicationCommand;
            set => Set(ref exitApplicationCommand, value);
        }
        public ICommand StartStopReset {
            get => startStopResetCommand;
            set => Set(ref startStopResetCommand, value);
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

            OpenFileDialog openFileDialog = new OpenFileDialog() {
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
using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;


namespace SLAM.ViewModels {

    using Helpers;
    using Models;


    public class MainVindowViewModel : ViewModelBase {

        private Model model;

        private WriteableBitmap mapViewportData;
        private WriteableBitmap topDepthViewportData;
        private WriteableBitmap frontDepthViewportData;

        private string modelCurrentState;
        private bool modelReady;
        private string currentFilePath;
        private int totalFramesCount;
        private int currentFrame;

        private DateTime lastTimeOfFrameUpdate;
        private TimeSpan frameUpdateLimit;

        private ICommand openFileCommand;
        private ICommand closeFileCommand;
        private ICommand nextFrameCommand;
        private ICommand prevFrameCommand;
        private ICommand exitApplicationCommand;

        public ImageSource MapViewportData {
            get { return mapViewportData; }
            set { Set(ref mapViewportData, (WriteableBitmap)value); }
        }
        public ImageSource TopDepthViewportData {
            get { return topDepthViewportData; }
            set { Set(ref topDepthViewportData, (WriteableBitmap)value); }
        }
        public ImageSource FrontDepthViewportData {
            get { return frontDepthViewportData; }
            set { Set(ref frontDepthViewportData, (WriteableBitmap)value); }
        }

        public string ModelCurrentState {
            get { return modelCurrentState; }
            private set { Set(ref modelCurrentState, value); }
        }
        public bool ModelReady {
            get { return modelReady; }
            private set { Set(ref modelReady, value); }
        }
        public string CurrentFileName {
            get { return Path.GetFileName(currentFilePath); }
            private set { Set(ref currentFilePath, value); }
        }
        public int TotalFramesCount {
            get { return totalFramesCount; }
            private set { Set(ref totalFramesCount, value); }
        }
        public int CurrentFrame {
            get { return currentFrame; }
            set { Set(ref currentFrame, value); UpdateViewports(); }
        }

        public ICommand OpenFile {
            get { return openFileCommand; }
            private set { Set(ref openFileCommand, value); }
        }
        public ICommand CloseFile {
            get { return closeFileCommand; }
            private set { Set(ref closeFileCommand, value); }
        }
        public ICommand NextFrame {
            get { return nextFrameCommand; }
            private set { Set(ref nextFrameCommand, value); }
        }
        public ICommand PrevFrame {
            get { return prevFrameCommand; }
            private set { Set(ref prevFrameCommand, value); }
        }
        public ICommand ExitApplication {
            get { return exitApplicationCommand; }
            private set { Set(ref exitApplicationCommand, value); }
        }

        public MainVindowViewModel() {
            model = new Model(UpdateUI);
            lastTimeOfFrameUpdate = DateTime.Now;
            frameUpdateLimit = TimeSpan.FromMilliseconds(1000.0 / 29.97);
            InitializeViewports();
            UpdateUI();
        }

        private void InitializeViewports() {
            MapViewportData = new WriteableBitmap(640, 480, 96.0, 96.0, PixelFormats.Bgr32, null);
            TopDepthViewportData = new WriteableBitmap(640, 480, 96.0, 96.0, PixelFormats.Bgr32, null);
            FrontDepthViewportData = new WriteableBitmap(640, 480, 96.0, 96.0, PixelFormats.Bgr32, null);

        }

        private void UpdateViewports() {

            if ((DateTime.Now - lastTimeOfFrameUpdate) >= frameUpdateLimit) {

                byte[] mapViewportPixels = model.GetViewportTopDepthFrame(CurrentFrame);
                byte[] topDepthViewportPixels = model.GetViewportTopDepthFrame(CurrentFrame);
                byte[] frontViewportPixels = model.GetViewportFrontDepthFrame(CurrentFrame);

                if (mapViewportPixels != null) {
                    mapViewportData.WritePixels(
                        new Int32Rect(0, 0, 640, 480),
                        mapViewportPixels, topDepthViewportData.PixelWidth * sizeof(int), 0);
                }
                if (topDepthViewportPixels != null) {
                    topDepthViewportData.WritePixels(
                        new Int32Rect(0, 0, 640, 480),
                        topDepthViewportPixels, topDepthViewportData.PixelWidth * sizeof(int), 0);
                }
                if (frontViewportPixels != null) {
                    frontDepthViewportData.WritePixels(
                        new Int32Rect(0, 0, 640, 480),
                        frontViewportPixels, frontDepthViewportData.PixelWidth * sizeof(int), 0);
                }
                lastTimeOfFrameUpdate = DateTime.Now;
            }
        }

        private void UpdateUI() {
            InitializeProperties();
            InitializeCommands();
        }

        private void InitializeProperties() {
            ModelCurrentState = model.CurrentState;
            ModelReady = model.Ready;
            CurrentFileName = model.FullFileName;
            TotalFramesCount = model.FramesCount;
        }

        private void InitializeCommands() {
            OpenFile = new RelayCommand(ExecuteOpenFileCommand, CanExecuteOpenFileCommand);
            CloseFile = new RelayCommand(ExecuteCloseFileCommand, CanExecuteCloseFileCommand);
            NextFrame = new RelayCommand(ExecuteNextFrameCommand, CanExecuteNavigationsCommand);
            PrevFrame = new RelayCommand(ExecutePrevFrameCommand, CanExecuteNavigationsCommand);
            ExitApplication = new RelayCommand(w => ((Window)w).Close());
        }

        private bool CanExecuteOpenFileCommand(object obj) {
            return ModelReady;
        }

        private void ExecutePrevFrameCommand(object obj) {
            if (CurrentFrame > 0) { --CurrentFrame; }
        }

        private void ExecuteNextFrameCommand(object obj) {
            if (CurrentFrame < TotalFramesCount) { ++CurrentFrame; }
        }

        private bool CanExecuteNavigationsCommand(object obj) {
            return TotalFramesCount > 0;
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
                    if (model.OpenFile(openFileDialog.FileName)) {
                        CurrentFileName = openFileDialog.FileName;
                        await model.CalculateFramesCount();
                        CurrentFrame = 0;
                    }
                    else {
                        MessageBox.Show(
                            "The content of this file corrupted, or the file has the wrong type.",
                            "Open RAW Depth Stream Data file",
                            MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                }
            }
        }

        private void ExecuteCloseFileCommand(object obj) {
            TotalFramesCount = 0;
            CurrentFileName = string.Empty;
            model.CloseFile();
            InitializeViewports();
        }

        private bool CanExecuteCloseFileCommand(object obj) {
            return TotalFramesCount > 0;
        }
    }
}
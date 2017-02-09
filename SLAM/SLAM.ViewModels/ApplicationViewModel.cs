using System.IO;
using Microsoft.Win32;
using System.Windows;

namespace SLAM.ViewModels {

    using Models.Mapping;

    public abstract class ApplicationViewModel : CommandsViewModel {

        protected Model model;

        private string modelCurrentState;
        private bool modelReady;
        private string currentFilePath;
        private int totalFramesCount;
        private int currentFrame;

        public string ModelCurrentState {
            get { return modelCurrentState; }
            protected set { Set(ref modelCurrentState, value); }
        }
        public bool ModelReady {
            get { return modelReady; }
            protected set { Set(ref modelReady, value); }
        }
        public string CurrentFileName {
            get { return Path.GetFileName(currentFilePath); }
            protected set { Set(ref currentFilePath, value); }
        }
        public int TotalFramesCount {
            get { return totalFramesCount; }
            protected set { Set(ref totalFramesCount, value); }
        }
        public int CurrentFrame {
            get { return currentFrame; }
            set { Set(ref currentFrame, value); UpdateViewports(); }
        }

        protected ApplicationViewModel() {
            model = new Model(UpdateUI);
        }

        protected void InitializeProperties() {
            ModelCurrentState = model.CurrentStateInfo;
            ModelReady = model.Ready;
            TotalFramesCount = model.FramesCount;
        }

        protected void UpdateUI() {
            InitializeProperties();
            InitializeCommands();
        }

        protected abstract void InitializeViewports();
        protected abstract void UpdateViewports();

        protected override bool CanExecuteOpenFileCommand(object obj) {
            return ModelReady;
        }

        protected override void ExecutePrevFrameCommand(object obj) {
            if (CurrentFrame > 0) { --CurrentFrame; }
        }

        protected override void ExecuteNextFrameCommand(object obj) {
            if (CurrentFrame < TotalFramesCount && model.Ready) { ++CurrentFrame; }
        }

        protected override bool CanExecuteNavigationsCommand(object obj) {
            return TotalFramesCount > 0;
        }

        protected override async void ExecuteOpenFileCommand(object obj) {

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

        protected override void ExecuteCloseFileCommand(object obj) {
            TotalFramesCount = 0;
            CurrentFileName = string.Empty;
            model.Stop();
            InitializeViewports();
        }

        protected override bool CanExecuteCloseFileCommand(object obj) {
            return TotalFramesCount > 0;
        }
    }
}
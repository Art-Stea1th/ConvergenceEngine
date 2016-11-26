using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Microsoft.Win32;


namespace SLAM.ViewModels {

    using Helpers;
    using Models;
    using System.Threading.Tasks;

    public class MainVindowViewModel : ViewModelBase {

        private Model model;
        private bool modelReady;
        private string modelCurrentState;

        private WriteableBitmap firstViewportData;
        private WriteableBitmap secondViewportData;

        private string currentFilePath;
        private int totalFramesCount;
        private int currentFrame;

        private ICommand openFileCommand;
        private ICommand closeFileCommand;
        private ICommand exitApplicationCommand;

        public MainVindowViewModel() {
            model = new Model(LockUI, UnlockUI);            
            currentFilePath = string.Empty;
            currentFrame = 0;
            //UnlockUI();
        }

        public string ModelCurrentState {
            get {
                return modelCurrentState;
            }
            private set {
                modelCurrentState = value;
                OnPropertyChanged(GetMemberName((MainVindowViewModel vm) => vm.ModelCurrentState));
            }
        }

        public bool ModelReady {
            get {
                return modelReady;
            }
            private set {
                modelReady = value;
                OnPropertyChanged(GetMemberName((MainVindowViewModel vw) => vw.ModelReady));
            }
        }

        public string CurrentFileName {
            get {
                return Path.GetFileName(currentFilePath);
            }
            private set {
                currentFilePath = value;
                OnPropertyChanged(GetMemberName((MainVindowViewModel vm) => vm.CurrentFileName));
            }
        }

        public int TotalFramesCount {
            get {
                return totalFramesCount;
            }
            private set {
                totalFramesCount = value;
                OnPropertyChanged(GetMemberName((MainVindowViewModel vm) => vm.TotalFramesCount));
            }
        }

        public int CurrentFrame {
            get {
                return currentFrame;
            }
            set {
                currentFrame = value;
                OnPropertyChanged(GetMemberName((MainVindowViewModel vm) => vm.currentFrame));
            }
        }

        public ICommand OpenFile {
            get {
                return
                    (openFileCommand) ??
                    (openFileCommand =
                    new RelayCommand(ExecuteOpenFileCommand));
            }
        }        

        public ICommand CloseFile {
            get {
                return
                    (closeFileCommand) ??
                    (closeFileCommand =
                    new RelayCommand(ExecuteCloseFileCommand));
            }
        }        

        public ICommand ExitApplication {
            get {
                return
                    (exitApplicationCommand) ??
                    (exitApplicationCommand =
                    new RelayCommand(w => ((Window)w).Close()));
            }
        }

        private async void ExecuteOpenFileCommand(object obj) {

            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.Title = "Open RAW Depth Stream Data file";
            openFileDialog.Multiselect = false;
            openFileDialog.DefaultExt = ".rdsd";
            openFileDialog.Filter = " RAW Depth Stream Data |*.rdsd";
            openFileDialog.ValidateNames = true;

            bool? result = openFileDialog.ShowDialog();

            if (result == true) {
                CurrentFileName = openFileDialog.FileName;
                if (model.OpenFile(currentFilePath)) {
                    TotalFramesCount = await model.CalculateFramesCount();
                }
            }
        }

        private void ExecuteCloseFileCommand(object obj) {
            CurrentFileName = string.Empty;
        }

        private void LockUI(string processName) {
            ModelReady = false;
            ModelCurrentState = processName;
        }

        private void UnlockUI() {
            ModelReady = true;
            ModelCurrentState = "Ready";
        }
    }
}
using System.Windows.Media;

namespace ConvergenceEngine.ViewModels {

    public sealed class MainViewModel : CommandsViewModel {

        private bool _viewportSettingsVisible;
        private bool _dataProviderSettingsVisible;
        private bool _fullFrameViewportVisible;

        private double _fpsCurrent;
        private bool _modelStarted;
        private int _totatFrames;
        private int _currentFrame;
        private string _startStopResetButtonText;

        public DoubleCollection FpsCollection { get; private set; }

        public bool ViewportSettingsVisible {
            get => _viewportSettingsVisible;
            set => Set(ref _viewportSettingsVisible, value);
        }
        public bool DataProviderSettingsVisible {
            get => _dataProviderSettingsVisible;
            set => Set(ref _dataProviderSettingsVisible, value);
        }
        public bool FullFrameViewportVisible {
            get => _fullFrameViewportVisible;
            set {
                Set(ref _fullFrameViewportVisible, value);
                SwitchFullFrameViewportVisibility(value);
            }
        }

        public override double FpsCurrent {
            get => _fpsCurrent;
            set {
                if (DataProvider != null) { Set(ref _fpsCurrent, DataProvider.FPS = value); return; }
                Set(ref _fpsCurrent, value);
            }
        }
        public override bool ModelStarted {
            get => _modelStarted;
            set => Set(ref _modelStarted, value);
        }
        public override int TotalFrames {
            get => _totatFrames;
            set => Set(ref _totatFrames, value);
        }
        public override int CurrentFrame {
            get => _currentFrame;
            set => Set(ref _currentFrame, value);
        }
        public override string StartStopResetButtonText {
            get => _startStopResetButtonText;
            set => Set(ref _startStopResetButtonText, value);
        }

        public MainViewModel() {
            InitializeCommands();
            InitializeViews();
            Initialize();
        }

        private void InitializeViews() {
            _viewportSettingsVisible = true;
            _dataProviderSettingsVisible = true;
            _fullFrameViewportVisible = true;
        }

        private void Initialize() {
            FpsCollection = new DoubleCollection { 1, 2, 3, 5, 10, 15, 20, 30, 50, 60 };
            FpsCurrent = 30.0;
            _modelStarted = false;
            UpdateStartStopResetButtonText();
        }

        protected override void UpdateStartStopResetButtonText() {
            if (Mapper == null) {
                StartStopResetButtonText = "START";
                return;
            }
            if (ModelStarted) {
                StartStopResetButtonText = "STOP";
                return;
            }
            StartStopResetButtonText = "RESET";
        }

        private void SwitchFullFrameViewportVisibility(bool visible) {
            if (DataProvider != null) {
                DataProvider.OnNextFullFrameReady -= UpdateFullFrame;
                if (visible) {
                    DataProvider.OnNextFullFrameReady += UpdateFullFrame;
                }
            }
        }
    }
}
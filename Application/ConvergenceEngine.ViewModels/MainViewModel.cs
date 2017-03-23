using System.Windows.Media;

namespace ConvergenceEngine.ViewModels {

    public sealed class MainViewModel : CommandsViewModel {

        private bool viewportSettingsVisible;
        private bool dataProviderSettingsVisible;
        private bool fullFrameViewportVisible;

        private double fpsCurrent;
        private bool modelStarted;
        private int totatFrames;
        private int currentFrame;
        private string startStopResetButtonText;

        public DoubleCollection FpsCollection { get; private set; }

        public bool ViewportSettingsVisible {
            get => viewportSettingsVisible;
            set => Set(ref viewportSettingsVisible, value);
        }
        public bool DataProviderSettingsVisible {
            get => dataProviderSettingsVisible;
            set => Set(ref dataProviderSettingsVisible, value);
        }
        public bool FullFrameViewportVisible {
            get => fullFrameViewportVisible;
            set {
                Set(ref fullFrameViewportVisible, value);
                SwitchFullFrameViewportVisibility(value);
            }
        }

        public override double FpsCurrent {
            get => fpsCurrent;
            set {
                if (DataProvider != null) { Set(ref fpsCurrent, DataProvider.FPS = value); return; }
                Set(ref fpsCurrent, value);
            }
        }
        public override bool ModelStarted {
            get => modelStarted;
            set => Set(ref modelStarted, value);
        }
        public override int TotalFrames {
            get => totatFrames;
            set => Set(ref totatFrames, value);
        }
        public override int CurrentFrame {
            get => currentFrame;
            set => Set(ref currentFrame, value);
        }
        public override string StartStopResetButtonText {
            get => startStopResetButtonText;
            set => Set(ref startStopResetButtonText, value);
        }

        public MainViewModel() {
            InitializeCommands();
            InitializeViews();
            Initialize();
        }

        private void InitializeViews() {
            viewportSettingsVisible = true;
            dataProviderSettingsVisible = true;
            fullFrameViewportVisible = true;
        }

        private void Initialize() {
            FpsCollection = new DoubleCollection { 1, 2, 3, 5, 10, 15, 20, 30, 50, 60 };
            FpsCurrent = 30.0;
            modelStarted = false;
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
using System;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ConvergenceEngine.Models {

    using IO.Readers;
    using IO.DataExtractors;
    using Mapping;
    using Mapping.Extensions;

    public sealed class Model {

        public event Action OnModelUpdated;

        private DataProvider dataProvider;
        private ColoredFrameExtractor coloredExtractor;

        public Mapper Mapper { get; private set; }

        public string CurrentStateInfo { get; private set; } = "Ready";
        public bool Ready { get; private set; } = true;
        public int FramesCount { get { return dataProvider.TotalFrames; } }

        public Model(Action onModelUpdated) {
            OnModelUpdated += onModelUpdated;
            Initialize();
        }

        private void Initialize() {
            dataProvider = new KinectFileReader();
            if (Mapper.IsNull()) { Mapper = new Mapper(dataProvider); }
            else { Mapper.ReInitializeData(dataProvider); }
            coloredExtractor = new ColoredFrameExtractor(dataProvider);
        }

        public bool Start(string fileName) {
            Initialize();
            bool result = (bool)(dataProvider as FileReader)?.Start(fileName);
            OnModelUpdated?.Invoke();
            return result;
        }

        public void MoveToPosition(int frameIndex) {
            (dataProvider as FileReader)?.MoveToPosition(frameIndex);
            OnModelUpdated?.Invoke();
        }

        public void Stop() {
            dataProvider?.Stop();
            Initialize();
            ChangeState("Ready");
        }

        private void ChangeState(string newModelStateInfo, bool lockModel = false) {
            CurrentStateInfo = newModelStateInfo;
            Ready = !lockModel;
            OnModelUpdated?.Invoke();
        }

        public Task CalculateFramesCountAsync() {
            Task calculateFramesCountTask = new Task(() => {
                ChangeState("Calculate Frames Count", true);
                (dataProvider as FileReader)?.CalculateFramesCount();
                ChangeState("Ready");
            });
            calculateFramesCountTask.Start();
            return calculateFramesCountTask;
        }

        public byte[] GetActualColoredDepthFrame(Color nearColor, Color farColor) {
            return coloredExtractor.ExtractColored(nearColor, farColor);
        }
    }
}
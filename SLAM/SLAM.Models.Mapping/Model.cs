using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace SLAM.Models.Mapping {

    using IO.Readers;
    using IO.DataExtractors;

    public sealed class Model {

        private Action onModelUpdated;

        private DataProvider dataProvider;
        private KinectColoredFrameExtractor coloredFrameExtractor;
        private Mapper mapper;

        public string CurrentStateInfo { get; private set; } = "Ready";
        public bool Ready { get; private set; } = true;
        public int FramesCount { get { return dataProvider.TotalFrames; } }

        public Model(Action onModelUpdated) {
            this.onModelUpdated = onModelUpdated;
            MapperSwitchOffline();
        }

        public void MapperSwitchOnline() {
            dataProvider = new KinectDeviceReader();
            Initialize();
        }

        public void MapperSwitchOffline() {
            dataProvider = new KinectFileReader();
            Initialize();
        }

        private void Initialize() {
            mapper = new Mapper(dataProvider);
            coloredFrameExtractor = new KinectColoredFrameExtractor(dataProvider);
        }

        public bool Start(string fileName) {
            MapperSwitchOffline();
            return (bool)(dataProvider as FileReader)?.Start(fileName);
        }

        public void MoveToPosition(int frameIndex) {
            (dataProvider as FileReader)?.MoveToPosition(frameIndex);
        }

        public void Stop() {
            dataProvider?.Stop();
        }

        private void ChangeState(string newModelStateInfo, bool lockModel = false) {
            CurrentStateInfo = newModelStateInfo;
            Ready = !lockModel;
            onModelUpdated?.Invoke();
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
            return coloredFrameExtractor.ExtractActualColoredDepthFrame(nearColor, farColor);
        }

        public Point[] GetActualPointsFrame() {
            //throw new NotImplementedException();
            return null;
        }

        public IEnumerable<IEnumerable<Point>> GetActualLinearFrame() {
            //throw new NotImplementedException();
            return null;
        }

        public IEnumerable<IEnumerable<Point>> GetPreviousGhostLinearFrame() {
            //throw new NotImplementedException();
            return null;
        }

        public Task<Point[]> GetActualMapFrameAsync() {
            Task<Point[]> getActualMapFrame = new Task<Point[]>(() => {
                ChangeState("Calculate Map", true);
                Point[] result = mapper.GetActualMapFrame();
                ChangeState("Ready");
                return result;
            });
            getActualMapFrame.Start();
            return getActualMapFrame;
        }
    }
}
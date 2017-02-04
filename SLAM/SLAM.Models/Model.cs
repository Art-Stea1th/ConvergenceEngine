using System;
using System.Threading.Tasks;
using System.Windows;


namespace SLAM.Models {

    using Events;
    using IO.Readers;
    using IO.Writers;
    using Mapping;
    using System.Collections.Generic;

    public class Model : IDisposable {

        public event ModelUpdatedEvent OnModelUpdated;

        private DataProvider reader;
        private BaseWriter   writer;
        private BaseMapper   mapper;

        private FramesProvider framesProvider;

        public string CurrentState { get; private set; } = "Ready";
        public bool Ready { get; private set; } = true;
        public int FramesCount { get { return reader.TotalFrames; } }

        public Model(ModelUpdatedEvent onModelUpdated) {
            OnModelUpdated += onModelUpdated;
            MapperSwitchOffline();
        }

        public void MapperSwitchOnline() {
            reader = new KinectDeviceReader();
            Initialize();
        }

        public void MapperSwitchOffline() {
            reader = new KinectFileReader();
            Initialize();
        }

        private void Initialize() {
            mapper = new ComplexMapper(reader);
            framesProvider = new FramesProvider(mapper);
        }

        private void ChangeState(string newModelState, bool lockModel = false) {
            CurrentState = newModelState;
            Ready = !lockModel;
            OnModelUpdated?.Invoke();
        }

        public Task CalculateFramesCountAsync() {
            Task calculateFramesCountTask = new Task(() => {
                ChangeState("Calculate Frames Count", true);
                (reader as FileReader)?.CalculateFramesCount();
                ChangeState("Ready");
            });
            calculateFramesCountTask.Start();
            return calculateFramesCountTask;
        }

        public bool Start(string fileName) {
            MapperSwitchOffline();
            return (bool)(reader as FileReader)?.Start(fileName);
        }

        public void MoveToPosition(int frameIndex) {
            (reader as FileReader)?.MoveToPosition(frameIndex);
        }

        public void Stop() {
            reader?.Stop();
        }

        public byte[] GetActualRawFrame() {
            return framesProvider.GetActualRawFrame();
        }

        public Point[] GetActualPointsFrame() {
            return framesProvider.GetActualPointsFrame();
        }

        public IEnumerable<IEnumerable<Point>> GetActualLinearFrame() {
            return framesProvider.GetActualLinearFrame();
        }

        public Point[] GetActualMapFrame() {
            return framesProvider.GetActualMapFrame();
        }

        public Task<Point[]> GetActualMapFrameAsync() {
            Task<Point[]> getActualMapFrame = new Task<Point[]>(() => {
                ChangeState("Calculate Map", true);
                Point[] result = framesProvider.GetActualMapFrame();
                ChangeState("Ready");
                return result;                
            });
            getActualMapFrame.Start();
            return getActualMapFrame;
        }

        public void Dispose() {
            reader?.Dispose();
        }
    }
}
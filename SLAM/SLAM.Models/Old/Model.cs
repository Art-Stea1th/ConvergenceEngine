using System;
using System.Threading;
using System.Threading.Tasks;


namespace SLAM.Models.Old {

    using DataProcessors;

    public delegate void ModelUpdatedEvent();

    public sealed class Model : IDisposable {

        private DataReader reader;

        private IDataProcessor fullMapFrameProcessor;
        private IDataProcessor topDepthFrameProcessor;
        private IDataProcessor frontDepthFrameProcessor;

        private byte[] fullMapFrameBuffer;
        private byte[] topDepthFrameBuffer;
        private byte[] frontDepthFrameBuffer;        

        public event ModelUpdatedEvent OnModelUpdated;

        public string CurrentState { get; private set; }
        public bool Ready { get; private set; }
        public string FullFileName { get { return reader.FullFileName; } }
        public int FramesCount { get { return reader.FramesCount; } }
        public int CurrentFrame { get { return reader.CurrentFrame; } }

        public Model(ModelUpdatedEvent onModelUpdated) {
            OnModelUpdated += onModelUpdated;
            reader = new DataReader();
            CurrentState = "Ready";
            Ready = true;
        }

        private void ChangeState(string newModelState, bool lockModel = false) {
            CurrentState = newModelState;
            Ready = !lockModel;
            if(OnModelUpdated != null)
                OnModelUpdated.Invoke();
        }

        public bool OpenFile(string fullFileName) {

            bool opened = reader.OpenFile(fullFileName);

            if (opened) {

                fullMapFrameBuffer = new byte[reader.FrameInfo.Length * sizeof(int)];
                topDepthFrameBuffer = new byte[reader.FrameInfo.Length * sizeof(int)];
                frontDepthFrameBuffer = new byte[reader.FrameInfo.Length * sizeof(int)];

                fullMapFrameProcessor = new FullMapFrameProcessor(reader.FrameInfo);
                topDepthFrameProcessor = new TopDepthFrameProcessor(reader.FrameInfo);
                frontDepthFrameProcessor = new FrontDepthFrameProcessor(reader.FrameInfo);
            }
            return opened;
        }

        public Task CalculateFramesCount() {
            Task calculateFramesCountTask = new Task(() => {
                ChangeState("Calculate Frames Count", true);
                reader.CalculateFramesCount();
                Thread.Sleep(333);
                ChangeState("Ready");
            });
            calculateFramesCountTask.Start();
            return calculateFramesCountTask;
        }

        public byte[] GetViewportFullMapFrame(int frameIndex) {
            return GetViewportTopDepthFrame(frameIndex); // tmp
        }

        public byte[] GetViewportTopDepthFrame(int frameIndex) {

            byte[] rawData = reader.ReadFrameBytes(frameIndex);

            if (rawData != null) {
                topDepthFrameBuffer = new byte[reader.FrameInfo.Length * sizeof(int)];
                topDepthFrameProcessor.CalculateViewportFrame(rawData, topDepthFrameBuffer);
                return topDepthFrameBuffer;
            }
            return null;
        }

        public byte[] GetViewportFrontDepthFrame(int frameIndex) {

            byte[] rawData = reader.ReadFrameBytes(frameIndex);            

            if (rawData != null) {
                frontDepthFrameProcessor.CalculateViewportFrame(rawData, frontDepthFrameBuffer);
                return frontDepthFrameBuffer;
            }
            return null;
        }        

        public void CloseFile() {
            reader.CloseFile();
        }

        public void Dispose() {
            reader.Dispose();
        }
    }
}
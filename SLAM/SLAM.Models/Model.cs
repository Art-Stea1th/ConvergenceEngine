using System;
using System.Threading;
using System.Threading.Tasks;


namespace SLAM.Models {

    public delegate void ModelUpdatedEvent();

    public sealed class Model : IDisposable {

        private DataReader reader;
        private DataConverter converter;
        private Mapper mapper;

        private byte[] fullFrameBuffer;
        private byte[] curveFrameBuffer;

        public event ModelUpdatedEvent OnModelUpdated;

        public string CurrentState { get; private set; } = "Ready";
        public bool Ready { get; private set; } = true;
        public string FullFileName { get { return reader.FullFileName; } }
        public int FramesCount { get { return reader.FramesCount; } }
        public int CurrentFrame { get { return reader.CurrentFrame; } }

        public Model(ModelUpdatedEvent onModelUpdated) {
            OnModelUpdated += onModelUpdated;
            reader = new DataReader();
            mapper = new Mapper();
        }

        private void ChangeState(string newModelState, bool lockModel = false) {
            CurrentState = newModelState;
            Ready = !lockModel;
            OnModelUpdated?.Invoke();
        }

        public bool OpenFile(string fullFileName) {
            bool openResult  = reader.OpenFile(fullFileName);
            fullFrameBuffer  = new byte[reader.FrameInfo.Length * sizeof(int)];
            curveFrameBuffer = new byte[reader.FrameInfo.Length * sizeof(int)];
            converter = new DataConverter(reader.FrameInfo);
            return openResult;
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

        public byte[] GetViewportFullFrame(int frameIndex) {

            byte[] rawData = reader.ReadFrameBytes(frameIndex);            

            if (rawData != null) {
                converter.ConvertRawToViewportFullFrame(rawData, fullFrameBuffer);
                return fullFrameBuffer;
            }
            return null;
        }

        public byte[] GetViewportCurveFrame(int frameIndex) {

            byte[] rawData = reader.ReadFrameBytes(frameIndex);

            if (rawData != null) {
                curveFrameBuffer = new byte[reader.FrameInfo.Length * sizeof(int)];
                converter.ConvertRawToViewportCurveFrame(rawData, curveFrameBuffer);
                return curveFrameBuffer;
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
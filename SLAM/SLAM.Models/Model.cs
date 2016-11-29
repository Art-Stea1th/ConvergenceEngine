using System;
using System.Threading;
using System.Threading.Tasks;


namespace SLAM.Models {

    using Converters;

    public delegate void ModelUpdatedEvent();

    public sealed class Model : IDisposable {

        private DataReader reader;
        private IDataConverter converter;
        private Mapper mapper;

        private byte[] fullFrameBuffer;
        private byte[] curveFrameBuffer;

        public event ModelUpdatedEvent OnModelUpdated;

        public string CurrentState { get; private set; }
        public bool Ready { get; private set; }
        public string FullFileName { get { return reader.FullFileName; } }
        public int FramesCount { get { return reader.FramesCount; } }
        public int CurrentFrame { get { return reader.CurrentFrame; } }

        public Model(ModelUpdatedEvent onModelUpdated) {
            OnModelUpdated += onModelUpdated;
            reader = new DataReader();
            mapper = new Mapper();
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
            bool openResult  = reader.OpenFile(fullFileName);
            fullFrameBuffer  = new byte[reader.FrameInfo.Length * sizeof(int)];
            curveFrameBuffer = new byte[reader.FrameInfo.Length * sizeof(int)];
            converter = new RapidConverter(reader.FrameInfo);
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
                converter.RawFrameToFullFrame(rawData, fullFrameBuffer);
                return fullFrameBuffer;
            }
            return null;
        }

        public byte[] GetViewportCurveFrame(int frameIndex) {

            byte[] rawData = reader.ReadFrameBytes(frameIndex);

            if (rawData != null) {
                curveFrameBuffer = new byte[reader.FrameInfo.Length * sizeof(int)];
                converter.RawFrameToCurveFrame(rawData, curveFrameBuffer);
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
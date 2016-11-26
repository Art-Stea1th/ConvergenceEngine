using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SLAM.Models {    

    internal class DataReader : IDisposable {

        private FileStream stream;
        private BinaryReader reader;

        internal DepthFrameSequenceInfo FrameInfo { get; private set; }

        private int currentFrameIndex;
        private long CurrentOffset {
            get { return FrameInfo.FirstFramePosition + (FrameInfo.BytesPerFrame * currentFrameIndex); }
        }

        internal int FramesCount { get; private set; }
        internal int CurrentFrameIndex {
            get { return currentFrameIndex; }
            set { currentFrameIndex = value < 0 ? 0 : (value >= FramesCount ? FramesCount - 1 : value); }
        }

        

        internal DataReader() {
            Initialize();
        }

        private void Initialize() {
            stream = null;
            reader = null;
            FrameInfo = null;
        }

        public bool OpenFile(string fullFileName) {
            CloseFile();
            stream = new FileStream(fullFileName, FileMode.Open);
            reader = new BinaryReader(stream);
            FrameInfo = new DepthFrameSequenceInfo(reader);
            if (FrameInfo.IsCorrect) {
                return true;
            }
            CloseFile();
            return false;
        }

        public Task<int> CalculateFramesCount(ModelUnavailableEvent onModelOccupied, ModelReadyEvent onModelReady) {

            Task<int> calculateFramesCountTask = new Task<int>(() => {

                onModelOccupied?.Invoke("Calculate Frames Count");

                long savedPosition = reader.BaseStream.Position;
                int framesCount = 0;

                byte[] buffer = new byte[FrameInfo.BytesPerFrame];
                reader.BaseStream.Position = FrameInfo.FirstFramePosition;

                while (reader.Read(buffer, 0, FrameInfo.BytesPerFrame) == FrameInfo.BytesPerFrame) {
                    ++framesCount;
                }
                reader.BaseStream.Position = savedPosition;

                onModelReady?.Invoke();
                return framesCount;
            });
            calculateFramesCountTask.Start();

            return calculateFramesCountTask;            
        }

        public byte[] ReadFrame(int frameIndex) {
            if (stream == null || reader == null) {
                throw new NullReferenceException("you must first open the file");
            }
            throw new NotImplementedException();
        }

        public void CloseFile() {
            Dispose();
            Initialize();
        }

        public void Dispose() {
            reader?.Dispose();
            stream?.Dispose();
        }
    }
}
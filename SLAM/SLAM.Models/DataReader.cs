using System;
using System.IO;

namespace SLAM.Models {

    internal class DataReader : IDisposable {

        private FileStream stream;
        private BinaryReader reader;

        private int currentFrameIndex;

        internal DepthFrameSequenceInfo FrameInfo { get; private set; }

        internal string FullFileName { get; private set; }
        internal int FramesCount { get; private set; }
        internal int CurrentFrame {
            get { return currentFrameIndex; }
            private set { currentFrameIndex = value >= 0 && value < FramesCount ? value : 0; } }

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
                FullFileName = fullFileName;
                return true;
            }
            CloseFile();
            return false;
        }

        public void CalculateFramesCount() {

            long savedPosition = reader.BaseStream.Position;
            int framesCount = 0;            

            byte[] buffer = new byte[FrameInfo.BytesPerFrame];
            reader.BaseStream.Position = 0;

            while (reader.Read(buffer, 0, FrameInfo.BytesPerFrame) == FrameInfo.BytesPerFrame) {
                ++framesCount;
            }
            reader.BaseStream.Position = savedPosition;
            FramesCount = framesCount - 1;
            CurrentFrame = 0;
        }

        public byte[] ReadFrame(int frameIndex) {

            if (stream == null || reader == null || FrameInfo == null) { return null; }
            if (FramesCount < 1) { return null; }
            if (frameIndex < 0 || frameIndex >= FramesCount) { return null; }

            long savedPosition = reader.BaseStream.Position;

            byte[] result = new byte[FrameInfo.BytesPerFrame];
            reader.BaseStream.Position = CalculateOffset(frameIndex);

            if (reader.Read(result, 0, FrameInfo.BytesPerFrame) == FrameInfo.BytesPerFrame) {
                CurrentFrame = frameIndex;
                return result;
            }
            reader.BaseStream.Position = savedPosition;
            return null;
        }

        private long CalculateOffset(int frameIndex) {
            return FrameInfo.FirstFramePosition + ((long)FrameInfo.BytesPerFrame * frameIndex);
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
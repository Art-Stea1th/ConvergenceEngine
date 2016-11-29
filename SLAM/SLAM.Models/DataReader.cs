using System;
using System.IO;

namespace SLAM.Models {

    internal class DataReader : IDisposable {

        private FileStream stream;
        private BinaryReader reader;

        private int currentFrameIndex;
        private byte[] currentFrameBuffer;

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
                currentFrameBuffer = new byte[FrameInfo.BytesPerFrame];
                return true;
            }
            CloseFile();
            return false;
        }

        public void CalculateFramesCount() {

            long savedPosition = reader.BaseStream.Position;
            int framesCount = 0;            

            reader.BaseStream.Position = 0;

            while (reader.Read(currentFrameBuffer, 0, FrameInfo.BytesPerFrame) == FrameInfo.BytesPerFrame) {
                ++framesCount;
            }
            reader.BaseStream.Position = savedPosition;
            FramesCount = framesCount - 1;
            CurrentFrame = 0;
        }

        // performance critical
        public byte[] ReadFrameBytes(int frameIndex) {

            long savedPosition = reader.BaseStream.Position;

            reader.BaseStream.Position = CalculateOffset(frameIndex);

            if (reader.Read(currentFrameBuffer, 0, FrameInfo.BytesPerFrame) == FrameInfo.BytesPerFrame) {
                CurrentFrame = frameIndex;
                return currentFrameBuffer;
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
            if(reader !=null)
                reader.Dispose();
            if(stream !=null)
                stream.Dispose();
        }
    }
}
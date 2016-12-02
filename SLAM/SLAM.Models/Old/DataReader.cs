using System;
using System.IO;
using System.Runtime.CompilerServices;


namespace SLAM.Models.Old {

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
            private set { currentFrameIndex = value >= 0 && value < FramesCount ? value : 0; }
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

            if (CurrentFrame != 0 && CurrentFrame == frameIndex) {
                return currentFrameBuffer;
            }

            long savedPosition = reader.BaseStream.Position;

            reader.BaseStream.Position = CalculateOffset(frameIndex);

            if (reader.Read(currentFrameBuffer, 0, FrameInfo.BytesPerFrame) == FrameInfo.BytesPerFrame) {
                CurrentFrame = frameIndex;
                HorizontalMirror(currentFrameBuffer);
                return currentFrameBuffer;
            }
            reader.BaseStream.Position = savedPosition;
            return null;
        }

        // Horizontal Mirror RAW Frame and swap [hi <--> low] bytes for each Int16\short depth
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void HorizontalMirror(byte[] frame) {

            int width  = FrameInfo.Width * sizeof(short);
            int height = FrameInfo.Height;

            for (int y = 0; y < height; ++y) {
                for (int x = 0; x < width / 2; ++x) {

                    int linearIndexNormal = GetLinearIndex(x, y, width);
                    int linearIndexInverse = GetLinearIndex((width - 1 - x), y, width);

                    byte tmp = frame[linearIndexNormal];
                    frame[linearIndexNormal] = frame[linearIndexInverse];
                    frame[linearIndexInverse] = tmp;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected int GetLinearIndex(int x, int y, int width) {
            return width * y + x;
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
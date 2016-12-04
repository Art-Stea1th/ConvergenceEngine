using System;
using System.Runtime.CompilerServices;

namespace SLAM.Models.Data.Readers {

    using Adapters;

    internal sealed class KinectFileReader : FileReader, IDisposable {

        public KinectFileReader(string fileName = null) : base(fileName) {
            adapter = new KinectDataAdapter(this);
        }

        internal override void CalculateFramesCount() {

            long savedPosition = reader.BaseStream.Position;
            int framesCount = -1;

            reader.BaseStream.Position = 0;
            while (reader.Read(rawFrameBuffer, 0, FrameInfo.BytesPerFrame) == FrameInfo.BytesPerFrame) {
                ++framesCount;
            }
            reader.BaseStream.Position = savedPosition;

            TotalFrames = framesCount;
            FrameIndex = 0;
        }

        internal override void MoveToPosition(int frameIndex) {

            if (FrameIndex != 0 && FrameIndex == frameIndex) { return; }

            long savedPosition = reader.BaseStream.Position;

            reader.BaseStream.Position = StreamPositionBy(frameIndex);

            if (reader.Read(rawFrameBuffer, 0, FrameInfo.BytesPerFrame) == FrameInfo.BytesPerFrame) {
                FrameIndex = frameIndex;
                HorizontalMirror(rawFrameBuffer);
                NotifyFrameReady();
                return;
            }
            reader.BaseStream.Position = savedPosition;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private long StreamPositionBy(int frameIndex) {
            return FrameInfo.FirstFramePosition + ((long)FrameInfo.BytesPerFrame * frameIndex);
        }

        // Horizontal Mirror RAW Frame and swap [hi <--> low] bytes for each Int16\short depth
        private void HorizontalMirror(byte[] frame) {

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
        private int GetLinearIndex(int x, int y, int width) {
            return width * y + x;
        }        
    }
}
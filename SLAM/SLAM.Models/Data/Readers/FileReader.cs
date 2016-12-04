using System;
using System.IO;

namespace SLAM.Models.Data.Readers {

    using Adapters;

    internal abstract class FileReader : DataProvider, IDisposable {

        protected FileStream   stream;
        protected BinaryReader reader;

        internal abstract void CalculateFramesCount();
        internal abstract void MoveToPosition(int frameIndex);

        internal FileReader(string fileName = null) {
            Start(fileName);
        }                

        internal override bool Start(string param) {
            Stop();
            if (param != null) {
                stream = new FileStream(param, FileMode.Open);
                reader = new BinaryReader(stream);
                FrameInfo = new FrameSequenceInfo(reader);
                if (FrameInfo.IsCorrect) {
                    rawFrameBuffer = new byte[FrameInfo.BytesPerFrame];
                    return true;
                }
            }
            Stop();
            return false;
        }

        internal override void Stop() {
            Dispose();
            Initialize();
        }

        public override void Dispose() {
            reader?.Dispose();
            stream?.Dispose();
        }

        private void Initialize() {
            stream = null;
            reader = null;
        }        
    }
}
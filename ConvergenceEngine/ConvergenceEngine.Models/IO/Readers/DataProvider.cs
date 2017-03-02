using System;

namespace ConvergenceEngine.Models.IO.Readers {

    internal abstract class DataProvider : IDisposable {

        public event Action OnNextFrameReady;        

        public FrameSequenceInfo FrameInfo { get; protected set; }
        public int TotalFrames { get; protected set; }
        public int FrameIndex  { get; protected set; }

        protected byte[] rawFrameBuffer;

        internal abstract bool Start(string param = null);
        internal abstract void Stop();
        public abstract void Dispose();

        public bool GetNextRawFrameTo(out byte[] destination) {
            destination = rawFrameBuffer;
            return destination != null;
        }

        protected void NotifyFrameReady() {
            OnNextFrameReady?.Invoke();
        }
    }
}
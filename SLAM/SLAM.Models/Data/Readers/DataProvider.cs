using System;
using System.Windows;

namespace SLAM.Models.Data.Readers {

    using Adapters;
    using Events;

    internal abstract class DataProvider : IDisposable {

        public event NextFrameReadyEvent OnNextFrameReady;        

        public FrameSequenceInfo FrameInfo { get; protected set; }
        public int TotalFrames { get; protected set; }
        public int FrameIndex  { get; protected set; }
        
        protected byte[] rawFrameBuffer;

        protected IDataAdapter adapter;

        internal abstract bool Start(string param = null);
        internal abstract void Stop();
        public abstract void Dispose();

        public virtual bool GetNextFrameTo(out Point[] destination) {
            Point[] adaptedPointsFrameBuffer = new Point[FrameInfo.Width];
            adapter?.Adapt(rawFrameBuffer, adaptedPointsFrameBuffer);
            destination = adaptedPointsFrameBuffer;
            return destination != null;
        }

        public bool GetNextRawFrameTo(out byte[] destination) {
            destination = rawFrameBuffer;
            return destination != null;
        }

        protected void NotifyFrameReady() {
            OnNextFrameReady?.Invoke();
        }
    }
}
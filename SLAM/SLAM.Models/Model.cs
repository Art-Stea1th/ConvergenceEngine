using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SLAM.Models {

    public delegate void ModelUnavailableEvent(string processName);
    public delegate void ModelReadyEvent();

    public sealed class Model : IDisposable {        

        private int currentFrameIndex;

        private DataReader reader;
        private Mapper mapper;

        public event ModelUnavailableEvent OnModelOccupied;
        public event ModelReadyEvent OnModelReady;

        public string FullFileName { get; private set; }

        public int FramesCount { get { return reader.FramesCount; } }
        public int CurrentFrameIndex {
            get { return currentFrameIndex; }
            set { }
        }

        public Model(ModelUnavailableEvent onModelOccupied, ModelReadyEvent onModelReady) {
            OnModelOccupied += onModelOccupied;
            OnModelReady += onModelReady;
            reader = new DataReader();
            mapper = new Mapper();            
            OnModelReady?.Invoke();
        }

        public bool OpenFile(string fullFileName) {
            return reader.OpenFile(fullFileName);
        }

        public Task<int> CalculateFramesCount() {
            return reader.CalculateFramesCount(OnModelOccupied, OnModelReady);
        }

        public void Dispose() {
            reader.Dispose();
        }

        
    }
}
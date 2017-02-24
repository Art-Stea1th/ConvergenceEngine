using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace ConvergenceEngine.Models.Mapping {

    using IO.DataExtractors;
    using IO.Readers;
    using Extensions;
    using Navigation;

    public sealed class Map : FrameSequence {

        private DataProvider dataProvider;
        private byte[] currentFrameBuffer;

        private MiddleLineFrameExtractor middleLineExtractor;

        public event Action OnFrameUpdate;

        // --> TMP
        public IEnumerable<Point> MapPoints { get { return GetMapPoints(); } }
        public IEnumerable<Point> SourceFramePoints { get { return ActualFrame?.Points; } }
        public IEnumerable<Tuple<Point, Point>> CurrentFrameSegments { get { return ActualFrame?.Select(s => new Tuple<Point, Point>(s.PointA, s.PointB)); } }
        public IEnumerable<Tuple<Point, Point>> PreviousFrameSegments { get { return PreviousFrame?.Select(s => new Tuple<Point, Point>(s.PointA, s.PointB)); } }
        public IEnumerable<Tuple<Point, Point>> TrackedFrameSegments {
            get {
                if (ActualFrame.IsNull() || PreviousFrame.IsNull()) { return null; }
                return ActualFrame.SelectTrackedTo(PreviousFrame).Select(s => new Tuple<Point, Point>(s.Item2.PointA, s.Item2.PointB));
            }
        }
        // <-- TMP

        internal Map(DataProvider dataProvider) {
            this.dataProvider = dataProvider;
            Initialize();
        }

        private void Initialize() {            
            dataProvider.OnNextFrameReady += Update;
        }

        private void Update() {
            if (middleLineExtractor == null) { // TMP
                middleLineExtractor = new MiddleLineFrameExtractor(dataProvider.FrameInfo);
            }

            dataProvider.GetNextRawFrameTo(out currentFrameBuffer);
            var points = middleLineExtractor.ExtractMiddleLine(currentFrameBuffer);

            NextFrameProceed(dataProvider.FrameIndex, new Frame(new List<Point>(points)));
            OnFrameUpdate?.Invoke();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace SLAM.Models.Mapping {

    using IO.DataExtractors;
    using IO.Readers;
    using Navigation;

    public sealed class Map : FrameSequence {

        private DataProvider dataProvider;
        private byte[] currentFrameBuffer;

        private MiddleLineFrameExtractor middleLineExtractor;

        public event Action OnFrameUpdate;

        // --> TMP
        public IEnumerable<Point> MapPoints { get { return GetMapPoints(); } }
        public IEnumerable<Point> FramePoints { get { return ActualFrame?.Points; } }
        public IEnumerable<Tuple<Point, Point>> FrameSegments { get { return ActualFrame?.Select(s => new Tuple<Point, Point>(s.PointA, s.PointB)); } }
        public IEnumerable<Tuple<Point, Point>> PreviousFrameSegments { get; set; }
        public IEnumerable<Tuple<Point, Point>> SimilarFrameSegments { get; set; }
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
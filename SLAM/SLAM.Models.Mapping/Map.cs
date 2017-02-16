using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace SLAM.Models.Mapping {

    using IO.DataExtractors;
    using IO.Readers;

    public sealed class Map {

        private DataProvider dataProvider;
        private byte[] currentFrameBuffer;

        private ColoredFrameExtractor coloredExtractor;

        private FrameSequence frameSequence;
        private Frame currentFrame, previousFrame;

        public event Action OnFrameUpdate;

        public byte[] Buffer { get { return coloredExtractor.ExtractColored(Color.FromArgb(255, 0, 128, 192), Color.FromArgb(255, 0, 0, 30)); } }
        //public IEnumerable<Point> MapPoints { get { return frameSequence?.SelectMany(f => f.Value.Points); } }
        public IEnumerable<Point> MapPoints { get { return frameSequence?.GetMapPoints(); } }
        public IEnumerable<Point> FramePoints { get { return currentFrame?.Points; } }
        public IEnumerable<Tuple<Point, Point>> FrameSegments { get { return currentFrame?.SegmentsAsEnumerableOfTuple; } }
        public IEnumerable<Tuple<Point, Point>> PreviousFrameSegments { get { return previousFrame?.SegmentsAsEnumerableOfTuple; } }


        public IEnumerable<Tuple<Point, Point>> SimilarFrameSegments {
            get {
                return currentFrame != null && previousFrame != null ?
                    currentFrame.Segments.FindSimilarSegmentsTo(previousFrame.Segments)
                    .Select(sq => new Tuple<Point, Point>(sq.Item2.PointA, sq.Item2.PointB)) : null;
            }
        }

        internal Map(DataProvider dataProvider) {
            this.dataProvider = dataProvider;
            Initialize();
        }

        private void Initialize() {
            //middleLineExtractor = new MiddleLineFrameExtractor(dataProvider.FrameInfo);
            coloredExtractor = new ColoredFrameExtractor(dataProvider);
            dataProvider.OnNextFrameReady += Update;
        }

        private void Update() {

            dataProvider.GetNextRawFrameTo(out currentFrameBuffer);

            if (frameSequence == null) {
                frameSequence = new FrameSequence(dataProvider.TotalFrames, new MiddleLineFrameExtractor(dataProvider.FrameInfo));
            }

            previousFrame = currentFrame;

            currentFrame = frameSequence.AppendData(dataProvider.FrameIndex, currentFrameBuffer);
            OnFrameUpdate?.Invoke();
        }
    }
}
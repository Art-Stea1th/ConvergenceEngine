using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace ConvergenceEngine.ViewModels.AppWindows {

    using Models.Mapping;

    public sealed class MixedDataWindowViewModel : ViewModelBase {

        private Map map;

        private IEnumerable<Point> sourcePoints;
        private IEnumerable<Tuple<Point, Point>> currentSegments;
        private IEnumerable<Tuple<Point, Point>> previousSegments;
        private IEnumerable<Tuple<Point, Point>> trackedSegments;

        public IEnumerable<Point> SourcePoints {
            get { return sourcePoints; }
            set { Set(ref sourcePoints, value); }
        }

        public IEnumerable<Tuple<Point, Point>> CurrentSegments {
            get { return currentSegments; }
            set { Set(ref currentSegments, value); }
        }

        public IEnumerable<Tuple<Point, Point>> PreviousSegments {
            get { return previousSegments; }
            set { Set(ref previousSegments, value); }
        }

        public IEnumerable<Tuple<Point, Point>> TrackedSegments {
            get { return trackedSegments; }
            set { Set(ref trackedSegments, value); }
        }

        internal MixedDataWindowViewModel(Map map) {
            this.map = map;
            this.map.OnFrameUpdate += Update;
            Initialize();
        }

        public void Initialize() {
            SourcePoints = null;
            CurrentSegments = null;
            PreviousSegments = null;
            TrackedSegments = null;
        }

        public void Update() {
            SourcePoints = map.CurrentFrame?.Points;
            CurrentSegments = map.CurrentFrame?.Select(s => (Tuple<Point, Point>)s);
            PreviousSegments = map.PreviousFrame?.Select(s => (Tuple<Point, Point>)s);
            TrackedSegments = map.CurrentFrame?.SelectTrackedTo(map.PreviousFrame)?
                .Select(s => new Tuple<Point, Point>(s.Item2.PointA, s.Item2.PointB));
        }
    }
}
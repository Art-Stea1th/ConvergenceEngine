using System;
using System.Collections.Generic;
using System.Windows;

namespace ConvergenceEngine.ViewModels.AppWindows {

    using Models.Mapping;

    public sealed class MixedDataWindowViewModel : ViewModelBase {

        private Model model;

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

        internal MixedDataWindowViewModel(Model model) {
            this.model = model;
            model.OnModelUpdated += Update;
            Initialize();
        }

        public void Initialize() {
            SourcePoints = null;
            CurrentSegments = null;
            PreviousSegments = null;
            TrackedSegments = null;
        }

        public void Update() {
            SourcePoints = model.Map.SourceFramePoints;
            CurrentSegments = model.Map.CurrentFrameSegments;
            PreviousSegments = model.Map.PreviousFrameSegments;
            TrackedSegments = model.Map.TrackedFrameSegments;
        }
    }
}
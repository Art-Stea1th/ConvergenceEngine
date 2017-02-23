using System;
using System.Collections.Generic;
using System.Windows;

namespace ConvergenceEngine.ViewModels.AppWindows {

    using Models.Mapping;

    public sealed class LinearDataWindowViewModel : ViewModelBase {

        private Model model;
        IEnumerable<Tuple<Point, Point>> segments;
        IEnumerable<Tuple<Point, Point>> previousSegments;
        IEnumerable<Tuple<Point, Point>> trackedSegments;

        public IEnumerable<Tuple<Point, Point>> Segments {
            get { return segments; }
            set { Set(ref segments, value); }
        }

        public IEnumerable<Tuple<Point, Point>> PreviousSegments {
            get { return previousSegments; }
            set { Set(ref previousSegments, value); }
        }

        public IEnumerable<Tuple<Point, Point>> TrackedSegments {
            get { return trackedSegments; }
            set { Set(ref trackedSegments, value); }
        }

        internal LinearDataWindowViewModel(Model model) {
            this.model = model;
            model.OnModelUpdated += Update;
            Initialize();
        }

        public void Initialize() {
            Segments = null;
            PreviousSegments = null;
            TrackedSegments = null;
        }

        public void Update() {
            Segments = model.Map.FrameSegments;
            PreviousSegments = model.Map.PreviousFrameSegments;
            TrackedSegments = model.Map.TrackedFrameSegments;
        }
    }
}
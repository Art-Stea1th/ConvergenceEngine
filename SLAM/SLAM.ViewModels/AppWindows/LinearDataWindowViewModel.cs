using System;
using System.Collections.Generic;
using System.Windows;

namespace SLAM.ViewModels.AppWindows {

    using Models.Mapping;

    public sealed class LinearDataWindowViewModel : ViewModelBase {

        private Model model;
        IEnumerable<Tuple<Point, Point>> previousSegments;
        IEnumerable<Tuple<Point, Point>> segments;

        public IEnumerable<Tuple<Point, Point>> Segments {
            get { return segments; }
            set { Set(ref segments, value); }
        }

        public IEnumerable<Tuple<Point, Point>> PreviousSegments {
            get { return previousSegments; }
            set { Set(ref previousSegments, value); }
        }

        internal LinearDataWindowViewModel(Model model) {
            this.model = model;
            model.OnModelUpdated += Update;
            Initialize();
        }

        public void Initialize() {
            Segments = null;
            PreviousSegments = null;
        }

        public void Update() {
            Segments = model.Map.FrameSegments;
            PreviousSegments = model.Map.PreviousFrameSegments;
        }
    }
}
using System;
using SLAM.Models;
using System.Windows;
using System.Collections.Generic;

namespace SLAM.ViewModels.AppWindows {

    using Models.Mapping;

    public class LinearDataWindowViewModel : ViewportWindowViewModel {

        IEnumerable<IEnumerable<Point>> segments;
        IEnumerable<IEnumerable<Point>> previousGhostSegments;

        public IEnumerable<IEnumerable<Point>> Segments {
            get { return segments; }
            set { Set(ref segments, value); }
        }

        public IEnumerable<IEnumerable<Point>> PreviousGhostSegments {
            get { return previousGhostSegments; }
            set { Set(ref previousGhostSegments, value); }
        }

        internal LinearDataWindowViewModel() {
            Title = "Linear Data";
            Initialize();
        }

        public override void Initialize() {
            Segments = null;
            PreviousGhostSegments = null;
        }

        public override void UpdateFrom(Model model) {
            Segments = model.GetActualLinearFrame();
            PreviousGhostSegments = model.GetPreviousGhostLinearFrame();
        }
    }
}
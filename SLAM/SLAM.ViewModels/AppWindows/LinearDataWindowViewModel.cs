using System;
using SLAM.Models;
using System.Windows;
using System.Collections.Generic;

namespace SLAM.ViewModels.AppWindows {

    public class LinearDataWindowViewModel : ViewportWindowViewModel {

        IEnumerable<IEnumerable<Point>> segments;

        public IEnumerable<IEnumerable<Point>> Segments {
            get { return segments; }
            set { Set(ref segments, value); }
        }

        internal LinearDataWindowViewModel() {
            Title = "Linear Data";
            Initialize();
        }

        public override void Initialize() {
            Segments = null;
        }

        public override void UpdateFrom(Model model) {
            Segments = model.GetActualLinearFrame();
        }
    }
}
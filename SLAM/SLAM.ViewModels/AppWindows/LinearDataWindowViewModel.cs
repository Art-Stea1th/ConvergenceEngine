using System.Collections.Generic;
using System.Windows;

namespace SLAM.ViewModels.AppWindows {

    using Models.Mapping;

    public sealed class LinearDataWindowViewModel : ViewModelBase {

        private Model model;
        IEnumerable<IEnumerable<Point>> segments;

        public IEnumerable<IEnumerable<Point>> Segments {
            get { return segments; }
            set { Set(ref segments, value); }
        }

        internal LinearDataWindowViewModel(Model model) {
            this.model = model;
            model.OnModelUpdated += Update;
            Initialize();
        }

        public void Initialize() {
            Segments = null;
        }

        public void Update() {
            Segments = model.Map.FrameSegments;
        }
    }
}
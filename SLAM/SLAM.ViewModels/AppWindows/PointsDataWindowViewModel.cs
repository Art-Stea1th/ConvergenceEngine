using System.Collections.Generic;
using System.Windows;

namespace SLAM.ViewModels.AppWindows {

    using Models.Mapping;

    public sealed class PointsDataWindowViewModel : ViewModelBase {

        private Model model;

        private IEnumerable<Point> pointsData;

        public IEnumerable<Point> PointsData {
            get { return pointsData; }
            set { Set(ref pointsData, value); }
        }

        internal PointsDataWindowViewModel(Model model) {
            this.model = model;
            model.OnModelUpdated += Update;
            Initialize();
        }

        public void Initialize() {
            PointsData = null;
        }

        public void Update() {
            PointsData = model.Map.FramePoints;
        }
    }
}
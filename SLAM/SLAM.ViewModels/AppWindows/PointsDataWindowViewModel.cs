using System.Windows;


namespace SLAM.ViewModels.AppWindows {

    using Models;

    internal class PointsDataWindowViewModel : ViewportWindowViewModel {

        private Point[] pointsData;

        public Point[] PointsData {
            get { return pointsData; }
            set { Set(ref pointsData, value); }
        }

        internal PointsDataWindowViewModel() {
            Title = "Points Data";
            Initialize();
        }

        public override void Initialize() {
            PointsData = null;
        }

        public override void UpdateFrom(Model model) {
            PointsData = model.GetActualTopDepthFrame();
        }
    }
}
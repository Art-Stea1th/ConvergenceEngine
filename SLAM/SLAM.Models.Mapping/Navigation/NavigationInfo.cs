using System.Windows;


namespace SLAM.Models.Mapping.Navigation {

    using Extensions;

    internal struct NavigationInfo {

        public Vector Direction { get; set; }
        public double Angle { get; set; }

        public double NormalizedDirection { get { return Angle.AsNormalizedAngle(); } }

        internal NavigationInfo(Vector direction, double angle) {
            Direction = direction;
            Angle = angle;
        }

        internal NavigationInfo(double offsetX = 0.0, double offsetY = 0.0, double angle = 0.0) {
            Direction = new Vector(offsetX, offsetY);
            Angle = angle;
        }
    }
}
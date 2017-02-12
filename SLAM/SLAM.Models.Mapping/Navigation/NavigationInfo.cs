using System.Windows;


namespace SLAM.Models.Mapping.Navigation {

    using Extensions;

    internal struct NavigationInfo {

        public Vector Location { get; private set; }
        public double Direction { get; private set; }

        public double NormalizedDirection { get { return Direction.AsNormalizedAngle(); } }

        internal NavigationInfo(Vector location, double direction) {
            Location = location;
            Direction = direction;
        }

        internal NavigationInfo(double locationX = 0.0, double locationY = 0.0, double direction = 0.0) {
            Location = new Vector(locationX, locationY);
            Direction = direction;
        }
    }
}
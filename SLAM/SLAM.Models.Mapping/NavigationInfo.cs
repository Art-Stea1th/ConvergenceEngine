using System.Windows;


namespace SLAM.Models.Mapping {

    internal struct NavigationInfo {

        private Vector location;
        private double direction;

        public Vector Location {
            get { return location; }
            set { location = value; }
        }
        public double Direction {
            get { return direction; } 
            set { direction = value; }
        }
        public double NormalizedDirection {
            get {
                return // if it even so (-180 < direction < 180), return; else clamp direction to -180 / 180 and return;
                  ((int)(direction %= 360.0) / 180) % 2 == 0 ? direction : (direction > 0 ? direction - 360.0 : direction + 360.0);
            }
        }
    }
}
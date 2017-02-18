using System.Windows;


namespace SLAM.Models.Mapping.Navigation {

    using Extensions;

    internal struct NavigationInfo {

        public Vector Direction { get; }
        public double Angle { get; }

        public double NormalizedDirection { get { return Angle.AsNormalizedAngle(); } }

        public override string ToString() {
            return $"{Direction}, {Angle}";
        }

        internal NavigationInfo(NavigationInfo navigationInfo) {
            Direction = navigationInfo.Direction;
            Angle = navigationInfo.Angle;
        }

        internal NavigationInfo(Vector direction, double angle) {
            Direction = direction;
            Angle = angle;
        }

        internal NavigationInfo(double offsetX = 0.0, double offsetY = 0.0, double angle = 0.0) {
            Direction = new Vector(offsetX, offsetY);
            Angle = angle;
        }

        public static NavigationInfo operator +(NavigationInfo navInfoA, NavigationInfo navInfoB) {
            decimal resultDirectionX = (decimal)navInfoA.Direction.X + (decimal)navInfoB.Direction.X;
            decimal resultDirectionY = (decimal)navInfoA.Direction.Y + (decimal)navInfoB.Direction.Y;
            decimal resultAngle = (decimal)navInfoA.Angle + (decimal)navInfoB.Angle;
            return new NavigationInfo((double)resultDirectionX, (double)resultDirectionY, (double)resultAngle);
        }

        public static NavigationInfo operator -(NavigationInfo navInfoA, NavigationInfo navInfoB) {
            decimal resultDirectionX = (decimal)navInfoA.Direction.X - (decimal)navInfoB.Direction.X;
            decimal resultDirectionY = (decimal)navInfoA.Direction.Y - (decimal)navInfoB.Direction.Y;
            decimal resultAngle = (decimal)navInfoA.Angle - (decimal)navInfoB.Angle;
            return new NavigationInfo((double)resultDirectionX, (double)resultDirectionY, (double)resultAngle);
        }

    }
}
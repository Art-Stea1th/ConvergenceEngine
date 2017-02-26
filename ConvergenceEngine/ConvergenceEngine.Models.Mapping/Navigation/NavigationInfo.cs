using System.Windows;


namespace ConvergenceEngine.Models.Mapping.Navigation {

    using Extensions;

    public sealed class NavigationInfo {

        public Vector Direction { get; private set; }
        public double Angle { get; private set; }

        public double NormalizedDirection { get { return Angle.AsNormalizedAngle(); } }

        public override string ToString() {
            return $"{Direction.X}, {Direction.Y}, {Angle}";
        }

        internal NavigationInfo(NavigationInfo navigationInfo) {
            Direction = new Vector(navigationInfo.Direction.X, navigationInfo.Direction.Y);
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
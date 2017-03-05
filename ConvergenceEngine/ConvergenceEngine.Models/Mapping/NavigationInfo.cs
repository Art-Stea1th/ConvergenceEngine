using System.Windows;


namespace ConvergenceEngine.Models.Mapping {

    using Extensions;

    public sealed class NavigationInfo {

        public Vector Offset { get; private set; }
        public double Angle { get; private set; }

        public double NormalizedDirection { get { return Angle.AsNormalizedAngle(); } }

        public override string ToString() {
            return $"{Offset.X}, {Offset.Y}, {Angle}";
        }

        internal NavigationInfo(NavigationInfo navigationInfo) {
            Offset = new Vector(navigationInfo.Offset.X, navigationInfo.Offset.Y);
            Angle = navigationInfo.Angle;
        }

        internal NavigationInfo(Vector direction, double angle) {
            Offset = direction;
            Angle = angle;
        }

        internal NavigationInfo(double offsetX = 0.0, double offsetY = 0.0, double angle = 0.0) {
            Offset = new Vector(offsetX, offsetY);
            Angle = angle;
        }

        public static NavigationInfo operator +(NavigationInfo navInfoA, NavigationInfo navInfoB) {
            decimal resultDirectionX = (decimal)navInfoA.Offset.X + (decimal)navInfoB.Offset.X;
            decimal resultDirectionY = (decimal)navInfoA.Offset.Y + (decimal)navInfoB.Offset.Y;
            decimal resultAngle = (decimal)navInfoA.Angle + (decimal)navInfoB.Angle;
            return new NavigationInfo((double)resultDirectionX, (double)resultDirectionY, (double)resultAngle);
        }

        public static NavigationInfo operator -(NavigationInfo navInfoA, NavigationInfo navInfoB) {
            decimal resultDirectionX = (decimal)navInfoA.Offset.X - (decimal)navInfoB.Offset.X;
            decimal resultDirectionY = (decimal)navInfoA.Offset.Y - (decimal)navInfoB.Offset.Y;
            decimal resultAngle = (decimal)navInfoA.Angle - (decimal)navInfoB.Angle;
            return new NavigationInfo((double)resultDirectionX, (double)resultDirectionY, (double)resultAngle);
        }
    }
}
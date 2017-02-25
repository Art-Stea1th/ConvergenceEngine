using System.Windows;


namespace ConvergenceEngine.Models.Mapping.Navigation {

    using Extensions;

    public sealed class NavigationInfo {

        public double X { get; }
        public double Y { get; }
        public double A { get; } // angle

        public double NormalizedDirection { get { return A.AsNormalizedAngle(); } }

        public override string ToString() {
            return $"{X}, {Y}, {A}";
        }

        internal NavigationInfo(NavigationInfo navigationInfo) {
            X = navigationInfo.X; Y = navigationInfo.Y; A = navigationInfo.A;
        }

        internal NavigationInfo(Vector direction, double angle) {
            X = direction.X; Y = direction.Y; A = angle;
        }

        internal NavigationInfo(double offsetX = 0.0, double offsetY = 0.0, double angle = 0.0) {
            X = offsetX; Y = offsetY; A = angle;
        }

        public static NavigationInfo operator +(NavigationInfo navInfoA, NavigationInfo navInfoB) {
            decimal resultDirectionX = (decimal)navInfoA.X + (decimal)navInfoB.X;
            decimal resultDirectionY = (decimal)navInfoA.Y + (decimal)navInfoB.Y;
            decimal resultAngle = (decimal)navInfoA.A + (decimal)navInfoB.A;
            return new NavigationInfo((double)resultDirectionX, (double)resultDirectionY, (double)resultAngle);
        }

        public static NavigationInfo operator -(NavigationInfo navInfoA, NavigationInfo navInfoB) {
            decimal resultDirectionX = (decimal)navInfoA.X - (decimal)navInfoB.X;
            decimal resultDirectionY = (decimal)navInfoA.Y - (decimal)navInfoB.Y;
            decimal resultAngle = (decimal)navInfoA.A - (decimal)navInfoB.A;
            return new NavigationInfo((double)resultDirectionX, (double)resultDirectionY, (double)resultAngle);
        }
    }
}
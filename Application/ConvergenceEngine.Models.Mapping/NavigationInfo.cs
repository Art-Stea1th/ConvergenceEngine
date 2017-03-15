using System.Runtime.CompilerServices;
using System.Windows;

namespace ConvergenceEngine.Models.Mapping {

    using Infrastructure.Interfaces;

    public sealed class NavigationInfo : INavigationInfo {

        public double X { get; private set; }
        public double Y { get; private set; }
        public double A { get; private set; }

        public override string ToString() {
            return $"{X}, {Y}, {A}";
        }

        internal NavigationInfo(INavigationInfo navigationInfo) {
            X = navigationInfo.X; Y = navigationInfo.Y; A = navigationInfo.A;
        }

        internal NavigationInfo(Vector direction, double angle) {
            X = direction.X; Y = direction.Y; A = angle;
        }

        internal NavigationInfo(double offsetX = 0.0, double offsetY = 0.0, double angle = 0.0) {
            X = offsetX; Y = offsetY; A = angle;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NavigationInfo operator +(NavigationInfo navInfoA, NavigationInfo navInfoB) {
            return new NavigationInfo(navInfoA.X + navInfoB.X, navInfoA.Y + navInfoB.Y, navInfoA.A + navInfoB.A);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NavigationInfo operator -(NavigationInfo navInfoA, NavigationInfo navInfoB) {
            return new NavigationInfo(navInfoA.X - navInfoB.X, navInfoA.Y - navInfoB.Y, navInfoA.A - navInfoB.A);
        }
    }
}
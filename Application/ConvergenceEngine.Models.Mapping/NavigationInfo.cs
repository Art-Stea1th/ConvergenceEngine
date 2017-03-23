using System.Runtime.CompilerServices;
using System.Windows;

namespace ConvergenceEngine.Models.Mapping {

    using Infrastructure.Interfaces;

    public struct NavigationInfo : INavigationInfo {

        decimal x, y, a;

        public double X { get => (double)x; }
        public double Y { get => (double)y; }
        public double A { get => (double)a; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NavigationInfo operator +(NavigationInfo navInfoA, NavigationInfo navInfoB) {
            return new NavigationInfo(navInfoA.x + navInfoB.x, navInfoA.y + navInfoB.y, navInfoA.a + navInfoB.a);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NavigationInfo operator -(NavigationInfo navInfoA, NavigationInfo navInfoB) {
            return new NavigationInfo(navInfoA.x - navInfoB.x, navInfoA.y - navInfoB.y, navInfoA.a - navInfoB.a);
        }

        public override string ToString() {
            return $"{x}, {y}, {a}";
        }

        internal NavigationInfo(INavigationInfo navigationInfo)
            : this((NavigationInfo)navigationInfo) { }

        internal NavigationInfo(NavigationInfo navigationInfo)
            : this(navigationInfo.x, navigationInfo.y, navigationInfo.a) { }

        internal NavigationInfo(decimal x, decimal y, decimal a) {
            this.x = x; this.y = y; this.a = a;
        }

        internal NavigationInfo(Vector direction, double angle)
            : this(direction.X, direction.Y, angle) { }

        internal NavigationInfo(double x, double y, double a) {
            this.x = (decimal)x; this.y = (decimal)y; this.a = (decimal)a;
        }
    }
}
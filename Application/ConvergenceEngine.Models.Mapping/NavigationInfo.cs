using System.Windows;

namespace ConvergenceEngine.Models.Mapping {

    using Infrastructure.Interfaces;

    public sealed class NavigationInfo : INavigationInfo {

        decimal _x, _y, _a;

        public double X => (double)_x;
        public double Y => (double)_y;
        public double A => (double)_a;

        public static NavigationInfo operator +(NavigationInfo navInfoA, NavigationInfo navInfoB)
            => new NavigationInfo(navInfoA._x + navInfoB._x, navInfoA._y + navInfoB._y, navInfoA._a + navInfoB._a);

        public static NavigationInfo operator -(NavigationInfo navInfoA, NavigationInfo navInfoB)
            => new NavigationInfo(navInfoA._x - navInfoB._x, navInfoA._y - navInfoB._y, navInfoA._a - navInfoB._a);

        public static NavigationInfo operator /(NavigationInfo navInfo, double number)
            => navInfo / (decimal)number;

        public static NavigationInfo operator /(NavigationInfo navInfo, decimal number)
            => new NavigationInfo(navInfo._x / number, navInfo._y / number, navInfo._a / number);

        public static NavigationInfo operator -(NavigationInfo navInfo)
            => new NavigationInfo(-navInfo._x, -navInfo._y, -navInfo._a);

        public override string ToString() => $"{_x}, {_y}, {_a}";

        public NavigationInfo() : this(0m, 0m, 0m) { }

        internal NavigationInfo(INavigationInfo navigationInfo)
            : this((NavigationInfo)navigationInfo) { }

        internal NavigationInfo(NavigationInfo navigationInfo)
            : this(navigationInfo._x, navigationInfo._y, navigationInfo._a) { }

        internal NavigationInfo(decimal x, decimal y, decimal a) {
            _x = x; _y = y; _a = a;
        }

        internal NavigationInfo(Vector direction, double angle)
            : this(direction.X, direction.Y, angle) { }

        internal NavigationInfo(double x, double y, double a) {
            _x = (decimal)x; _y = (decimal)y; _a = (decimal)a;
        }
    }
}
using System;
using System.Runtime.CompilerServices;
using System.Windows;

namespace SLAM.Models.Extensions {

    public static class PointExtensions {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double DistanceTo(this Point p, Point point) {
            return Math.Sqrt(
                Math.Pow(point.X - p.X, 2.0) +
                Math.Pow(point.Y - p.Y, 2.0));
        }
    }
}
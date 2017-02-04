using System.Runtime.CompilerServices;
using System.Windows;


namespace SLAM.Models.Extensions {

    public static class PointExtensions {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double DistanceTo(this Point p, Point point) {
            return (point - p).Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double DistanceTo(this Point pointC, Point pointA, Point pointB) {
            Vector ab = pointB - pointA;
            Vector ac = pointC - pointA;
            ab.Normalize();
            return (pointC - (Vector.Multiply(ab, ac) * ab + pointA)).Length;
        }
    }
}
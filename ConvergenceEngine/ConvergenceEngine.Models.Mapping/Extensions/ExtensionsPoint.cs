using System;
using System.Runtime.CompilerServices;
using System.Windows;


namespace ConvergenceEngine.Models.Mapping.Extensions {

    internal static partial class Extensions {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Point Rotate(this Point p, double angle) {
            return p.RotateRadians(angle.AsNormalizedAngle().ToRadians());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Point RotateAt(this Point p, double angle, double centerX, double centerY) {
            return p.RotateRadiansAt(angle.AsNormalizedAngle().ToRadians(), centerX, centerY);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Point RotateRadiansAt(this Point p, double angle, double centerX, double centerY) {
            var result = new Point(p.X - centerX, p.Y - centerY).RotateRadians(angle);
            return new Point(result.X + centerX, result.Y + centerY);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Point RotateRadians(this Point p, double angle) {
            var cosA = Math.Cos(angle);
            var sinA = Math.Sin(angle);
            return new Point((p.X * cosA) - (p.Y * sinA), (p.Y * cosA) + (p.X * sinA));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double DistanceTo(this Point pointC, Point pointA, Point pointB) {
            return (pointC - pointC.DistancePointTo(pointA, pointB)).Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double DistanceTo(this Point p, Point point) {
            return p.ConvergenceTo(point).Length;
        }

        public static Point DistancePointTo(this Point pointC, Point pointA, Point pointB) {

            if (pointC == pointA || pointC == pointB) {
                return pointC;
            }
            if (pointA == pointB) {
                return pointA;
            }
            Vector ab = pointA.ConvergenceTo(pointB);
            Vector ac = pointA.ConvergenceTo(pointC);
            ab.Normalize();
            return Vector.Multiply(ab, ac) * ab + pointA;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector ConvergenceTo(this Point p, Point point) {
            return point - p;
        }
    }
}
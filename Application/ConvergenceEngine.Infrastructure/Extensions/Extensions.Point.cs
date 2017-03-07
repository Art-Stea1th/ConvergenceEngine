using System;
using System.Runtime.CompilerServices;
using System.Windows;


namespace ConvergenceEngine.Infrastructure.Extensions {

    public static partial class Extensions {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Point ShiftedAndRotated(this Point p, double offsetX, double offsetY, double angle) {
            p.Offset(offsetX, offsetY); return p.Rotated(angle);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Point RotatedAndShifted(this Point p, double offsetX, double offsetY, double angle) {
            p = p.Rotated(angle); p.Offset(offsetX, offsetY); return p;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Point Rotated(this Point p, double angle) {
            return p.RotatedRadians(angle.AsNormalizedAngle().ToRadians());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Point RotatedAt(this Point p, double angle, double centerX, double centerY) {
            return p.RotatedRadiansAt(angle.AsNormalizedAngle().ToRadians(), centerX, centerY);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Point RotatedRadiansAt(this Point p, double angle, double centerX, double centerY) {
            p.Offset(-centerX, -centerY); p = p.RotatedRadians(angle); p.Offset(centerX, centerY);
            return p;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Point RotatedRadians(this Point p, double angle) {
            var cosA = Math.Cos(angle);
            var sinA = Math.Sin(angle); // repeated new for max XY precision (expressions in the initializer) / tested
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
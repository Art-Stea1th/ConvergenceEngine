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
        public static Point Shifted(this Point p, double offsetX, double offsetY) {
            p.Offset(offsetX, offsetY); return p;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Point ShiftedX(this Point p, double offsetX) {
            p.Offset(offsetX, 0); return p;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Point ShiftedY(this Point p, double offsetY) {
            p.Offset(0, offsetY); return p;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Point Rotated(this Point p, double angle) {
            return p.RotatedRadians(angle.AsNormalizedAngle().ToRadians());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Point RotatedAt(this Point p, double angle, Point center) {
            return p.RotatedRadiansAt(angle.AsNormalizedAngle().ToRadians(), center);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Point RotatedAt(this Point p, double angle, double centerX, double centerY) {
            return p.RotatedRadiansAt(angle.AsNormalizedAngle().ToRadians(), centerX, centerY);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Point RotatedRadiansAt(this Point p, double angle, Point center) {
            p.Offset(-center.X, -center.Y); p = p.RotatedRadians(angle); p.Offset(center.X, center.Y);
            return p;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Point RotatedRadiansAt(this Point p, double angle, double centerX, double centerY) {
            p.Offset(-centerX, -centerY); p = p.RotatedRadians(angle); p.Offset(centerX, centerY);
            return p;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Point RotatedRadians(this Point p, double angle) {
            double cosA = Math.Cos(angle);
            double sinA = Math.Sin(angle); // repeated new for max XY precision (expressions in the initializer) / tested
            return new Point((p.X * cosA) - (p.Y * sinA), (p.Y * cosA) + (p.X * sinA));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double DistanceTo(this Point pointC, Point pointA, Point pointB) {
            return (pointC - pointC.DistancePointTo(pointA, pointB)).Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double DistanceTo(this Point p, Point point) {
            return p.DistanceVectorTo(point).Length;
        }

        public static Point DistancePointTo(this Point pointC, Point pointA, Point pointB) {

            if (pointC == pointA || pointC == pointB) {
                return pointC;
            }
            if (pointA == pointB) {
                return pointA;
            }
            var ab = pointA.DistanceVectorTo(pointB);
            var ac = pointA.DistanceVectorTo(pointC);
            ab.Normalize();
            return Vector.Multiply(ab, ac) * ab + pointA;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector DistanceVectorTo(this Point p, Point point) {
            return point - p;
        }
    }
}
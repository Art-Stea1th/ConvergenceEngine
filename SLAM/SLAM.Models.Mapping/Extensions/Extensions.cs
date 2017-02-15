using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;


namespace SLAM.Models.Mapping.Extensions {

    // Double
    internal static partial class Extensions {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double ToDegrees(this double radians) {
            return (radians * 180.0) / Math.PI;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double ToRadians(this double degrees) {
            return (Math.PI / 180.0) * degrees;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double AsNormalizedAngle(this double degrees) {
            return ((int)(degrees %= 360.0) / 180) % 2 == 0 ? degrees : (degrees > 0 ? degrees - 360.0 : degrees + 360.0);
            //       if it even so (-180 < direction < 180), return;  //  else clamp direction to -180 / 180 and return;
        }
    }

    // IEnumerable
    internal static partial class Extensions {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> sequence) {
            return sequence.IsNull() || sequence.IsEmpty();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNull<T>(this IEnumerable<T> sequence) {
            return sequence == null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsEmpty<T>(this IEnumerable<T> sequence) {
            return sequence.Count() < 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<TResult> DoSequential<T1, T2, TResult>(
            this IEnumerable<T1> sq, IEnumerable<T2> sequence, Func<T1, T2, TResult> selector) {

            var sqEnumerator = sq.GetEnumerator();
            var sequenceEnumerator = sequence.GetEnumerator();

            while (sqEnumerator.MoveNext() && sequenceEnumerator.MoveNext()) {
                yield return selector.Invoke(sqEnumerator.Current, sequenceEnumerator.Current);
            }
        }

        public static Vector Sum(this IEnumerable<Vector> sequence) {
            var enumerator = sequence.GetEnumerator();

            enumerator.MoveNext();
            var result = enumerator.Current;

            while (enumerator.MoveNext()) {
                result += enumerator.Current;
            }
            return result;
        }
    }

    // Point
    internal static partial class Extensions {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Point Rotate(this Point p, double angle) {
            return p.RotateRadians(angle.ToRadians());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Point RotateAt(this Point p, Point point, double angle) {
            return p.RotateRadiansAt(point, angle.ToRadians());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Point RotateRadiansAt(this Point p, Point point, double angle) {
            var res = ((Point)(p - point)).RotateRadians(angle);
            return new Point(res.X + point.X, res.Y + point.Y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Point RotateRadians(this Point p, double angle) {
            var cosA = Math.Cos(angle);
            var sinA = Math.Sin(angle);
            return new Point((p.X * cosA) - (p.Y * sinA), (p.Y * cosA) + (p.X * sinA));
        }        

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double DistanceTo(this Point p, Point point) {
            return p.ConvergenceTo(point).Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double DistanceTo(this Point pointC, Point pointA, Point pointB) {
            return (pointC - pointC.DistancePointTo(pointA, pointB)).Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Point DistancePointTo(this Point pointC, Point pointA, Point pointB) {
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
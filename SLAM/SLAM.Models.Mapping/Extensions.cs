using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Windows;


namespace SLAM.Models.Mapping {

    internal static partial class Extensions {

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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNullOrEmpty<T>(this ICollection<T> sequence) {
            return sequence.IsNull() || sequence.IsEmpty();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNull<T>(this ICollection<T> sequence) {
            return sequence == null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsEmpty<T>(this ICollection<T> sequence) {
            return sequence.Count < 1;
        }        
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;

namespace SLAM.Models.Mapping {

    using PointSequence = List<Point>;

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

        public static Segment GetApproximated(this ICollection<Point> sequence) {

            Point p0 = sequence.First(), pN = sequence.Last();

            double avgX = sequence.Average(p => p.X);
            double avgY = sequence.Average(p => p.Y);
            double avgXY = sequence.Average(p => p.X * p.Y);
            double avgSqX = sequence.Average(p => Math.Pow(p.X, 2));
            double sqAvgX = Math.Pow(avgX, 2);

            double A = (avgXY - avgX * avgY) / (avgSqX - sqAvgX);
            double B = avgY - A * avgX;

            return new Segment(new Point(p0.X, A * p0.X + B), new Point(pN.X, A * pN.X + B));
        }

        public static IEnumerable<PointSequence> Segmentate(this PointSequence sequence) {

            List<PointSequence> result = new List<PointSequence>();

            if (!sequence.IsNullOrEmpty()) {
                var pair = SplitByMaxPoint(sequence);

                if (pair == null) {
                    if (IsValidSequence(sequence)) {
                        result.Add(new Segment(sequence).ToList()); // <--- Linear
                        //result.Add(sequence.GetApproximated());     // <--- OLS
                    }
                }
                else {
                    result.AddRange(Segmentate(pair.Item1));
                    result.AddRange(Segmentate(pair.Item2));
                }
            }
            return result;
        }

        #region Segmentate resources

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsValidSequence(PointSequence sequence) {
            var averageDistanceBetweenPoints = sequence.First().DistanceTo(sequence.Last()) / (sequence.Count - 1);
            return averageDistanceBetweenPoints <= ExpectedDistanceBetweenPoints(sequence.Average(p => p.Y));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double ExpectedDistanceBetweenPoints(double segmentPositionY) {
            double a = 100.0 * (2.0 / 3.0), b = a; // magic coefficients // a=b = 100.0 * (2.0 / 3.0);
            return (segmentPositionY - b) / a;     // y = ax + b; => x = (y - b) / a;
        }

        private static Tuple<PointSequence, PointSequence> SplitByMaxPoint(PointSequence sequence) {

            if (sequence.Count >= 4) {
                var maxDistancePointIndex = FindIndexOfMaxDistancePoint(sequence);

                if (!AllowedPointDistanceToLine(sequence[maxDistancePointIndex], sequence.First(), sequence.Last())) {
                    var left = sequence.TakeWhile((p, i) => i <= maxDistancePointIndex).ToList();
                    var right = sequence.SkipWhile((p, i) => i < maxDistancePointIndex).ToList();
                    return new Tuple<PointSequence, PointSequence>(left, right);
                }
            }
            return null;
        }

        private static bool AllowedPointDistanceToLine(Point point, Point lineStart, Point lineEnd) {
            var distance = point.DistanceTo(lineStart, lineEnd);
            var averagegPositionY = (point.Y + lineStart.Y + lineEnd.Y) / 3;
            return distance < averagegPositionY * 0.03 ? true : false; // distance < 3.0% of AvgY
        }

        private static int FindIndexOfMaxDistancePoint(PointSequence sequence) {

            var maxDistance = 0.0;
            var maxDistancePointIndex = 0;

            for (int i = 0; i < sequence.Count; ++i) {
                double currentDistance = sequence[i].DistanceTo(sequence.First(), sequence.Last());
                if (currentDistance > maxDistance) {
                    maxDistance = currentDistance;
                    maxDistancePointIndex = i;
                }
            }
            return maxDistancePointIndex;
        }
        #endregion
    }
}
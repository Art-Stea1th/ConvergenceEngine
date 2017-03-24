using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;

namespace ConvergenceEngine.Models.Mapping.Extensions {

    using Infrastructure.Extensions;

    internal static class Segmenter { // IEnumerable<Point> Extension class

        public static IEnumerable<List<Point>> Segmentate(this IEnumerable<Point> points, double allowedDivergencePercent = 3.0) {

            var result = new List<List<Point>>();

            if (!points.IsNullOrEmpty()) {

                var segment = new List<Point>(points);
                var segmentPair = SplitByMaxDivergencePoint(segment, allowedDivergencePercent);

                if (segmentPair.left == null || segmentPair.right == null) {
                    if (IsValidSequence(segment)) {
                        result.Add(segment);
                    }
                }
                else {
                    result.AddRange(segmentPair.left.Segmentate(allowedDivergencePercent));
                    result.AddRange(segmentPair.right.Segmentate(allowedDivergencePercent));
                }
            }
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsValidSequence(IReadOnlyCollection<Point> sequence) {
            double averageDistanceBetweenPoints = sequence.First().DistanceTo(sequence.Last()) / (sequence.Count - 1);
            return averageDistanceBetweenPoints <= ExpectedDistanceBetweenPoints(sequence.Average(p => p.Y));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double ExpectedDistanceBetweenPoints(double segmentPositionY) {
            double a = 100.0 * (2.0 / 3.0), b = a; // magic coefficients // a = b = 100.0 * (2.0 / 3.0);
            return (segmentPositionY - b) / a;     // y = ax + b; => x = (y - b) / a;
        }

        private static (IEnumerable<Point> left, IEnumerable<Point> right) SplitByMaxDivergencePoint(
            IReadOnlyList<Point> points, double allowedDivergencePercent) {

            int maxDivergencePointIndex = points.IndexOfMaxBy(p => p.DistanceTo(points.First(), points.Last()));

            double allowedDivergence = AveragePositionY(
                points[maxDivergencePointIndex], points.First(), points.Last()) * (allowedDivergencePercent / 100.0);

            if (points[maxDivergencePointIndex].DistanceTo(points.First(), points.Last()) > allowedDivergence) {
                return points.SplitBy(maxDivergencePointIndex);
            }
            return (null, null);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double AveragePositionY(Point point, Point lineStart, Point lineEnd) {
            return Math.Abs((point.Y + lineStart.Y + lineEnd.Y) / 3);
        }
    }
}
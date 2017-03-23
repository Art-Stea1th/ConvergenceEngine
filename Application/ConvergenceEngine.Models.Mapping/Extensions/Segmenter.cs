using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;

namespace ConvergenceEngine.Models.Mapping.Extensions {

    using Infrastructure.Extensions;

    internal static class Segmenter { // IEnumerable<Point> Extension class

        public static IEnumerable<List<Point>> Segmentate(this IEnumerable<Point> points, double allowedDivergencePercent = 3.0) {

            List<List<Point>> result = new List<List<Point>>();

            if (!points.IsNullOrEmpty()) {

                List<Point> segment = new List<Point>(points);
                var segmentPair = SplitByMaxDivergencePoint(segment, allowedDivergencePercent);

                if (segmentPair.Left == null || segmentPair.Right == null) {
                    if (IsValidSequence(segment)) {
                        result.Add(segment);
                    }
                }
                else {
                    result.AddRange(segmentPair.Left.Segmentate(allowedDivergencePercent));
                    result.AddRange(segmentPair.Right.Segmentate(allowedDivergencePercent));
                }
            }
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsValidSequence(IReadOnlyCollection<Point> sequence) {
            var averageDistanceBetweenPoints = sequence.First().DistanceTo(sequence.Last()) / (sequence.Count - 1);
            return averageDistanceBetweenPoints <= ExpectedDistanceBetweenPoints(sequence.Average(p => p.Y));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double ExpectedDistanceBetweenPoints(double segmentPositionY) {
            double a = 100.0 * (2.0 / 3.0), b = a; // magic coefficients // a = b = 100.0 * (2.0 / 3.0);
            return (segmentPositionY - b) / a;     // y = ax + b; => x = (y - b) / a;
        }

        private static (IEnumerable<Point> Left, IEnumerable<Point> Right) SplitByMaxDivergencePoint(
            IReadOnlyList<Point> points, double allowedDivergencePercent) {

            var maxDivergencePointIndex = points.IndexOfMaxBy(p => p.DistanceTo(points.First(), points.Last()));

            var allowedDivergence = AveragePositionY(
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
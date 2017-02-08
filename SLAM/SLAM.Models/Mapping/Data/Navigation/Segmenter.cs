using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;


namespace SLAM.Models.Mapping.Data.Navigation {

    using Extensions;

    internal sealed class Segmenter {

        private Approximator approximator = new Approximator();

        internal ICollection<IList<Point>> ToSegment(IList<Point> sequence) {

            List<IList<Point>> result = new List<IList<Point>>();

            if (!sequence.IsNullOrEmpty()) {
                var pair = SplitByMaxPoint(sequence);

                if (pair == null) {
                    if (IsValidSequence(sequence)) {
                        result.Add(new List<Point> { sequence.First(), sequence.Last() });       // <--- Linear
                        //var resultLine = approximator.ApproximateByOrdinaryLeastSquares(sequence); // <--- OLS
                        //result.Add(new List<Point> { resultLine.Item1, resultLine.Item2 });
                    }
                }
                else {
                    result.AddRange(ToSegment(pair.Item1));
                    result.AddRange(ToSegment(pair.Item2));
                }
            }
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsValidSequence(ICollection<Point> sequence) {
            var averageDistanceBetweenPoints = sequence.First().DistanceTo(sequence.Last()) / (sequence.Count - 1);
            return averageDistanceBetweenPoints <= ExpectedDistanceBetweenPoints(sequence.Average(p => p.Y));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private double ExpectedDistanceBetweenPoints(double segmentPositionY) {
            double a = 100.0 * (2.0 / 3.0), b = a; // magic coefficients // a=b = 100.0 * (2.0 / 3.0);
            return (segmentPositionY - b) / a;     // y = ax + b; => x = (y - b) / a;
        }

        private Tuple<IList<Point>, IList<Point>> SplitByMaxPoint(IList<Point> sequence) {

            if (sequence.Count >= 4) {
                var maxDistancePointIndex = FindIndexOfMaxDistancePoint(sequence);

                if (!AllowedPointDistanceToLine(sequence[maxDistancePointIndex], sequence.First(), sequence.Last())) {
                    var left = sequence.TakeWhile((p, i) => i <= maxDistancePointIndex).ToList();
                    var right = sequence.SkipWhile((p, i) => i < maxDistancePointIndex).ToList();
                    return new Tuple<IList<Point>, IList<Point>>(left, right);
                }
            }
            return null;
        }

        private bool AllowedPointDistanceToLine(Point point, Point lineStart, Point lineEnd) {
            var distance = point.DistanceTo(lineStart, lineEnd);
            var averagegPositionY = (point.Y + lineStart.Y + lineEnd.Y) / 3;
            return distance < averagegPositionY * 0.03 ? true : false; // distance < 3.0% of AvgY
        }

        private int FindIndexOfMaxDistancePoint(IList<Point> sequence) {

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
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;


namespace SLAM.Models.Mapping {

    internal sealed class SegmentSequence : List<Segment> {

        private SegmentSequence() { }

        public NavigationInfo Difference(SegmentSequence sequence) {

            IEnumerable<Segment> res;

            foreach (var segment in this) {
                res = segment.FindSegmentsWithNearestPoints(sequence, ((segment.PointA.Y + segment.PointB.Y) / 2) * 0.05);
                Console.WriteLine(res);
            }

            
            return new NavigationInfo();
        }

        public static SegmentSequence Segmentate(IList<Point> sequence) {

            SegmentSequence result = new SegmentSequence();

            if (!sequence.IsNullOrEmpty()) {
                var pair = SplitByMaxPoint(sequence);

                if (pair == null) {
                    if (IsValidSequence(sequence)) {
                        result.Add(Segment.CreateByFirstAndLastFrom(sequence)); // <--- Linear
                        //result.Add(Segment.ApproximateFrom(sequence));          // <--- OLS
                    }
                }
                else {
                    result.AddRange(Segmentate(pair.Item1));
                    result.AddRange(Segmentate(pair.Item2));
                }
            }
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsValidSequence(IList<Point> sequence) {
            var averageDistanceBetweenPoints = sequence.First().DistanceTo(sequence.Last()) / (sequence.Count - 1);
            return averageDistanceBetweenPoints <= ExpectedDistanceBetweenPoints(sequence.Average(p => p.Y));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double ExpectedDistanceBetweenPoints(double segmentPositionY) {
            double a = 100.0 * (2.0 / 3.0), b = a; // magic coefficients // a=b = 100.0 * (2.0 / 3.0);
            return (segmentPositionY - b) / a;     // y = ax + b; => x = (y - b) / a;
        }

        private static Tuple<IList<Point>, IList<Point>> SplitByMaxPoint(IList<Point> sequence) {

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

        private static bool AllowedPointDistanceToLine(Point point, Point lineStart, Point lineEnd) {
            var distance = point.DistanceTo(lineStart, lineEnd);
            var averagegPositionY = (point.Y + lineStart.Y + lineEnd.Y) / 3;
            return distance < averagegPositionY * 0.03 ? true : false; // distance < 3.0% of AvgY
        }

        private static int FindIndexOfMaxDistancePoint(IList<Point> sequence) {

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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;


namespace SLAM.Models.Mapping.Data.Navigation {

    using Extensions;    

    internal sealed class Segmenter {

        internal IList<IList<Point>> ToSegment(IList<Point> sequence) {

            List<IList<Point>> result = new List<IList<Point>>();

            var pair = SplitByMaxPoint(sequence);

            if (pair == null) {
                if (IsValidSequence(sequence.First(), sequence.Last(), sequence.Count)) {
                    result.Add(new List<Point> { sequence.First(), sequence.Last() }); // <--- Linear
                }
            }
            else {
                result.AddRange(ToSegment(pair.Item1));
                result.AddRange(ToSegment(pair.Item2));
            }
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsValidSequence(Point first, Point last, int pointsCount) {
            return first.DistanceTo(last) / pointsCount <= 3.0 ? true : false;
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
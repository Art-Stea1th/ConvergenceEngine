using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SLAM.Models.Mapping.Data.Navigation {

    using Extensions;
    using System.Runtime.CompilerServices;

    internal sealed class Skeleton {

        private IList<Point> points;
        internal IList<IList<Point>> Segments { get { return QuickInterpolation(points); } }

        internal Skeleton(IList<Point> points) {
            this.points = points;
        }

        private IList<IList<Point>> QuickInterpolation(IList<Point> sequence) {

            List<IList<Point>> result = new List<IList<Point>>();

            var pair = SplitByMaxPoint(sequence);

            if (pair == null) {
                result.Add(new List<Point> { sequence.First(), sequence.Last() }); // <--- Linear
            }
            else {
                result.AddRange(QuickInterpolation(pair.Item1));
                result.AddRange(QuickInterpolation(pair.Item2));
            }
            return result;
        }

        private Tuple<IList<Point>, IList<Point>> SplitByMaxPoint(IList<Point> sequence) {

            if (sequence.Count >= 4) {
                var maxDistancePointIndex = FindIndexOfMaxDistancePoint(sequence);
                var maxDistancePoint = sequence[maxDistancePointIndex];
                var maxDistance = sequence[maxDistancePointIndex].DistanceTo(sequence.First(), sequence.Last());

                if (maxDistance > PercentOfNumber(2, maxDistancePoint.Y)) {
                    var left = sequence.TakeWhile((p, i) => i <= maxDistancePointIndex).ToList();
                    var right = sequence.SkipWhile((p, i) => i < maxDistancePointIndex).ToList();
                    return new Tuple<IList<Point>, IList<Point>>(left, right);
                }
            }
            return null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private double PercentOfNumber(double percent, double number) {
            return number * 0.01 * percent;
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

        private void RemoveBadSegments() { }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private double AveragePointsDestiny(ICollection<Point> points) {
            return points.First().DistanceTo(points.Last()) / points.Count;
        }
    }
}
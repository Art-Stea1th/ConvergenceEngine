using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace ConvergenceEngine.Models.Mapping.Extensions.Ops {

    using Iterable;
    using Segments;

    internal static class Determinator {

        public static NavigationInfo ConvergenceTo(this IEnumerable<MultiPointsSegment> current, IEnumerable<MultiPointsSegment> another,
            double maxDistancePercent = 5.0, double maxAngleDegrees = 3.0) {

            var trackedPairs = current.SelectSimilarTo(another, maxDistancePercent, maxAngleDegrees);
            if (trackedPairs.IsNullOrEmpty()) {
                return new NavigationInfo(0.0, 0.0, 0.0); // to be processed later
            }

            var trackedCurrent = new List<MultiPointsSegment>(trackedPairs.Select(s => s.Item1));
            var trackedAnother = new List<MultiPointsSegment>(trackedPairs.Select(s => s.Item2));

            double resultAngle = trackedCurrent.DetermineAngleTo(trackedAnother);

            trackedAnother = new List<MultiPointsSegment>(trackedAnother.Select(
                s => new MultiPointsSegment(new List<Point> { s.PointA.Rotate(-resultAngle), s.PointB.Rotate(-resultAngle) })));

            Vector resultDirection = trackedCurrent.DetermineDirectionTo(trackedAnother);

            return new NavigationInfo(resultDirection, resultAngle);
        }

        public static Vector DetermineDirectionTo(this IReadOnlyList<MultiPointsSegment> current, IReadOnlyList<MultiPointsSegment> another) {

            if (current.Count != another.Count) {
                throw new ArgumentOutOfRangeException();
            }
            var heights = current.Sequential(another, (c, a) => c.CenterPoint.ConvergenceTo(c.CenterPoint.DistancePointTo(a.PointA, a.PointB)));
            return heights.OrderByAngle().ApproximateSorted();
        }

        public static double DetermineAngleTo(this IEnumerable<MultiPointsSegment> current, IEnumerable<MultiPointsSegment> another) {

            var lehgths = current.Sequential(another, (c, a) => (c.Length + a.Length) * 0.5);
            var angles = current.Sequential(another, (c, a) => SkeletonSegment.AngleBetween(c, a));

            return AverageWeightedByLengthsAngle(angles, lehgths);
        }

        private static double AverageWeightedByLengthsAngle(IEnumerable<double> angles, IEnumerable<double> lengths) {

            var fullLength = lengths.Sum();
            var weights = lengths.Select(s => s * 100.0 / fullLength);

            return angles.Sequential(weights, (a, w) => a / 100.0 * w).Sum();
        }
    }
}
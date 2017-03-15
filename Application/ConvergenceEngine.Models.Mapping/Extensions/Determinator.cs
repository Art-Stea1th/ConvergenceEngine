using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace ConvergenceEngine.Models.Mapping.Extensions {

    using Segments;
    using Infrastructure.Extensions;

    internal static class Determinator {

        public static NavigationInfo ComputeConvergence(this IEnumerable<Tuple<Segment, Segment>> trackedPairs,
            double maxDistancePercent, double maxAngleDegrees) {

            var trackedCurrent = trackedPairs.Select(s => s.Item1);
            var trackedAnother = trackedPairs.Select(s => s.Item2);

            double resultAngle = trackedCurrent.DetermineAngleTo(trackedAnother);

            trackedAnother = trackedAnother.Select(s => new Segment(s.A.Rotated(-resultAngle), s.B.Rotated(-resultAngle)));

            Vector resultDirection = trackedCurrent.DetermineDirectionTo(trackedAnother);

            return new NavigationInfo(resultDirection, resultAngle);
        }

        public static Vector DetermineDirectionTo(this IEnumerable<Segment> current, IEnumerable<Segment> another) {

            if (current.IsNullOrEmpty() || another.IsNullOrEmpty()) {
                return new Vector();
            }

            var heights = current.Sequential(another, (c, a) => c.Center.DistanceVectorTo(c.Center.DistancePointTo(a.A, a.B)));
            return heights.OrderByAngle().ApproximateOrdered();
        }

        public static double DetermineAngleTo(this IEnumerable<Segment> current, IEnumerable<Segment> another) {

            var lehgths = current.Sequential(another, (c, a) => (c.Length + a.Length) * 0.5);
            var angles = current.Sequential(another, (c, a) => c.AngleTo(a));

            return AverageWeightedByLengthsAngle(angles, lehgths);
        }

        private static double AverageWeightedByLengthsAngle(IEnumerable<double> angles, IEnumerable<double> lengths) {

            var fullLength = lengths.Sum();
            var weights = lengths.Select(s => s * 100.0 / fullLength);

            return angles.Sequential(weights, (a, w) => a / 100.0 * w).Sum();
        }
    }
}
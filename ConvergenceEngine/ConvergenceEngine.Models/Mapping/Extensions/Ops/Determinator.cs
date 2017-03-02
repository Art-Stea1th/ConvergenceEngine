using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace ConvergenceEngine.Models.Mapping.Extensions.Ops {

    using Iterable;

    internal static class Determinator {

        public static Vector DetermineDirectionTo(this IReadOnlyList<Segment> current, IReadOnlyList<Segment> another) {

            if (current.Count != another.Count) {
                throw new ArgumentOutOfRangeException();
            }
            var heights = current.DoSequential(another, (c, a) => c.MiddlePoint().ConvergenceTo(c.MiddlePoint().DistancePointTo(a.PointA, a.PointB)));
            return heights.Approximate();
        }

        public static double DetermineAngleTo(this IEnumerable<Segment> current, IEnumerable<Segment> another) {

            var lehgths = current.DoSequential(another, (c, a) => (c.Length + a.Length) * 0.5);
            var angles = current.DoSequential(another, (c, a) => Segment.AngleBetween(c, a));

            return AverageWeightedByLengthsAngle(angles, lehgths);
        }

        private static double AverageWeightedByLengthsAngle(IEnumerable<double> angles, IEnumerable<double> lengths) {

            var fullLength = lengths.Sum();
            var weights = lengths.Select(s => s * 100.0 / fullLength);

            return angles.DoSequential(weights, (a, w) => a / 100.0 * w).Sum();
        }
    }
}
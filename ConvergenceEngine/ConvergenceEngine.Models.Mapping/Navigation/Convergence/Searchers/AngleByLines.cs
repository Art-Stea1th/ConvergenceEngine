using System.Collections.Generic;
using System.Linq;

namespace ConvergenceEngine.Models.Mapping.Navigation.Convergence.Searchers {

    using Extensions;
    using Segmentation;

    internal static class AngleByLines { // as static TMP

        public static double SearchBetween(IEnumerable<Segment> current, IEnumerable<Segment> another) {

            var lehgths = current.DoSequential(another, (c, a) => (c.Length + a.Length) * 0.5);
            var angles = current.DoSequential(another, (c, a) => Segment.AngleBetween(c, a));

            return AverageWeightedByLengthsAngle(lehgths, angles);
        }

        private static double AverageWeightedByLengthsAngle(IEnumerable<double> lengths, IEnumerable<double> angles) {

            var fullLength = lengths.Sum();
            var weights = lengths.Select(s => s * 100.0 / fullLength);

            return angles.DoSequential(weights, (a, w) => a / 100.0 * w).Sum();
        }
    }
}
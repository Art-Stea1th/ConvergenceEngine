using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace ConvergenceEngine.Models.Mapping.Extensions {

    using ConvergenceEngine.Infrastructure.Interfaces;
    using Infrastructure.Extensions;
    using Segments;

    internal static class Determinator {

        public static NavigationInfo ComputeConvergence(this IEnumerable<(ISegment current, ISegment nearest)> trackedPairs,
            double maxDistancePercent, double maxAngleDegrees) {

            if (trackedPairs.IsNullOrEmpty()) {
                return null; // !
            }

            double resultAngle = AverageWeightedAngle(trackedPairs);

            trackedPairs = trackedPairs.Select(sp => (
                current: sp.current,
                nearest: (sp.nearest as Segment).RotatedAtZero(-resultAngle) as ISegment));

            var resultDirection = AverageDirection(trackedPairs);

            return new NavigationInfo(resultDirection, resultAngle);
        }

        public static double AverageWeightedAngle(this IEnumerable<(ISegment current, ISegment nearest)> trackedPairs) {

            var weightedVectors = trackedPairs.Select(sp => (
            vector: (sp.current as Segment).AngleTo(sp.nearest).DegreesToVector(),
            weight: sp.current.Length + sp.nearest.Length));

            double weightsSum = weightedVectors.Sum(wv => wv.weight);

            return Vector.AngleBetween(Segment.BasisX, new Vector(
                    weightedVectors.Sum(wv => wv.vector.X * wv.weight / weightsSum),
                    weightedVectors.Sum(wv => wv.vector.Y * wv.weight / weightsSum)));
        }

        public static Vector AverageDirection(this IEnumerable<(ISegment current, ISegment nearest)> trackedPairs) {
            return trackedPairs
                .Select(sp => sp.current.Center.DistanceVectorTo(sp.current.Center.DistancePointTo(sp.nearest.A, sp.nearest.B)))
                .OrderByAngle().ApproximateOrdered();
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace ConvergenceEngine.Models.Mapping.Extensions {

    using Segments;
    using Infrastructure.Extensions;

    internal static class Determinator {

        public static NavigationInfo ComputeConvergence(this IEnumerable<(Segment current, Segment nearest)> trackedPairs,
            double maxDistancePercent, double maxAngleDegrees) {

            if (trackedPairs.IsNullOrEmpty()) {
                return new NavigationInfo(); // !
            }

            double resultAngle = AverageWeightedAngle(trackedPairs);

            trackedPairs = trackedPairs
                .Select(sp => (sp.current, sp.nearest.RotatedAtZero(-resultAngle)));

            var resultDirection = trackedPairs
                .Select(sp => sp.current.Center.DistanceVectorTo(sp.current.Center.DistancePointTo(sp.nearest.A, sp.nearest.B)))
                .OrderByAngle().ApproximateOrdered();

            return new NavigationInfo(resultDirection, resultAngle);
        }

        public static double AverageWeightedAngle(this IEnumerable<(Segment current, Segment nearest)> trackedPairs) {

            var vectorizedAngles = trackedPairs.Select(sp => sp.current.AngleTo(sp.nearest).DegreesToVector());

            var weigths = trackedPairs.Select(sp => sp.current.Length + sp.nearest.Length);
            double weightsSum = weigths.Sum();

            double weightedAverageX = vectorizedAngles.Sequential(weigths, (v, w) => v.X * w / weightsSum).Sum();
            double weightedAverageY = vectorizedAngles.Sequential(weigths, (v, w) => v.Y * w / weightsSum).Sum();

            return Vector.AngleBetween(Segment.BasisX, new Vector(weightedAverageX, weightedAverageY));
        }
    }
}
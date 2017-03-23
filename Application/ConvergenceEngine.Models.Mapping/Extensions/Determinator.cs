using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace ConvergenceEngine.Models.Mapping.Extensions {

    using Segments;
    using Infrastructure.Extensions;

    internal static class Determinator {

        public static NavigationInfo ComputeConvergence(this IEnumerable<(Segment Current, Segment Nearest)> trackedPairs,
            double maxDistancePercent, double maxAngleDegrees) {

            if (trackedPairs.IsNullOrEmpty()) {
                return new NavigationInfo();
            }

            double resultAngle = AverageWeightedAngle(trackedPairs);

            trackedPairs = trackedPairs
                .Select(sp => (sp.Current, sp.Nearest.RotatedAtZero(-resultAngle))); // !!

            Vector resultDirection = trackedPairs
                .Select(sp => sp.Current.Center.DistanceVectorTo(sp.Current.Center.DistancePointTo(sp.Nearest.A, sp.Nearest.B)))
                .OrderByAngle().ApproximateOrdered();            

            return new NavigationInfo(resultDirection, resultAngle);
        }

        public static double AverageWeightedAngle(this IEnumerable<(Segment Current, Segment Nearest)> trackedPairs) {

            var vectorizedAngles = trackedPairs.Select(sp => sp.Current.AngleTo(sp.Nearest).DegreesToVector());

            var weigths = trackedPairs.Select(sp => sp.Current.Length + sp.Nearest.Length);
            var weightsSum = weigths.Sum();

            var weightedAverageX = vectorizedAngles.Sequential(weigths, (v, w) => v.X * w / weightsSum).Sum();
            var weightedAverageY = vectorizedAngles.Sequential(weigths, (v, w) => v.Y * w / weightsSum).Sum();

            return Vector.AngleBetween(new Vector(1.0, 0.0), new Vector(weightedAverageX, weightedAverageY));
        }
    }
}
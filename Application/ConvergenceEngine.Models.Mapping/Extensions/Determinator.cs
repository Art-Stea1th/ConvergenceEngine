using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace ConvergenceEngine.Models.Mapping.Extensions {

    using Segments;
    using Infrastructure.Extensions;

    internal static class Determinator {

        public static NavigationInfo ComputeConvergence(this IEnumerable<Tuple<Segment, Segment>> trackedPairs,
            double maxDistancePercent, double maxAngleDegrees, double currentPositionX/* = 0.0*/, double currentPositionY/* = 0.0*/) {

            if (trackedPairs.IsNullOrEmpty()) {
                return new NavigationInfo();
            }

            double resultAngle = AverageWeightedAngle(trackedPairs);

            trackedPairs = trackedPairs
                .Select(sp => Tuple.Create(sp.Item1, sp.Item2.RotatedAt(-resultAngle, currentPositionX, currentPositionY))); // !!

            Vector resultDirection = trackedPairs
                .Select(sp => sp.Item1.Center.DistanceVectorTo(sp.Item1.Center.DistancePointTo(sp.Item2.A, sp.Item2.B)))
                .OrderByAngle().ApproximateOrdered();            

            return new NavigationInfo(resultDirection, resultAngle);
        }

        public static double AverageWeightedAngle(this IEnumerable<Tuple<Segment, Segment>> trackedPairs) {

            var vectorizedAngles = trackedPairs.Select(sp => sp.Item1.AngleTo(sp.Item2).DegreesToVector());

            var weigths = trackedPairs.Select(sp => sp.Item1.Length + sp.Item2.Length);
            var weightsSum = weigths.Sum();

            var weightedAverageX = vectorizedAngles.Sequential(weigths, (v, w) => v.X * w / weightsSum).Sum();
            var weightedAverageY = vectorizedAngles.Sequential(weigths, (v, w) => v.Y * w / weightsSum).Sum();

            return Vector.AngleBetween(new Vector(1.0, 0.0), new Vector(weightedAverageX, weightedAverageY));
        }
    }
}
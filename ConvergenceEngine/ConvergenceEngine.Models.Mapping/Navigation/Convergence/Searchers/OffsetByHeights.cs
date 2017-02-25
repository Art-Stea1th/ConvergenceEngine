using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace ConvergenceEngine.Models.Mapping.Navigation.Convergence.Searchers {

    using Extensions;
    using Segmentation;

    internal static class OffsetByHeights { // as static TMP

        public static Vector? SearchBetween(IReadOnlyList<Segment> current, IReadOnlyList<Segment> another) {

            if (current.IsNullOrEmpty() || another.IsNullOrEmpty()) {
                return null;
            }
            if (current.Count != another.Count) {
                throw new ArgumentOutOfRangeException();
            }
            var heights = current.DoSequential(another, (c, a) => c.MiddlePoint().ConvergenceTo(c.MiddlePoint().DistancePointTo(a.PointA, a.PointB)));
            return ApproximateDirections(heights);
        }

        private static Vector ApproximateDirections(IEnumerable<Vector> directions) {

            if (directions.IsNull()) {
                throw new ArgumentNullException();
            }
            if (directions.IsEmpty()) {
                throw new ArgumentOutOfRangeException();
            }
            switch (directions.Count()) {
                case 1: return directions.First();
                case 2: return (directions.First() + directions.Last()) * 0.5;
            }

            Vector p0 = directions.First(), pN = directions.Last();

            double avgX = directions.Average(p => p.X);
            double avgY = directions.Average(p => p.Y);
            double avgXY = directions.Average(p => p.X * p.Y);
            double avgSqX = directions.Average(p => Math.Pow(p.X, 2));
            double sqAvgX = Math.Pow(avgX, 2);

            double A = (avgXY - avgX * avgY) / (avgSqX - sqAvgX);
            double B = avgY - A * avgX;

            return (new Vector(p0.X, A * p0.X + B) + new Vector(pN.X, A * pN.X + B)) * 0.5;
        }
    }
}
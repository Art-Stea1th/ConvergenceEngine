using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace ConvergenceEngine.Models.Mapping.Navigation.Convergence.Searchers {

    using Extensions;
    using Segmentation;

    internal static class OffsetByIntersectionPoints { // as static TMP

        public static Vector SearchBetween(IReadOnlyList<Segment> current, IReadOnlyList<Segment> another) { // quick impl.

            //var lehgths = current.DoSequential(another, (c, a) => (c.Length + a.Length) * 0.5);

            if (current.Count != another.Count) {
                throw new ArgumentOutOfRangeException();
            }

            var resultDirections = new List<Vector>();

            for (int i = 0; i < current.Count - 1; ++i) {
                for (int j = i + 1; j < current.Count; ++j) {

                    var currentDirection = Direction(current[i], current[j], another[i], another[j], limit: 3.0);
                    if (currentDirection != null) {
                        resultDirections.Add(currentDirection.Value);
                    }
                }
            }

            if (resultDirections.IsEmpty()) {
                return new Vector(0.0, 0.0);
            }
            return resultDirections.Sum() / resultDirections.Count; // TMP avg., will be weighted by lengths
        }

        private static Vector? Direction(Segment currentA, Segment currentB, Segment anotherA, Segment anotherB, double limit) {

            //var absAngle = Math.Abs(Segment.AngleBetween(currentA, currentB));

            //if (absAngle < 45.0 || absAngle > 135.0) {
            //    return null;
            //}
            Point? intersectionA = currentA.IntersectionPointWith(currentB);
            if (intersectionA == null) {
                return null;
            }
            Point? intersectionB = anotherA.IntersectionPointWith(anotherB);
            if (intersectionB == null) {
                return null;
            }

            var resultDirection = intersectionA.Value.ConvergenceTo(intersectionB.Value);
            if (Math.Abs(resultDirection.X) > limit || Math.Abs(resultDirection.Y) > limit) {
                return null;
            }
            return resultDirection;
        }

        private static bool IsValidIntersectionPoint(Point point) {

            throw new NotImplementedException();
        }

        private static Vector AverageWeightedByLengthsDirection(IEnumerable<double> lengths, IEnumerable<Vector> directions) {

            var fullLength = lengths.Sum();
            var weights = lengths.Select(s => s * 100.0 / fullLength);

            return directions.DoSequential(weights, (d, w) => d / 100.0 * w).Sum();
        }
    }
}
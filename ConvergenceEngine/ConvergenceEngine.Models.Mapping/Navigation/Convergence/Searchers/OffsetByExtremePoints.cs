using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;

namespace ConvergenceEngine.Models.Mapping.Navigation.Convergence.Searchers {

    using Extensions;
    using Segmentation;

    internal static class OffsetByExtremePoints { // as static TMP

        // ---------- hardcode trapeze limits --------->
        private static double maxY = 395.0, minY = 81.0;
        private static double maxX = 210, minX = -maxX;
        // <--------- hardcode trapeze limits ----------

        public static Vector? SearchBetween(IReadOnlyList<Segment> current, IReadOnlyList<Segment> another) {

            if (current.IsNullOrEmpty() || another.IsNullOrEmpty()) {
                return null;
            }
            if (current.Count != another.Count) {
                throw new ArgumentOutOfRangeException();
            }

            var pairs = GetValidPointsPairs(current, another);
            if (pairs.IsNullOrEmpty()) {
                return null;
            }

            var lehgths = current.DoSequential(another, (c, a) => (c.Length + a.Length) * 0.5);
            var directions = pairs.Select(sp => sp.Item1.ConvergenceTo(sp.Item2));

            return AverageWeightedByLehgthsDirection(lehgths, directions);
        }

        private static IEnumerable<Tuple<Point, Point>> GetValidPointsPairs(IReadOnlyList<Segment> current, IReadOnlyList<Segment> another) {

            var currentEnumerator = current.GetEnumerator();
            var anotherEnumerator = another.GetEnumerator();

            while (currentEnumerator.MoveNext() && anotherEnumerator.MoveNext()) {
                var validPair = GetValidPointsPair(currentEnumerator.Current, anotherEnumerator.Current);
                if (validPair != null) {
                    yield return validPair;
                }
            }
        }

        private static Tuple<Point, Point> GetValidPointsPair(Segment current, Segment another) {

            var pointAPair = GetNearest(current.PointA, another.PointA, another.PointB);
            var pointBPair = GetNearest(current.PointB, another.PointB, another.PointA);

            var pairA = new Tuple<Point, Point>(current.PointA, pointAPair);
            var pairB = new Tuple<Point, Point>(current.PointB, pointBPair);

            bool isValidA = IsValidPair(pairA);
            bool isValidB = IsValidPair(pairB);

            if (isValidA && isValidB) {
                return pairA.Item1.DistanceTo(pairA.Item2) < pairB.Item1.DistanceTo(pairB.Item2) ? pairA : pairB; // m.b. avg. ?
            }
            if (isValidA) {
                return pairA;
            }
            if (isValidB) {
                return pairB;
            }
            return null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Point GetNearest(Point current, Point anotherA, Point anotherB) {
            return current.DistanceTo(anotherA) < current.DistanceTo(anotherB) ? anotherA : anotherB;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsValidPair(Tuple<Point, Point> points) {
            return IsValidPoint(points.Item1) && IsValidPoint(points.Item2) ? true : false;
        }

        private static bool IsValidPoint(Point point) {
            if (point.Y >= maxY || point.Y <= minY) {         // point above the maximum or below the minimum Y
                return false;
            }
            var current = (Vector)point;

            var left = new Vector(minX, maxY);
            if (Vector.AngleBetween(current, left) <= 0.0) {  // point to the left of the left-limiting vector
                return false;
            }
            var right = new Vector(maxX, maxY);
            if (Vector.AngleBetween(current, right) >= 0.0) { // point to the right of the right-limiting vector
                return false;
            }
            return true;
        }

        private static Vector AverageWeightedByLehgthsDirection(IEnumerable<double> lengths, IEnumerable<Vector> directions) {

            var fullLength = lengths.Sum();
            var weights = lengths.Select(s => s * 100.0 / fullLength);

            return directions.DoSequential(weights, (d, w) => d / 100.0 * w).Sum();
        }
    }
}
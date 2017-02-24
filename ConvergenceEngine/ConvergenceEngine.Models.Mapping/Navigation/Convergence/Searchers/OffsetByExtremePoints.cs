using System;
using System.Windows;

namespace ConvergenceEngine.Models.Mapping.Navigation.Convergence.Searchers {

    using Extensions;
    using Segmentation;
    using System.Runtime.CompilerServices;

    internal static class OffsetByExtremePoints { // as static TMP

        // ---------- hardcode trapeze limits --------->
        private static double maxY = 395.0, minY = 81.0;
        private static double maxX = 210, minX = -maxX;
        // <--------- hardcode trapeze limits ----------

        public static Vector SearchBetween(Segment current, Segment another) {

            var pair = GetValidPointsPair(current, another);
            if (pair != null) {
                return pair.Item1.ConvergenceTo(pair.Item2);
            }
            var currentMiddle = new Point((current.PointA.X + current.PointB.X) * 0.5, (current.PointA.Y + current.PointB.Y) * 0.5);
            return currentMiddle.ConvergenceTo(currentMiddle.DistancePointTo(another.PointA, another.PointB));
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
    }
}
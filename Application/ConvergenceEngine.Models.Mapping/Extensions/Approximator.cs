using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;

namespace ConvergenceEngine.Models.Mapping.Extensions {

    using Infrastructure.Extensions;

    internal static class Approximator { // IEnumerable<Point>, IEnumerable<Vector> Extension class

        public static IEnumerable<Point> ThinOutSorted(this IEnumerable<Point> points, double maxDistance = 1.0) {

            var first = points.First();
            var last = points.Last();

            if (first.DistanceTo(last) <= maxDistance) {
                yield return first;
                yield return last;
                yield break;
            }

            var prev = first;
            foreach (var next in points.Skip(1)) {
                if (next.DistanceTo(prev) >= maxDistance) {
                    yield return next;
                    prev = next;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Tuple<Point, Point> ApproximateOrdered(this IEnumerable<Point> points) {
            return Approximate(points);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector ApproximateOrdered(this IEnumerable<Vector> vectors) {
            var result = Approximate(vectors.Select(v => (Point)v));
            return ((Vector)result.Item1 + (Vector)result.Item2) * 0.5;
        }

        private static Tuple<Point, Point> Approximate(IEnumerable<Point> points) {
            if (points == null) {
                throw new ArgumentNullException(points.ToString());
            }
            switch (points.Count()) {
                case 0: throw new ArgumentOutOfRangeException(points.ToString());
                case 1: return Tuple.Create(points.First(), points.First());
                case 2: return Tuple.Create(points.First(), points.Last());
            }

            var p0 = points.First();
            var pN = points.Last();

            var avgX = points.Average(p => p.X);
            var avgY = points.Average(p => p.Y);
            var avgXY = points.Average(p => p.X * p.Y);
            var avgSqX = points.Average(p => Math.Pow(p.X, 2));
            var sqAvgX = Math.Pow(avgX, 2);

            var a = (avgXY - avgX * avgY) / (avgSqX - sqAvgX);
            var b = avgY - a * avgX;

            if (double.IsNaN(a) || double.IsInfinity(a)) {
                return Tuple.Create(p0, pN);
            }

            var olsP0 = new Point(p0.X, a * p0.X + b);
            var olsPN = new Point(pN.X, a * pN.X + b);

            // Trim Y
            var resultP0 = new Point(p0.X, p0.DistancePointTo(olsP0, olsPN).Y);
            var resultPN = new Point(pN.X, pN.DistancePointTo(olsP0, olsPN).Y);

            return Tuple.Create(resultP0, resultPN);
        }
    }
}
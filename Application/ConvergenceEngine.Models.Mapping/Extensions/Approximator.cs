using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;

namespace ConvergenceEngine.Models.Mapping.Extensions {

    using Infrastructure.Extensions;

    internal static class Approximator { // IEnumerable<Point>, IEnumerable<Vector> Extension class

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Tuple<Point, Point> ApproximateOrdered(this IEnumerable<Point> points) {
            var result = points.Approximate(p => p.X, p => p.Y);
            return Tuple.Create(new Point(result.Item1, result.Item2), new Point(result.Item3, result.Item4));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector ApproximateOrdered(this IEnumerable<Vector> vectors) {
            var result = vectors.Approximate(v => v.X, v => v.Y);
            return (new Vector(result.Item1, result.Item2) + new Vector(result.Item3, result.Item4)) * 0.5;
        }

        // Ordinary Least Squares
        private static Tuple<double, double, double, double> Approximate<TSource>(this IEnumerable<TSource> sequence,
            Func<TSource, double> x, Func<TSource, double> y) {
            if (sequence == null || x == null || y == null) {
                throw new ArgumentNullException();
            }
            int count = sequence.Count();
            if (count == 0) {
                throw new ArgumentOutOfRangeException(sequence.ToString());
            }
            var first = sequence.First();
            if (count == 1) {
                return Tuple.Create(x(first), y(first), x(first), y(first));
            }
            var last = sequence.Last();
            if (count == 2) {
                return Tuple.Create(x(first), y(first), x(last), y(last));
            }

            var avgX = sequence.Average(p => x(p));
            var avgY = sequence.Average(p => y(p));
            var avgXY = sequence.Average(p => x(p) * y(p));
            var avgSqX = sequence.Average(p => Math.Pow(x(p), 2));
            var sqAvgX = Math.Pow(avgX, 2);

            var a = (avgXY - avgX * avgY) / (avgSqX - sqAvgX);
            var b = avgY - a * avgX;

            if (double.IsNaN(a) || double.IsInfinity(a)) {
                return Tuple.Create(x(first), y(first), x(last), y(last));
            }

            var p0 = new Point(x(first), y(first));
            var pN = new Point(x(last), y(last));
            var olsP0 = new Point(p0.X, a * p0.X + b);
            var olsPN = new Point(pN.X, a * pN.X + b);

            // Trim Y
            var resultP0 = new Point(p0.X, p0.DistancePointTo(olsP0, olsPN).Y);
            var resultPN = new Point(pN.X, pN.DistancePointTo(olsP0, olsPN).Y);

            return Tuple.Create(resultP0.X, resultP0.Y, resultPN.X, resultPN.Y);
        }
    }
}
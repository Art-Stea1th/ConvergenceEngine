using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace ConvergenceEngine.Models.Mapping.Extensions {

    using Infrastructure.Extensions;

    internal static class Approximator { // IEnumerable<Point>, IEnumerable<Vector> Extension class

        public static (Point a, Point b) ApproximateOrdered(this IEnumerable<Point> points) {
            var s = points.Approximate(p => p.X, p => p.Y);
            return (a: new Point(s.x1, s.y1), b: new Point(s.x2, s.y2));
        }

        public static Vector ApproximateOrdered(this IEnumerable<Vector> vectors) {
            var s = vectors.Approximate(v => v.X, v => v.Y);
            return (new Vector(s.x1, s.y1) + new Vector(s.x2, s.y2)) * 0.5;
        }

        // Ordinary Least Squares
        private static (double x1, double y1, double x2, double y2) Approximate<TSource>(
            this IEnumerable<TSource> sequence, Func<TSource, double> x, Func<TSource, double> y) {
            if (sequence == null || x == null || y == null) {
                throw new ArgumentNullException();
            }
            int count = sequence.Count();
            if (count == 0) {
                throw new ArgumentOutOfRangeException(sequence.ToString());
            }
            var first = sequence.First();
            if (count == 1) {
                return (x1: x(first), y1: y(first), x2: x(first), y2: y(first));
            }
            var last = sequence.Last();
            if (count == 2) {
                return (x1: x(first), y1: y(first), x2: x(last), y2: y(last));
            }

            double avgX = sequence.Average(p => x(p));
            double avgY = sequence.Average(p => y(p));
            double avgXY = sequence.Average(p => x(p) * y(p));
            double avgSqX = sequence.Average(p => Math.Pow(x(p), 2));
            double sqAvgX = Math.Pow(avgX, 2);

            double a = (avgXY - avgX * avgY) / (avgSqX - sqAvgX);
            double b = avgY - a * avgX;

            if (double.IsNaN(a) || double.IsInfinity(a)) {
                return (x1: x(first), y1: y(first), x2: x(last), y2: y(last));
            }

            var p0 = new Point(x(first), y(first));
            var pN = new Point(x(last), y(last));
            var olsP0 = new Point(p0.X, a * p0.X + b);
            var olsPN = new Point(pN.X, a * pN.X + b);

            // Trim Y
            var resultP0 = new Point(p0.X, p0.DistancePointTo(olsP0, olsPN).Y);
            var resultPN = new Point(pN.X, pN.DistancePointTo(olsP0, olsPN).Y);

            return (x1: resultP0.X, y1: resultP0.Y, x2: resultPN.X, y2: resultPN.Y);
        }
    }
}
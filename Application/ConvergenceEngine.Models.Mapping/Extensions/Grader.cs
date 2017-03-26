using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace ConvergenceEngine.Models.Mapping.Extensions {

    using Infrastructure.Extensions;

    internal static class Grader { // IEnumerable<Point>, IEnumerable<Vector> Extension class

        public static readonly Vector BasisX = new Vector(1.0, 0.0);
        public static readonly Vector BasisY = new Vector(0.0, 1.0);

        public static IEnumerable<Vector> OrderByAngle(this IEnumerable<Vector> vectors) {
            return vectors.OrderBy(v => Vector.AngleBetween(v, BasisY));
        }

        public static IEnumerable<Point> OrderByLine(this IEnumerable<Point> points, Point pointA, Point pointB) {
            return points.OrderByVector(pointB - pointA);
        }

        public static IEnumerable<Point> OrderByVector(this IEnumerable<Point> points, Vector vector) {
            double angle = Vector.AngleBetween(vector, BasisX);
            return points
                .Select(p => (source: p, rotated: p.Rotated(angle)))
                .OrderBy(sr => sr.rotated.X)
                .Select(sr => sr.source);
        }
    }
}
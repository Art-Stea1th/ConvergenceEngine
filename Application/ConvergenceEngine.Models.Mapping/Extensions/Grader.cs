using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;

namespace ConvergenceEngine.Models.Mapping.Extensions {

    using Infrastructure.Extensions;

    internal static class Grader { // IEnumerable<Point>, IEnumerable<Vector> Extension class

        public static readonly Vector BasisX = new Vector(1.0, 0.0);
        public static readonly Vector BasisY = new Vector(0.0, 1.0);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<Vector> OrderByAngle(this IEnumerable<Vector> vectors) {
            return vectors.OrderBy(v => Vector.AngleBetween(v, BasisY));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<Point> OrderByLine(this IEnumerable<Point> points, Point pointA, Point pointB) {
            return points.OrderByVector(pointB - pointA);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<Point> OrderByVector(this IEnumerable<Point> points, Vector vector) {
            var angle = Vector.AngleBetween(vector, BasisX);
            var pairsWithRotated = points.Select(p => new { Original = p, Rotated = p.Rotated(angle) });
            return pairsWithRotated.OrderBy(pp => pp.Rotated.X).Select(pp => pp.Original);
        }
    }
}
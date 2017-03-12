using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace ConvergenceEngine.Models.Mapping.Extensions {

    using Infrastructure.Extensions;
    using Infrastructure.Interfaces;
    using Segments;

    internal static class Approximator { // IEnumerable<Point>, IEnumerable<Vector> Extension class

        public static IEnumerable<Point> ThinOutSorted(this IEnumerable<Point> points, double maxDistance = 1.0) {

            var enumerator = points.GetEnumerator();
            var previous = enumerator.Current;
            yield return previous;

            while (enumerator.MoveNext()) {
                var current = enumerator.Current;
                if (current.DistanceTo(previous) < maxDistance) {
                    continue;
                }
                yield return current;
                previous = current;
            }
        }

        public static MultiPointSegment MergedWith(this ISegment current, ISegment another) { // ?

            ISegment primary, secondary;

            if (current.Length > another.Length) {
                primary = current; secondary = another;
            }
            else {
                primary = another; secondary = current;
            }

            var angle = Segment.AngleBetween(secondary, primary);
            secondary = new MultiPointSegment(secondary.Select(p => p.RotatedAt(angle, secondary.CenterPoint.X, secondary.CenterPoint.Y)));

            var direction = secondary.CenterPoint.ConvergenceTo(secondary.CenterPoint.DistancePointTo(primary.PointA, primary.PointB));
            secondary.ApplyTransform(direction.X, direction.Y, 0);

            var resultPoints = new List<ISegment> { primary, secondary }.SelectMany(p => p).OrderByLine(primary.PointA, primary.PointB);
            return new MultiPointSegment(resultPoints);
        }

        public static Tuple<Point, Point> ApproximateSorted(this IEnumerable<Point> points) {

            Point p0 = points.First(), pN = points.Last();

            double avgX = points.Average(p => p.X);
            double avgY = points.Average(p => p.Y);
            double avgXY = points.Average(p => p.X * p.Y);
            double avgSqX = points.Average(p => Math.Pow(p.X, 2));
            double sqAvgX = Math.Pow(avgX, 2);

            double A = (avgXY - avgX * avgY) / (avgSqX - sqAvgX);
            double B = avgY - A * avgX;

            Point olsP0 = new Point(p0.X, A * p0.X + B), olsPN = new Point(pN.X, A * pN.X + B);

            // Trim Y
            Point resultP0 = new Point(p0.X, p0.DistancePointTo(olsP0, olsPN).Y);
            Point resultPN = new Point(pN.X, pN.DistancePointTo(olsP0, olsPN).Y);

            return new Tuple<Point, Point>(resultP0, resultPN);
        }

        public static Vector ApproximateSorted(this IEnumerable<Vector> directions) {

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
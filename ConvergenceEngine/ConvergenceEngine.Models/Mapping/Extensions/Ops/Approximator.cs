using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace ConvergenceEngine.Models.Mapping.Extensions.Ops {

    using Iterable;

    internal static class Approximator { // IEnumerable<Point>, IEnumerable<Vector> Extension class

        public static Tuple<Point, Point> Approximate(this IEnumerable<Point> sequence) {

            Point p0 = sequence.First(), pN = sequence.Last();

            double avgX = sequence.Average(p => p.X);
            double avgY = sequence.Average(p => p.Y);
            double avgXY = sequence.Average(p => p.X * p.Y);
            double avgSqX = sequence.Average(p => Math.Pow(p.X, 2));
            double sqAvgX = Math.Pow(avgX, 2);

            double A = (avgXY - avgX * avgY) / (avgSqX - sqAvgX);
            double B = avgY - A * avgX;

            Point olsP0 = new Point(p0.X, A * p0.X + B), olsPN = new Point(pN.X, A * pN.X + B);

            // Trim Y
            Point resultP0 = new Point(p0.X, p0.DistancePointTo(olsP0, olsPN).Y);
            Point resultPN = new Point(pN.X, pN.DistancePointTo(olsP0, olsPN).Y);

            return new Tuple<Point, Point>(resultP0, resultPN);
        }

        public static Vector Approximate(this IEnumerable<Vector> directions) {

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
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;

namespace SLAM.Models.Mapping.Navigation {

    using Extensions;

    internal sealed class Segment : IEnumerable<Point> {

        public Point PointA { get; private set; }
        public Point PointB { get; private set; }

        public double Length { get { return (PointA - PointB).Length; } }

        public Segment Rotate(double angle) {
            return new Segment(PointA.Rotate(angle), PointB.Rotate(angle));
        }

        public Segment RotateAt(Point point, double angle) {
            return new Segment(PointA.RotateAt(point, angle), PointB.RotateAt(point, angle));
        }

        public Vector ConvergenceToNearestPoint(Segment segment) { // <-- :\

            Vector result = PointA.ConvergenceTo(segment.PointA);

            var comparable = PointA.ConvergenceTo(segment.PointB);
            if (comparable.Length < result.Length) {
                result = comparable;
            }
            comparable = PointB.ConvergenceTo(segment.PointA);
            if (comparable.Length < result.Length) {
                result = comparable;
            }
            comparable = PointB.ConvergenceTo(segment.PointB);
            if (comparable.Length < result.Length) {
                result = comparable;
            }
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double DistanceToNearestPoint(Segment segment) {
            return new[] {
                PointA.DistanceTo(segment.PointA),
                PointA.DistanceTo(segment.PointB),
                PointB.DistanceTo(segment.PointA),
                PointB.DistanceTo(segment.PointB)
            }.Min();
        }

        public static double AngleBetween(Segment segmentA, Segment segmentB) {
            return Vector.AngleBetween((segmentA.PointB - segmentA.PointA), (segmentB.PointB - segmentB.PointA));
        }

        public static implicit operator Segment(Tuple<Point, Point> poinsPair) {
            return new Segment(poinsPair);
        }

        public static explicit operator Tuple<Point, Point>(Segment segment) {
            return new Tuple<Point, Point>(segment.PointA, segment.PointB);
        }

        public static Segment CreateByFirstAndLastFrom(ICollection<Point> sequence) {
            return new Segment(sequence.First(), sequence.Last());
        }

        public static Segment ApproximateFrom(ICollection<Point> sequence) {

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

            return new Segment(resultP0, resultPN);
        }

        public Segment(Point pointA, Point pointB) {
            PointA = pointA;
            PointB = pointB;
        }

        public Segment(Tuple<Point, Point> pointsPair) {
            PointA = pointsPair.Item1;
            PointB = pointsPair.Item2;
        }

        #region IEnumerable<Point>
        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        public IEnumerator<Point> GetEnumerator() {
            return AllPoints().GetEnumerator();
        }

        private IEnumerable<Point> AllPoints() {
            yield return PointA;
            yield return PointB;
        }
        #endregion
    }
}
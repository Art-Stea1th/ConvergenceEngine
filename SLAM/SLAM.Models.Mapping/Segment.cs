using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;

namespace SLAM.Models.Mapping {

    internal sealed class Segment : IEnumerable<Point> {

        public Point PointA { get; }
        public Point PointB { get; }
        public Point Average { get; }

        public double Length { get { return (PointA - PointB).Length; } }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<Segment> FindSegmentsWithNearestPoints(IEnumerable<Segment> sequence, double pointsDistanceLimit) {
            return sequence.Where(s => DistanceBetweenNearestPoints(this, s) < pointsDistanceLimit);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<Segment> FindSegmentsWithMinimalAngle(IEnumerable<Segment> sequence, double angleLimit) {
            return sequence.Where(s => AngleBetween(this, s) < angleLimit);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double DistanceBetweenNearestPoints(Segment segmentA, Segment segmentB) {
            return new[] {
                segmentA.PointA.DistanceTo(segmentB.PointA),
                segmentA.PointA.DistanceTo(segmentB.PointB),
                segmentA.PointB.DistanceTo(segmentB.PointA),
                segmentA.PointB.DistanceTo(segmentB.PointB)
            }.Min();
        }

        public static double AngleBetween(Segment segmentA, Segment segmentB) {
            double angle = Vector.AngleBetween((segmentA.PointA - segmentA.PointB), (segmentB.PointA - segmentB.PointB));
            return angle <= 180.0 ? angle : 360.0 - angle;
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

            return new Segment(new Point(p0.X, A * p0.X + B), new Point(pN.X, A * pN.X + B));
        }

        public Segment(Tuple<Point, Point> pointsPair) {
            PointA = pointsPair.Item1;
            PointB = pointsPair.Item2;
            Average = new Point((PointA.X + PointB.X) * 0.5, (PointA.Y + PointB.Y) * 0.5);
        }

        public Segment(Point pointA, Point pointB) {
            PointA = pointA;
            PointB = pointB;
            Average = new Point((PointA.X + PointB.X) * 0.5, (PointA.Y + PointB.Y) * 0.5);
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
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;

namespace ConvergenceEngine.Models.Mapping.Segments {

    using Extensions;

    public class Segment : ISegment {

        public virtual Point PointA { get; protected set; }
        public virtual Point PointB { get; protected set; }
        public Point CenterPoint { get { return new Point((PointA.X + PointB.X) * 0.5, (PointA.Y + PointB.Y) * 0.5); } }
        public double Length { get { return (PointA - PointB).Length; } }

        internal Segment(Tuple<Point, Point> points) : this(points.Item1, points.Item2) { }
        internal Segment(Point pointA, Point pointB) {
            PointA = pointA; PointB = pointB;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static double AngleBetween(ISegment segmentA, ISegment segmentB) {
            return Vector.AngleBetween((segmentA.PointB - segmentA.PointA), (segmentB.PointB - segmentB.PointA));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator Tuple<Point, Point>(Segment segment) {
            return new Tuple<Point, Point>(segment.PointA, segment.PointB);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double DistanceToNearestPoint(ISegment segment) {
            return new[] {
                PointA.DistanceTo(segment.PointA),
                PointA.DistanceTo(segment.PointB),
                PointB.DistanceTo(segment.PointA),
                PointB.DistanceTo(segment.PointB)
            }.Min();
        }

        internal Point? IntersectWith(Segment segment) {

            var lineCoefficients1 = CalculateLineCoefficients();
            var lineCoefficients2 = segment.CalculateLineCoefficients();

            double A1 = lineCoefficients1.Item1;
            double B1 = lineCoefficients1.Item2;
            double C1 = lineCoefficients1.Item3;

            double A2 = lineCoefficients2.Item1;
            double B2 = lineCoefficients2.Item2;
            double C2 = lineCoefficients2.Item3;

            double commonDenominator = A1 * B2 - A2 * B1;

            if (commonDenominator == 0.0) {
                return null;
            }

            double resultX = -(C1 * B2 - C2 * B1) / commonDenominator;
            double resultY = -(A1 * C2 - A2 * C1) / commonDenominator;

            return new Point(resultX, resultY);
        }

        internal Tuple<double, double, double> CalculateLineCoefficients() { // Ax + By + C
            double A = PointA.Y - PointB.Y;
            double B = PointB.X - PointA.X;
            double C = -(A * PointA.X) - (B * PointA.Y);
            return new Tuple<double, double, double>(A, B, C);
        }

        private IEnumerable<Point> Points() {
            yield return PointA;
            yield return PointB;
        }

        #region Generic Interfaces

        public virtual int Count { get { return 2; } }
        public virtual Point this[int index] {
            get {
                switch (index) {
                    case 0: return PointA;
                    case 1: return PointB;
                    default: throw new IndexOutOfRangeException();
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual IEnumerator<Point> GetEnumerator() {
            return Points().GetEnumerator();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
        #endregion
    }
}
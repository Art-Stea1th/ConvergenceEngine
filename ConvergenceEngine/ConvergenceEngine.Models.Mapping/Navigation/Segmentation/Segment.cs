using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;

namespace ConvergenceEngine.Models.Mapping.Navigation.Segmentation {

    using Extensions;

    internal sealed class Segment : ReadOnlyPointSequence {

        private readonly Lazy<Tuple<Point, Point>> approximated;
        private readonly Lazy<int> maxDivergencePointIndex;

        internal Point PointA { get { return approximated.Value.Item1; } }
        internal Point PointB { get { return approximated.Value.Item2; } }

        public double Length { get { return (PointA - PointB).Length; } }

        internal double MaxDivergence { get { return MaxDivergencePoint.DistanceTo(Points.First(), Points.Last()); } }
        internal Point MaxDivergencePoint { get { return Points[MaxDivergencePointIndex]; } }
        internal int MaxDivergencePointIndex { get { return maxDivergencePointIndex.Value; } }

        internal Segment(IEnumerable<Point> points) : base(points) {
            approximated = new Lazy<Tuple<Point, Point>>(() => Approximate(this));
            maxDivergencePointIndex = new Lazy<int>(() => FindMaxDivergencePointIndex(this));
        }

        public static double AngleBetween(Segment segmentA, Segment segmentB) { // TMP
            return Vector.AngleBetween((segmentA.PointB - segmentA.PointA), (segmentB.PointB - segmentB.PointA));
        }

        public static explicit operator Tuple<Point, Point>(Segment segment) {
            return new Tuple<Point, Point>(segment.PointA, segment.PointB);
        }

        public Point? IntersectionPointWith(Segment segment) {

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

        private Tuple<double, double, double> CalculateLineCoefficients() { // Ax + By + C
            double A = PointA.Y - PointB.Y;
            double B = PointB.X - PointA.X;
            double C = -(A * PointA.X) - (B * PointA.Y);
            return new Tuple<double, double, double>(A, B, C);
        }

        public Vector ConvergenceToNearestPoint(Segment segment) {

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

        internal int FindMaxDivergencePointIndex(IReadOnlyList<Point> sequence) {

            var maxDistance = 0.0;
            var maxDistancePointIndex = 0;

            for (int i = 0; i < sequence.Count; ++i) {
                double currentDistance = sequence[i].DistanceTo(sequence.First(), sequence.Last());
                if (currentDistance > maxDistance) {
                    maxDistance = currentDistance;
                    maxDistancePointIndex = i;
                }
            }
            return maxDistancePointIndex;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;

namespace SLAM.Models.Mapping.Navigation {

    using Extensions;

    internal sealed class SegmentSequence : List<Segment> {

        public SegmentSequence(IEnumerable<Segment> sequence) : base(sequence) { }
        public SegmentSequence(int capacity) : base(capacity) { }
        public SegmentSequence() { }

        public NavigationInfo GetDifferenceTo(SegmentSequence sequence) {

            var similar = FindSimilarSegmentsTo(sequence);

            var lehgths = similar.Select(sp => (sp.Item1.Length + sp.Item2.Length) / 2);
            var angles = similar.Select(sp => Segment.AngleBetween(sp.Item1, sp.Item2));

            double resultAngle = AverageWeightedByLengthsAngle(lehgths, angles);

            Matrix m = new Matrix();
            m.Rotate(resultAngle);

            similar = similar.Select(sp => new Tuple<Segment, Segment>(sp.Item1, new Segment(m.Transform(sp.Item2.PointA), m.Transform(sp.Item2.PointB))));

            var directions = similar.Select(sp => sp.Item2.ConvergenceToNearestPoint(sp.Item1));
            Vector resultDirection = AverageWeightedByLengthsDirection(lehgths, directions);

            NavigationInfo result = new NavigationInfo(resultDirection, resultAngle);

            return result;
        }

        private double AverageWeightedByLengthsAngle(IEnumerable<double> lengths, IEnumerable<double> angles) {

            var fullLength = lengths.Sum();
            var weights = lengths.Select(s => s * 100 / fullLength);

            return angles.DoSequential(weights, (a, w) => a / 100 * w).Sum();
        }

        private Vector AverageWeightedByLengthsDirection(IEnumerable<double> lengths, IEnumerable<Vector> directions) {

            var fullLength = lengths.Sum();
            var weights = lengths.Select(s => s * 100 / fullLength);

            return directions.DoSequential(weights, (d, w) => d / 100.0 * w).Sum();
        }

        public IEnumerable<Tuple<Segment, Segment>> FindSimilarSegmentsTo(SegmentSequence sequence) {

            foreach (var segment in this) {

                var maxDistance = Math.Min(segment.PointA.Y, segment.PointB.Y) * 0.05;
                var maxAngle = 3.0;

                Segment similar = sequence.FindSimilarSegmentFor(segment, maxDistance, maxAngle);
                if (similar != null) {
                    yield return new Tuple<Segment, Segment>(segment, similar);
                }
            }
        }        

        public Segment FindSimilarSegmentFor(Segment segment, double maxDistance, double maxAngle) {
            if (segment != null) {
                var selection = FindSegmentsByDistanceTo(segment, maxDistance).Intersect(FindSegmentsByAngleTo(segment, maxAngle));
                if (selection.Count() > 1) {
                    return new SegmentSequence(selection).FindSegmentWithNearestLengthTo(segment);
                }
                return selection.FirstOrDefault();
            }
            throw new ArgumentNullException(segment.ToString());
        }

        public Segment FindSegmentWithNearestLengthTo(Segment segment) {
            if (segment != null) {
                var minDifference = this.Min(s => Math.Abs(segment.Length - s.Length));
                return this.Where(sg => Math.Abs(segment.Length - sg.Length) == minDifference).FirstOrDefault();
            }
            throw new ArgumentNullException(segment.ToString());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<Segment> FindSegmentsByAngleTo(Segment segment, double maxAngle) {
            if (segment != null) {
                return this.Where(s => Math.Abs(Segment.AngleBetween(segment, s)) < maxAngle);
            }
            throw new ArgumentNullException(segment.ToString());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<Segment> FindSegmentsByDistanceTo(Segment segment, double maxDistance) {
            if (segment != null) {
                return this.Where(s => segment.DistanceToNearestPoint(s) < maxDistance);
            }
            throw new ArgumentNullException(segment.ToString());
        }

        #region Segmentate
        public static SegmentSequence Segmentate(IList<Point> sequence) {

            SegmentSequence result = new SegmentSequence();

            if (!sequence.IsNullOrEmpty()) {
                var pair = SplitByMaxPoint(sequence);

                if (pair == null) {
                    result.Add(Segment.ApproximateFrom(sequence));
                    //result.Add(Segment.CreateByFirstAndLastFrom(sequence));
                }
                else {
                    result.AddRange(Segmentate(pair.Item1));
                    result.AddRange(Segmentate(pair.Item2));
                }
            }
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsValidSequence(IList<Point> sequence) {
            var averageDistanceBetweenPoints = sequence.First().DistanceTo(sequence.Last()) / (sequence.Count - 1);
            return averageDistanceBetweenPoints <= ExpectedDistanceBetweenPoints(sequence.Average(p => p.Y));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double ExpectedDistanceBetweenPoints(double segmentPositionY) {
            double a = 100.0 * (2.0 / 3.0), b = a; // magic coefficients // a=b = 100.0 * (2.0 / 3.0);
            return (segmentPositionY - b) / a;     // y = ax + b; => x = (y - b) / a;
        }

        private static Tuple<IList<Point>, IList<Point>> SplitByMaxPoint(IList<Point> sequence) {

            if (sequence.Count >= 4) {
                var maxDistancePointIndex = FindIndexOfMaxDistancePoint(sequence);

                if (!AllowedPointDistanceToLine(sequence[maxDistancePointIndex], sequence.First(), sequence.Last())) {
                    var left = sequence.TakeWhile((p, i) => i <= maxDistancePointIndex).ToList();
                    var right = sequence.SkipWhile((p, i) => i < maxDistancePointIndex).ToList();
                    return new Tuple<IList<Point>, IList<Point>>(left, right);
                }
            }
            return null;
        }

        private static bool AllowedPointDistanceToLine(Point point, Point lineStart, Point lineEnd) {
            var distance = point.DistanceTo(lineStart, lineEnd);
            var averagegPositionY = (point.Y + lineStart.Y + lineEnd.Y) / 3;
            return distance < averagegPositionY * 0.03 ? true : false; // distance < 3.0% of AvgY
        }

        private static int FindIndexOfMaxDistancePoint(IList<Point> sequence) {

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
        #endregion
    }
}
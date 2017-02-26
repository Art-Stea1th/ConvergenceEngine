using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;

namespace ConvergenceEngine.Models.Mapping.Navigation.Segmentation {

    using Extensions;

    public abstract class SegmentSequence : IReadOnlyList<Segment>, IReadOnlyCollection<Segment>, IEnumerable<Segment> {

        private Lazy<List<Segment>> segments;
        public IEnumerable<Point> Points { get { return segments.Value.SelectMany(p => p).Distinct(); } } // !! Distinct() may be slow

        internal SegmentSequence(IEnumerable<Point> points) {
            segments = new Lazy<List<Segment>>(() => Segmentate(points));
        }

        internal SegmentSequence(IEnumerable<Segment> segments) {
            this.segments = new Lazy<List<Segment>>(() => new List<Segment>(segments)); // <-- TMP
        }

        internal NavigationInfo ConvergenceTo(SegmentSequence sequence) {

            var trackedPairs = SelectTrackedTo(sequence);
            if (trackedPairs.IsNullOrEmpty()) {
                return new NavigationInfo(0.0, 0.0, 0.0); // to be processed later
            }

            var trackedCurrent = new List<Segment>(trackedPairs.Select(s => s.Item1));
            var trackedAnother = new List<Segment>(trackedPairs.Select(s => s.Item2));

            double resultAngle = CalculateAngle(trackedCurrent, trackedAnother);

            trackedAnother = new List<Segment>(trackedAnother.Select(
                s => new Segment(new List<Point> { s.PointA.Rotate(-resultAngle), s.PointB.Rotate(-resultAngle) })));

            Vector? resultDirection = CalculateDirection(trackedCurrent, trackedAnother);

            if (resultDirection == null) {
                return new NavigationInfo(0.0, 0.0, resultAngle);
            }
            return new NavigationInfo(resultDirection.Value, resultAngle);
        }

        public IEnumerable<Tuple<Segment, Segment>> SelectTrackedTo(SegmentSequence sequence) {
            if (sequence.IsNullOrEmpty()) { return null; }
            return SelectorOfTrackedSegmentsPairs.SelectTrackedPairs(this, sequence);
        }

        private double CalculateAngle(IEnumerable<Segment> current, IEnumerable<Segment> another) {

            var lehgths = current.DoSequential(another, (c, a) => (c.Length + a.Length) * 0.5);
            var angles = current.DoSequential(another, (c, a) => Segment.AngleBetween(c, a));

            return AverageWeightedByLengthsAngle(lehgths, angles);
        }

        private double AverageWeightedByLengthsAngle(IEnumerable<double> lengths, IEnumerable<double> angles) {

            var fullLength = lengths.Sum();
            var weights = lengths.Select(s => s * 100.0 / fullLength);

            return angles.DoSequential(weights, (a, w) => a / 100.0 * w).Sum();
        }

        private Vector? CalculateDirection(IReadOnlyList<Segment> current, IReadOnlyList<Segment> another) {

            if (current.Count != another.Count) {
                throw new ArgumentOutOfRangeException();
            }
            var heights = current.DoSequential(another, (c, a) => c.MiddlePoint().ConvergenceTo(c.MiddlePoint().DistancePointTo(a.PointA, a.PointB)));
            return ApproximateDirections(heights);
        }

        private Vector ApproximateDirections(IEnumerable<Vector> directions) {

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

        #region Segmentation

        private List<Segment> Segmentate(IEnumerable<Point> points) {

            List<Segment> result = new List<Segment>();

            if (!points.IsNullOrEmpty()) {

                Segment segment = new Segment(points);
                var segmentPair = SplitByMaxDivergencePoint(segment);

                if (segmentPair == null) {
                    if (IsValidSequence(segment)) {
                        result.Add(segment);
                    }                    
                }
                else {
                    result.AddRange(Segmentate(segmentPair.Item1));
                    result.AddRange(Segmentate(segmentPair.Item2));
                }
            }
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsValidSequence(IReadOnlyList<Point> sequence) {
            var averageDistanceBetweenPoints = sequence.First().DistanceTo(sequence.Last()) / (sequence.Count - 1);
            return averageDistanceBetweenPoints <= ExpectedDistanceBetweenPoints(sequence.Average(p => p.Y));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double ExpectedDistanceBetweenPoints(double segmentPositionY) {
            double a = 100.0 * (2.0 / 3.0), b = a; // magic coefficients // a = b = 100.0 * (2.0 / 3.0);
            return (segmentPositionY - b) / a;     // y = ax + b; => x = (y - b) / a;
        }

        private Tuple<IEnumerable<Point>, IEnumerable<Point>> SplitByMaxDivergencePoint(Segment segment) {

            var allowedDivergence =
                AveragePositionY(segment.MaxDivergencePoint, segment.First(), segment.Last()) * 0.05; // 5.0% of AvgY

            if (segment.MaxDivergence > allowedDivergence) {
                return segment.SplitBy(segment.MaxDivergencePointIndex);
            }
            return null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double AveragePositionY(Point point, Point lineStart, Point lineEnd) {
            return Math.Abs((point.Y + lineStart.Y + lineEnd.Y) / 3);
        }        
        #endregion

        #region Generic Interfaces

        public Segment this[int index] { get { return segments.Value[index]; } }
        public int Count { get { return segments.Value.Count; } }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerator<Segment> GetEnumerator() {
            return segments.Value.GetEnumerator();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator IEnumerable.GetEnumerator() {
            return segments.Value.GetEnumerator();
        }
        #endregion
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;


namespace SLAM.Models.Mapping.Navigation.Segmentation {

    using Extensions;

    internal abstract class SegmentSequence : IReadOnlyList<Segment>, IReadOnlyCollection<Segment>, IEnumerable<Segment> {

        private Lazy<List<Segment>> segments;
        public IEnumerable<Point> Points { get { return segments.Value.SelectMany(p => p).Distinct(); } } // !! Distinct() may be slow

        internal SegmentSequence(IEnumerable<Point> points) {
            segments = new Lazy<List<Segment>>(() => Segmentate(points));
        }

        internal SegmentSequence(IEnumerable<Segment> segments) {
            this.segments = new Lazy<List<Segment>>(() => new List<Segment>(segments)); // <-- TMP
        }

        public NavigationInfo ConvergenceTo(SegmentSequence sequence) {

            var similar = FindSimilarSegmentsTo(sequence);

            var lehgths = similar.Select(sp => (sp.Item1.Length + sp.Item2.Length) / 2);
            var angles = similar.Select(sp => Segment.AngleBetween(sp.Item1, sp.Item2));

            double resultAngle = AverageWeightedByLengthsAngle(lehgths, angles);

            Matrix m = new Matrix();
            m.Rotate(resultAngle);

            similar = similar.Select(sp => new Tuple<Segment, Segment>(
                sp.Item1,
                new Segment(new Tuple<Point, Point>(m.Transform(sp.Item2.PointA), m.Transform(sp.Item2.PointB))))); // <-- TMP

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
                    return FindSegmentWithNearestLengthTo(selection, segment);
                }
                return selection.FirstOrDefault();
            }
            throw new ArgumentNullException(segment.ToString());
        }

        public Segment FindSegmentWithNearestLengthTo(IEnumerable<Segment> sequence, Segment segment) {
            if (segment != null) {
                var minDifference = sequence.Min(s => Math.Abs(segment.Length - s.Length));
                return sequence.Where(sg => Math.Abs(segment.Length - sg.Length) == minDifference).FirstOrDefault();
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

        #region Segmentation

        private List<Segment> Segmentate(IEnumerable<Point> points) {

            List<Segment> result = new List<Segment>();

            if (!points.IsNullOrEmpty()) {

                Segment segment = new Segment(points);
                var segmentPair = SplitByMaxDivergencePoint(segment);

                if (segmentPair == null) {
                    result.Add(segment);
                }
                else {
                    result.AddRange(Segmentate(segmentPair.Item1));
                    result.AddRange(Segmentate(segmentPair.Item2));
                }
            }
            return result;
        }

        private Tuple<IEnumerable<Point>, IEnumerable<Point>> SplitByMaxDivergencePoint(Segment segment) {

            var allowedDivergence =
                AveragePositionY(segment.MaxDivergencePoint, segment.First(), segment.Last()) * 0.03; // 3.0% of AvgY

            if (segment.MaxDivergence > allowedDivergence) {
                return segment.SplitBy(segment.MaxDivergencePointIndex);
            }
            return null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double AveragePositionY(Point point, Point lineStart, Point lineEnd) {
            return ((point.Y + lineStart.Y + lineEnd.Y) / 3);
        }
        #endregion

        #region Generic Interfaces

        public Segment this[int index] { get { return segments.Value[index]; } }
        public int Count { get { return segments.Value.Count; } }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator<Segment> IEnumerable<Segment>.GetEnumerator() {
            return segments.Value.GetEnumerator();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator IEnumerable.GetEnumerator() {
            return segments.Value.GetEnumerator();
        }
        #endregion
    }
}
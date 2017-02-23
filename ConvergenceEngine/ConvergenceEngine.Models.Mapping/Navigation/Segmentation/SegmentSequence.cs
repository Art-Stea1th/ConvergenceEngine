using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;

namespace ConvergenceEngine.Models.Mapping.Navigation.Segmentation {

    using Extensions;
    using Convergence;
    using Convergence.Searchers;

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

            var trackedPairs = SelectTrackedTo(sequence);
            var trackedCurrent = trackedPairs.Select(s => s.Item1);
            var trackedAnother = trackedPairs.Select(s => s.Item2);

            double resultAngle = AngleByLines.SearchBetween(trackedCurrent, trackedAnother);
            trackedAnother = trackedAnother.Select(
                s => new Segment(new List<Point> { s.PointA.Rotate(-resultAngle), s.PointB.Rotate(-resultAngle) }));

            Vector resultDirection;
            int trackedPairsCount = trackedPairs.Count();

            if (trackedPairsCount > 1) {
                resultDirection = OffsetByIntersectionPoints.SearchBetween(new List<Segment>(trackedCurrent), new List<Segment>(trackedAnother));
                return new NavigationInfo(resultDirection, resultAngle);
            }

            if (trackedPairsCount == 1) {
                var currentFirst = trackedCurrent.First();
                var anotherFirst = trackedAnother.First();

                resultDirection = OffsetByExtremePoints.SearchBetween(currentFirst, anotherFirst);
                return new NavigationInfo(resultDirection, resultAngle);
            }

            return new NavigationInfo(new Vector(0.0, 0.0), resultAngle);
        }

        public IEnumerable<Tuple<Segment, Segment>> SelectTrackedTo(SegmentSequence sequence) {
            return SelectorOfTrackedSegmentsPairs.SelectTrackedPairs(this, sequence);
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
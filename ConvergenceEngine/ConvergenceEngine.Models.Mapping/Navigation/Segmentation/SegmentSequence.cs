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

            double resultAngle = AngleByLines.SearchBetween(trackedCurrent, trackedAnother);

            trackedAnother = new List<Segment>(trackedAnother.Select(
                s => new Segment(new List<Point> { s.PointA.Rotate(-resultAngle), s.PointB.Rotate(-resultAngle) })));

            Vector? resultDirection = null;

            if (resultDirection == null) {
                resultDirection = OffsetByIntersectionPoints.SearchBetween(trackedCurrent, trackedAnother);
            }

            if (resultDirection == null) {
                resultDirection = OffsetByExtremePoints.SearchBetween(trackedCurrent, trackedAnother);
            }

            if (resultDirection == null) {
                resultDirection = OffsetByHeights.SearchBetween(trackedCurrent, trackedAnother);
            }

            if (resultDirection == null) {
                return new NavigationInfo(new Vector(0.0, 0.0), resultAngle);
            }
            return new NavigationInfo(resultDirection.Value, resultAngle);
        }

        public IEnumerable<Tuple<Segment, Segment>> SelectTrackedTo(SegmentSequence sequence) {
            if (sequence.IsNullOrEmpty()) { return null; }
            return SelectorOfTrackedSegmentsPairs.SelectTrackedPairs(this, sequence);
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
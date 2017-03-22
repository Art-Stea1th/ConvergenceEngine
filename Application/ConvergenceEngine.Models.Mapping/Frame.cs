using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;

namespace ConvergenceEngine.Models.Mapping {

    using Extensions;
    using Infrastructure.Extensions;
    using Infrastructure.Interfaces;
    using Segments;

    internal sealed class Frame : IFrame {

        private const double AllowedDivergencePercent = 3.0;

        private const double MaxDistancePercent = 5.0;
        private const double MaxAngleDegrees = 3.0;

        private List<Segment> sourceSegments = new List<Segment>();
        private List<Segment> actualSegments = new List<Segment>();

        private NavigationInfo absolute = new NavigationInfo();
        private NavigationInfo relative = new NavigationInfo();

        public Frame Prev { get; private set; } = null;
        // public Frame Next { get; private set; } = null;

        public INavigationInfo Absolute { get { return absolute; } }
        public INavigationInfo Relative { get { return relative; } }

        private List<Segment> nearestToPrev = null;

        internal Frame(Frame frame) {
            sourceSegments = frame.sourceSegments;
            actualSegments = frame.actualSegments;
            absolute = frame.absolute;
            relative = frame.relative;
        }

        internal Frame(IEnumerable<Point> points) {
            Initialize(points.Segmentate(AllowedDivergencePercent).Select(s => new Segment(s)).ToList());
        }

        internal Frame(IEnumerable<ISegment> segments) {
            Initialize((segments as IEnumerable<Segment>).ToList());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Initialize(List<Segment> segments) {
            sourceSegments = segments;
        }

        internal void SetPrev(Frame frame) {
            if (frame == null || ReferenceEquals(frame, this)) {
                return;
            }
            Prev = frame;
            SetOffsetBy(Prev);
        }

        private void SetOffsetBy(Frame frame) {
            var nearest = sourceSegments.SelectNearestTo(frame.sourceSegments, MaxDistancePercent, MaxAngleDegrees, 0, 0);
            relative = nearest.ComputeConvergence(MaxDistancePercent, MaxAngleDegrees, 0, 0);
            absolute = frame.absolute + relative;
            actualSegments = sourceSegments.Select(s => s.RotatedAtZero(absolute.A).Shifted(absolute.X, absolute.Y)).ToList();
        }

        private IEnumerable<Tuple<ISegment, ISegment>> SelectNearestToPrev(double maxDistancePercent, double maxAngleDegrees) {
            foreach (var segment in sourceSegments) {
                var currentMaxDistance = Math.Min(segment.A.Y, segment.B.Y) / 100.0 * maxDistancePercent;
                ISegment nearest = segment.SelectNearestFrom(Prev.sourceSegments, currentMaxDistance, maxAngleDegrees);
                if (nearest != null) {
                    yield return Tuple.Create(segment as ISegment, nearest);
                }
            }
        }

        #region Generic Interfaces

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerator<ISegment> GetEnumerator() {
            return actualSegments.GetEnumerator();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator IEnumerable.GetEnumerator() {
            return actualSegments.GetEnumerator();
        }
        #endregion
    }
}
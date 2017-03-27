using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace ConvergenceEngine.Models.Mapping {

    using Extensions;
    using Infrastructure.Interfaces;
    using Segments;

    internal sealed class Frame : IFrame {

        private const double _allowedDivergencePercent = 3.0, _maxDistancePercent = 5.0, _maxAngleDegrees = 3.0;

        private Frame _prev, _next;
        private IEnumerable<(ISegment current, ISegment nearest)> _nearestWithPrev, _nearestWithNext;
        private INavigationInfo _relativeByPrev, _relativeByNext, _absolute;

        public IEnumerable<ISegment> NearestToPrev => _nearestWithPrev?.Select(s => s.nearest);
        public IEnumerable<ISegment> NearestToNext => _nearestWithNext?.Select(s => s.nearest);

        public INavigationInfo RelativeByPrev => _relativeByPrev ?? new NavigationInfo();
        public INavigationInfo RelativeByNext => _relativeByNext ?? new NavigationInfo();
        public INavigationInfo Absolute {
            get => _absolute ?? new NavigationInfo();
            internal set => _absolute = value;
        }

        public IEnumerable<ISegment> SourceSegments { get; }
        public IEnumerable<ISegment> ActualSegments => SourceSegments?
            .Select(s => (s as Segment).RotatedAtZero(Absolute.A).Shifted(Absolute.X, Absolute.Y));

        public IEnumerable<ISegment> ActualSegmentsNearestOnly => Actual();

        internal Frame(IEnumerable<Point> points) {
            SourceSegments = points.Segmentate(_allowedDivergencePercent).Select(s => new Segment(s));
        }

        private IEnumerable<ISegment> Actual() {
            return _nearestWithPrev?
                //.Select(s => s.current)
                //.Intersect(_prev._nearestWithNext.Select(s => s.nearest))
                .Select(s => (s.current as Segment).RotatedAtZero(Absolute.A).Shifted(Absolute.X, Absolute.Y));
        }

        internal void SetPrev(Frame frame) {
            if (frame == null || ReferenceEquals(frame, this)) {
                return;
            }
            _prev = frame;
            SetNearestWithPrev();
            CalculateNavigationInfoByPrev();
        }

        internal void SetNext(Frame frame) {
            if (frame == null || ReferenceEquals(frame, this)) {
                return;
            }
            _next = frame;
            SetNearestWithNext();
            CalculateNavigationInfoByNext();
        }

        private void SetNearestWithPrev() {
            _nearestWithPrev = SelectNearestWith(_prev, _maxDistancePercent, _maxAngleDegrees);
        }

        private void SetNearestWithNext() {
            _nearestWithNext = SelectNearestWith(_next, _maxDistancePercent, _maxAngleDegrees);
        }

        private void CalculateNavigationInfoByPrev() {
            _relativeByPrev = _nearestWithPrev.ComputeConvergence(_maxDistancePercent, _maxAngleDegrees);
        }

        private void CalculateNavigationInfoByNext() {
            _relativeByNext = _nearestWithNext.ComputeConvergence(_maxDistancePercent, _maxAngleDegrees);
        }

        private IEnumerable<(ISegment current, ISegment nearest)> SelectNearestWith(Frame frame,
            double maxDistancePercent, double maxAngleDegrees) {

            foreach (var segment in SourceSegments) {

                double maxDistance = Math.Min(segment.A.Y, segment.B.Y) / 100.0 * maxDistancePercent;

                var nearest = (segment as Segment).SelectNearestFrom(frame.SourceSegments, maxDistance, maxAngleDegrees);
                if (nearest != null) {
                    yield return (current: segment as ISegment, nearest: nearest);
                }
            }
        }

        #region Generic Interfaces

        public IEnumerator<ISegment> GetEnumerator() => SourceSegments.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => SourceSegments.GetEnumerator();
        #endregion
    }
}
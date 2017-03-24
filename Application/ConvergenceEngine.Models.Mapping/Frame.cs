using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;

namespace ConvergenceEngine.Models.Mapping {

    using Extensions;
    using Infrastructure.Interfaces;
    using Segments;

    internal sealed class Frame : /*IFrame,*/ IEnumerable<ISegment> {

        private const double _allowedDivergencePercent = 3.0, _maxDistancePercent = 5.0, _maxAngleDegrees = 3.0;

        private Frame _prev, _next;

        private List<Segment> SourceSegments { get; }
        //private List<Segment> ActualSegments { get; set; }

        private IEnumerable<(ISegment current, ISegment nearest)> _nearestWithPrev, _nearestWithNext;

        private NavigationInfo _relative, _absolute;


        //public INavigationInfo Absolute { get => absolute; }
        //public INavigationInfo Relative { get => relative; }


        //internal Frame(Frame frame) {
        //    sourceSegments = frame.sourceSegments;
        //    actualSegments = frame.actualSegments;
        //    absolute = frame.absolute;
        //    relative = frame.relative;
        //}

        internal Frame(IEnumerable<Point> points) {
            SourceSegments = points.Segmentate(_allowedDivergencePercent).Select(s => new Segment(s)).ToList();
        }

        //internal Frame(IEnumerable<ISegment> segments) {
        //    sourceSegments = (segments as IEnumerable<Segment>).ToList();
        //}

        internal void SetPrev(Frame frame) {
            if (frame == null || ReferenceEquals(frame, this)) {
                return;
            }
            _prev = frame;
            SetNearestWithPrev();
        }


        private void CalculateRelative() {
            //_relative = _nearestWithPrev.ComputeConvergence(_maxDistancePercent, _maxAngleDegrees);
        }

        private void SetNearestWithPrev() {
            _nearestWithPrev = SelectNearestWith(_prev, _maxDistancePercent, _maxAngleDegrees);
        }

        private void SetNearestWithNext() {
            _nearestWithNext = SelectNearestWith(_next, _maxDistancePercent, _maxAngleDegrees);
        }

        private IEnumerable<(ISegment current, ISegment nearest)> SelectNearestWith(Frame frame,
            double maxDistancePercent, double maxAngleDegrees) {

            foreach (var segment in frame.SourceSegments) {

                double maxDistance = Math.Min(segment.A.Y, segment.B.Y) / 100.0 * maxDistancePercent;

                var nearest = segment.SelectNearestFrom(frame.SourceSegments, maxDistance, maxAngleDegrees);
                if (nearest != null) {
                    yield return (current: segment as ISegment, nearest: nearest);
                }
            }
        }

        //private void SetOffsetBy(Frame frame) {
        //    var nearest = sourceSegments.SelectNearestTo(frame.sourceSegments, MaxDistancePercent, MaxAngleDegrees);
        //    relative = nearest.ComputeConvergence(MaxDistancePercent, MaxAngleDegrees);
        //    absolute = frame.absolute + relative;
        //    actualSegments = sourceSegments.Select(s => s.RotatedAtZero(absolute.A).Shifted(absolute.X, absolute.Y)).ToList();
        //}

        #region Generic Interfaces

        public IEnumerator<ISegment> GetEnumerator() => SourceSegments.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => SourceSegments.GetEnumerator();
        #endregion
    }
}
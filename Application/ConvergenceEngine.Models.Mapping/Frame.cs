using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace ConvergenceEngine.Models.Mapping {

    using Extensions;
    using Infrastructure.Interfaces;
    using Segments;

    internal sealed class Frame : IFrame {

        private const double AllowedDivergencePercent = 3.0;

        private const double MaxDistancePercent = 5.0;
        private const double MaxAngleDegrees = 3.0;

        private List<Segment> segments;

        private Frame prev = null, next = null;

        public Frame Prev {
            get { return prev; }
            set { prev = ReferenceEquals(value, this) ? null : value; }
        }
        public Frame Next {
            get { return next; }
            set { next = ReferenceEquals(value, this) ? null : value; }
        }

        private NavigationInfo absolute;
        private NavigationInfo relative;

        public INavigationInfo Absolute { get { return absolute; } }
        public INavigationInfo Relative { get { return relative; } }

        internal Frame(Frame frame) {
            segments = frame.segments;
            absolute = frame.absolute;
            relative = frame.relative;
        }

        internal Frame(IEnumerable<Point> points) {
            Initialize(points.Segmentate(AllowedDivergencePercent).Select(s => new Segment(s)).ToList());
        }

        internal Frame(IEnumerable<ISegment> segments) {
            Initialize((segments as IEnumerable<Segment>).ToList());
        }

        private void Initialize(List<Segment> segments) {
            this.segments = segments;
            SetOffsetBy(this);
        }

        internal void SetOffsetBy(Frame frame) {
            if (frame == null || ReferenceEquals(frame, this)) {
                absolute = new NavigationInfo();
                relative = new NavigationInfo();
                return;
            }
            var nearest = segments.SelectNearestTo(frame.segments, MaxDistancePercent, MaxAngleDegrees, 0, 0);
            relative = nearest.ComputeConvergence(MaxDistancePercent, MaxAngleDegrees, 0, 0);
            absolute = frame.absolute + relative;
        }

        #region Generic Interfaces

        public IEnumerator<ISegment> GetEnumerator() {
            return segments.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return segments.GetEnumerator();
        }
        #endregion
    }
}
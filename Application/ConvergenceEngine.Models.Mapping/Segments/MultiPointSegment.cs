using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace ConvergenceEngine.Models.Mapping.Segments {

    using Extensions;
    using Infrastructure.Extensions;
    using Infrastructure.Interfaces;

    public class MultiPointSegment : Segment, ISegment {

        protected List<Point> points;

        public override Point this[int index] { get { return points[index]; } }
        public override int Count { get { return points.Count; } }

        internal MultiPointSegment(MultiPointSegment segment) : this(segment.points) { }
        internal MultiPointSegment(IEnumerable<Point> linearSortedPoints) : base(linearSortedPoints.ApproximateSorted()) {
            points = new List<Point>(linearSortedPoints);
        }

        public override void ApplyTransform(double offsetX, double offsetY, double angle, bool rotatePrepend = true) {
            if (rotatePrepend) {
                points = new List<Point>(points.Select(p => p.RotatedAndShifted(offsetX, offsetY, angle)));
            }
            else {
                points = new List<Point>(points.Select(p => p.ShiftedAndRotated(offsetX, offsetY, angle)));
            }
            base.ApplyTransform(offsetX, offsetY, angle, rotatePrepend);
        }

        public override IEnumerator<Point> GetEnumerator() {
            return points.GetEnumerator();
        }
    }
}
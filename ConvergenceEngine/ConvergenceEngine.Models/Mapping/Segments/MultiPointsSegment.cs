using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Windows;

namespace ConvergenceEngine.Models.Mapping.Segments {

    using Extensions.Ops;

    public class MultiPointsSegment : Segment {

        protected List<Point> points;

        public override Point this[int index] { get { return points[index]; } }
        public override int Count { get { return points.Count; } }

        internal MultiPointsSegment(IEnumerable<Point> linearSortedPoints) : base(linearSortedPoints.ApproximateSorted()) {
            points = new List<Point>(linearSortedPoints);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override IEnumerator<Point> GetEnumerator() {
            return points.GetEnumerator();
        }
    }
}
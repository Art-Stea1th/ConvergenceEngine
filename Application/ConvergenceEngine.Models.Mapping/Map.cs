using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ConvergenceEngine.Models.Mapping {

    using Extensions;
    using Infrastructure.Extensions;
    using Infrastructure.Interfaces;
    using Segments;

    public sealed class Map : IEnumerable<ISegment> {

        private const double MaxDistancePercent = 25.0; // using Selector & Determinator
        private const double MaxAngleDegrees = 30.0;    // using Selector & Determinator

        private SortedList<int, ISegment> segments;

        public int UnusedId { get { return segments.Count; } }

        public Map(IEnumerable<MultiPointSegment> segments) {
            this.segments = new SortedList<int, ISegment>();
            foreach (var segment in segments) {                
                AddSegment(UnusedId, segment);
            }
        }

        public void AddSegment(ISegment segment) {
            var nearest = segments.SelectNearestTo(segment, MaxDistancePercent, MaxAngleDegrees);
            if (nearest.Value == null) {
                AddSegment(UnusedId, segment);
            }
            else {
                AddSegment(nearest.Key, segment);
            }
        }

        private void AddSegment(int id, ISegment segment) {
            if (segment.Count < 2) {
                return;
            }
            if (segments.ContainsKey(id)) {
                //var result = MergedFromSegment(segments[id], segment);
                var result = GetLarger(segments[id], segment);
                segments.RemoveAt(id);
                segments.Add(id, result);
            }
            else {
                segments.Add(id, segment);
            }
        }

        private ISegment MergedFromSegment(ISegment current, ISegment another) {

            ISegment primary, secondary;

            if (current.Length >= another.Length) {
                primary = current; secondary = another;
            }
            else {
                primary = another; secondary = current;
            }

            var angle = Segment.AngleBetween(secondary, primary);
            secondary = new MultiPointSegment(secondary.Select(p => p.RotatedAt(angle, secondary.CenterPoint.X, secondary.CenterPoint.Y)));

            var direction = secondary.CenterPoint.ConvergenceTo(secondary.CenterPoint.DistancePointTo(primary.PointA, primary.PointB));
            secondary.ApplyTransform(direction.X, direction.Y, 0);

            var resultPoints = new List<ISegment> { primary, secondary }.SelectMany(p => p)
                .OrderByLine(primary.PointA, primary.PointB).ThinOutSorted(3.0);

            return new MultiPointSegment(resultPoints);
        }

        private ISegment GetLarger(ISegment current, ISegment another) {
            return current.Length >= another.Length ? current : another;
        }

        public IEnumerator<ISegment> GetEnumerator() {
            return segments.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return segments.GetEnumerator();
        }
    }
}
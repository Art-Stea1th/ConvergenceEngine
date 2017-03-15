using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ConvergenceEngine.Models.Mapping {

    using Extensions;
    using Infrastructure.Extensions;
    using Infrastructure.Interfaces;
    using Segments;

    internal sealed class Map : IEnumerable<MapSegment> {

        private const double MaxDistancePercent = 10.0; // using Selector & Determinator
        private const double MaxAngleDegrees = 10.0;    // using Selector & Determinator

        private List<MapSegment> segments;

        public int UnusedId { get { return segments.Count; } }

        internal Map(IEnumerable<Segment> segments) {
            this.segments = new List<MapSegment>();
            foreach (var segment in segments) {                
                AddSegment(new MapSegment(UnusedId, segment));
            }
        }

        public void AddSegment(MapSegment segment) {

            var existing = segments.FirstOrDefault(s => s.Id == segment.Id);

            if (existing == null) {
                existing = segments.SelectNearestTo(segment, MaxDistancePercent, MaxAngleDegrees);
            }
            if (existing != null) {
                segments.RemoveAll(s => s.Id == segment.Id);
                segment = GetLarger(segment, existing);
            }
            segments.Add(segment);
        }

        private MapSegment MergedFromSegment(MapSegment current, MapSegment another) {

            MapSegment primary, secondary;

            if (current.Length >= another.Length) {
                primary = current; secondary = another;
            }
            else {
                primary = another; secondary = current;
            }

            var angle = secondary.AngleTo(primary);
            secondary = new MapSegment(secondary.Select(p => p.RotatedAt(angle, secondary.Center.X, secondary.Center.Y)));

            var direction = secondary.Center.DistanceVectorTo(secondary.Center.DistancePointTo(primary.A, primary.B));
            secondary.ApplyTransform(direction.X, direction.Y, 0);

            var resultPoints = new List<MapSegment> { primary, secondary }.SelectMany(p => p)
                .OrderByLine(primary.A, primary.B).ThinOutSorted(3.0);

            return new MapSegment(resultPoints);
        }

        private MapSegment GetLarger(MapSegment current, MapSegment another) {
            return current.Length >= another.Length ? current : another;
        }

        public IEnumerator<MapSegment> GetEnumerator() {
            return segments.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return segments.GetEnumerator();
        }
    }
}
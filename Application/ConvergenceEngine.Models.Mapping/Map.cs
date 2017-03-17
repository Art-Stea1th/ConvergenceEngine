using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ConvergenceEngine.Models.Mapping {

    using Extensions;
    using Infrastructure.Extensions;
    using Infrastructure.Interfaces;
    using Segments;

    internal sealed class Map : IEnumerable<Segment> {

        private const double MaxDistancePercent = 5.0; // using Selector & Determinator
        private const double MaxAngleDegrees = 3.0;    // using Selector & Determinator

        private List<Segment> segments;

        //public int UnusedId { get { return segments.Count; } }

        internal Map(IEnumerable<Segment> segments) {
            this.segments = new List<Segment>();
            foreach (var segment in segments) {
                AddSegment(new Segment(segment));
            }
        }

        public void AddSegment(Segment segment) {

            var //existing = segments.FirstOrDefault(s => s.Id == segment.Id);

            //if (existing == null) {
                existing = segments.SelectNearestTo(segment, MaxDistancePercent, MaxAngleDegrees);
            //}
            if (existing != null) {
                int index = segments.IndexOf(existing);
                segments.RemoveAt(index);
                segment = GetLarger(segment, existing);
            }
            segments.Add(segment);
        }

        private Segment GetLarger(Segment current, Segment another) {
            return current.Length >= another.Length ? current : another;
        }

        public IEnumerator<Segment> GetEnumerator() {
            return segments.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return segments.GetEnumerator();
        }
    }
}
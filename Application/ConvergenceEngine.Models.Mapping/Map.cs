using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ConvergenceEngine.Models.Mapping {

    using Infrastructure.Extensions;
    using Infrastructure.Interfaces;
    using Segments;

    internal sealed class Map : IEnumerable<Segment> {

        private const double _maxDistancePercent = 5.0; // using Selector & Determinator
        private const double _maxAngleDegrees = 3.0;    // using Selector & Determinator

        private List<Segment> _segments = new List<Segment>();

        internal Map() { }


        internal void AddSegments(IEnumerable<ISegment> segments) {
            if (segments.IsNullOrEmpty()) {
                return;
            }
            _segments.AddRange(segments.Select(s => s as Segment));
        }

        private Segment GetLarger(Segment current, Segment another) {
            return current.Length >= another.Length ? current : another;
        }

        public IEnumerator<Segment> GetEnumerator() => _segments.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _segments.GetEnumerator();
    }
}
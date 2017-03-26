using System.Collections;
using System.Collections.Generic;

namespace ConvergenceEngine.Models.Mapping {

    using Segments;

    internal sealed class Map : IEnumerable<Segment> {

        private const double _maxDistancePercent = 5.0; // using Selector & Determinator
        private const double _maxAngleDegrees = 3.0;    // using Selector & Determinator

        private List<Segment> _segments = new List<Segment>();

        internal Map() { }


        internal void AddSegments(IEnumerable<Segment> segments) {

        }

        private Segment GetLarger(Segment current, Segment another) {
            return current.Length >= another.Length ? current : another;
        }

        public IEnumerator<Segment> GetEnumerator() => _segments.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _segments.GetEnumerator();
    }
}
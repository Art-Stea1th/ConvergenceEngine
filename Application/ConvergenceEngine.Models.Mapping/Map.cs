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

        private List<Segment> segments = new List<Segment>();

        internal Map() { }


        internal void AddSegments(IEnumerable<Segment> segments) {

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
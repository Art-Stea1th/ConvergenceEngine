using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace ConvergenceEngine.Models.Mapping {

    using Extensions;
    using Mapping.Segments;

    internal sealed class FrameX {

        private const double _allowedDivergencePercent = 3.0, _maxDistancePercent = 5.0, _maxAngleDegrees = 3.0;

        private List<Segment> _segments;

        internal FrameX(IEnumerable<Point> points) {
            _segments = points.Segmentate(_allowedDivergencePercent).Select(s => new Segment(s)).ToList();
        }

        public static void SmoothOut(FrameX frameA, FrameX frameB) {

            var segmentsA = frameA._segments;
            var segmentsB = frameB._segments;

            var aToB = new List<(Segment segment, List<Segment> nearests)>();

            for (int a = 0; a < segmentsA.Count; ++a) {
                aToB.Add((segment: segmentsA[a], nearests: new List<Segment>()));

                for (int b = 0; b < segmentsB.Count; ++b) {
                    if (aToB[a].segment.NearestByAngleTo(segmentsB[b], _maxAngleDegrees)) {
                        aToB[a].nearests.Add(segmentsB[b]);
                    }
                }
            }

        }
    }
}

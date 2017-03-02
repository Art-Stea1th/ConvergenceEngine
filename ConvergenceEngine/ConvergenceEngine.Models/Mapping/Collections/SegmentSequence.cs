using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ConvergenceEngine.Models.Mapping.Collections {

    using Extensions;
    using Extensions.Iterable;
    using Extensions.Ops;

    public abstract class SegmentSequence : ReadOnlyLazyList<Segment> {

        public IEnumerable<Segment> Segments { get { return this; } }
        public IEnumerable<Point> Points { get { return this.SelectMany(p => p); } }

        internal SegmentSequence(IEnumerable<Point> points) : base(() => points.Segmentate(5.0)) {

        }

        /*protected*/
        internal NavigationInfo ConvergenceTo(SegmentSequence sequence, double maxDistancePercent = 5.0, double maxAngleDegrees = 3.0) {

            var trackedPairs = SelectTrackedTo(sequence);
            if (trackedPairs.IsNullOrEmpty()) {
                return new NavigationInfo(0.0, 0.0, 0.0); // to be processed later
            }

            var trackedCurrent = new List<Segment>(trackedPairs.Select(s => s.Item1));
            var trackedAnother = new List<Segment>(trackedPairs.Select(s => s.Item2));

            double resultAngle = trackedCurrent.DetermineAngleTo(trackedAnother);

            trackedAnother = new List<Segment>(trackedAnother.Select(
                s => new Segment(new List<Point> { s.PointA.Rotate(-resultAngle), s.PointB.Rotate(-resultAngle) })));

            Vector resultDirection = trackedCurrent.DetermineDirectionTo(trackedAnother);

            //if (resultDirection == null) {
            //    return new NavigationInfo(0.0, 0.0, resultAngle);
            //}
            return new NavigationInfo(resultDirection/*.Value*/, resultAngle);
        }

        public IEnumerable<Tuple<Segment, Segment>> SelectTrackedTo(IEnumerable<Segment> sequence) {
            return this.SelectSimilarTo(sequence);
        }
    }
}
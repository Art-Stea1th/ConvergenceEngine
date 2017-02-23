using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace ConvergenceEngine.Models.Mapping.Navigation.Convergence {

    using Segmentation;

    internal static class SelectorOfTrackedSegmentsPairs { // as static TMP

        public const double MaxDistancePercent = 5.0;
        public const double MaxAngleDegrees = 3.0;

        public static IEnumerable<Tuple<Segment, Segment>> SelectTrackedPairs(SegmentSequence current, SegmentSequence another) {

            foreach (var segment in current) {

                var currentMaxDistance = Math.Min(segment.PointA.Y, segment.PointB.Y) / 100.0 * MaxDistancePercent;

                Segment similar = FindSimilarSegmentFor(another, segment, currentMaxDistance, MaxAngleDegrees);
                if (similar != null) {
                    yield return new Tuple<Segment, Segment>(segment, similar);
                }
            }
        }

        private static Segment FindSimilarSegmentFor(IEnumerable<Segment> sequence, Segment segment, double maxDistance, double maxAngle) {
            if (segment != null) {
                var selection = FindSegmentsByDistanceTo(sequence, segment, maxDistance).Intersect(FindSegmentsByAngleTo(sequence, segment, maxAngle));
                if (selection.Count() > 1) {
                    return FindSegmentWithNearestLengthTo(selection, segment);
                }
                return selection.FirstOrDefault();
            }
            throw new ArgumentNullException(segment.ToString());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Segment FindSegmentWithNearestLengthTo(IEnumerable<Segment> sequence, Segment segment) {
            if (segment != null) {
                var minDifference = sequence.Min(s => Math.Abs(segment.Length - s.Length));
                return sequence.Where(sg => Math.Abs(segment.Length - sg.Length) == minDifference).FirstOrDefault();
            }
            throw new ArgumentNullException(segment.ToString());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static IEnumerable<Segment> FindSegmentsByAngleTo(IEnumerable<Segment> sequence, Segment segment, double maxAngle) {
            if (segment != null) {
                return sequence.Where(s => Math.Abs(Segment.AngleBetween(segment, s)) < maxAngle);
            }
            throw new ArgumentNullException(segment.ToString());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static IEnumerable<Segment> FindSegmentsByDistanceTo(IEnumerable<Segment> sequence, Segment segment, double maxDistance) {
            if (segment != null) {
                return sequence.Where(s => segment.DistanceToNearestPoint(s) < maxDistance);
            }
            throw new ArgumentNullException(segment.ToString());
        }
    }
}
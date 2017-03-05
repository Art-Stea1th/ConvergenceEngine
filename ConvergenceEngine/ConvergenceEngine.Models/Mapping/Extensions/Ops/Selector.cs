using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace ConvergenceEngine.Models.Mapping.Extensions.Ops {

    using Segments;

    internal static class Selector { // IEnumerable<ISegment> Extension class

        public static IEnumerable<Tuple<ISegment, ISegment>> SelectSimilarTo(this IEnumerable<ISegment> current, IEnumerable<ISegment> another,
            double maxDistancePercent = 5.0, double maxAngleDegrees = 3.0) {

            foreach (var segment in current) {
                var currentMaxDistance = Math.Min(segment.PointA.Y, segment.PointB.Y) / 100.0 * maxDistancePercent;

                ISegment similar = another.SelectSimilarSegmentTo(segment, currentMaxDistance, maxAngleDegrees);
                if (similar != null) {
                    yield return new Tuple<ISegment, ISegment>(segment, similar);
                }
            }
        }

        public static ISegment SelectSimilarSegmentTo(this IEnumerable<ISegment> sequence, ISegment segment, double maxDistance, double maxAngle) {
            var selection = sequence.SelectSegmentsByDistanceTo(segment, maxDistance).Intersect(sequence.SelectSegmentsByAngleTo(segment, maxAngle));
            if (selection.Count() > 1) {
                return selection.SelectSegmentWithNearestLengthTo(segment);
            }
            return selection.FirstOrDefault();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ISegment SelectSegmentWithNearestLengthTo(this IEnumerable<ISegment> sequence, ISegment segment) {
            var minDifference = sequence.Min(s => Math.Abs(segment.Length - s.Length));
            return sequence.Where(sg => Math.Abs(segment.Length - sg.Length) == minDifference).FirstOrDefault();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<ISegment> SelectSegmentsByAngleTo(this IEnumerable<ISegment> sequence, ISegment segment, double maxAngle) {
            return sequence.Where(s => Math.Abs(Segment.AngleBetween(segment, s)) < maxAngle);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<ISegment> SelectSegmentsByDistanceTo(this IEnumerable<ISegment> sequence, ISegment segment, double maxDistance) {
            return sequence.Where(s => segment.DistanceToNearestPoint(s) < maxDistance);
        }
    }
}
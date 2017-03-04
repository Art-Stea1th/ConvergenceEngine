using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace ConvergenceEngine.Models.Mapping.Extensions.Ops {

    using Segments;

    internal static class Selector { // IEnumerable<Segment> Extension class

        public static IEnumerable<Tuple<MultiPointsSegment, MultiPointsSegment>> SelectSimilarTo(this IEnumerable<MultiPointsSegment> current, IEnumerable<MultiPointsSegment> another,
            double maxDistancePercent = 5.0, double maxAngleDegrees = 3.0) {

            foreach (var segment in current) {
                var currentMaxDistance = Math.Min(segment.PointA.Y, segment.PointB.Y) / 100.0 * maxDistancePercent;

                MultiPointsSegment similar = another.SelectSimilarSegmentTo(segment, currentMaxDistance, maxAngleDegrees);
                if (similar != null) {
                    //segment.Id = similar.Id;
                    yield return new Tuple<MultiPointsSegment, MultiPointsSegment>(segment, similar);
                }
            }
        }

        public static MultiPointsSegment SelectSimilarSegmentTo(this IEnumerable<MultiPointsSegment> sequence, MultiPointsSegment segment, double maxDistance, double maxAngle) {
            var selection = sequence.SelectSegmentsByDistanceTo(segment, maxDistance).Intersect(sequence.SelectSegmentsByAngleTo(segment, maxAngle));
            if (selection.Count() > 1) {
                return selection.SelectSegmentWithNearestLengthTo(segment);
            }
            return selection.FirstOrDefault();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static MultiPointsSegment SelectSegmentWithNearestLengthTo(this IEnumerable<MultiPointsSegment> sequence, MultiPointsSegment segment) {
            var minDifference = sequence.Min(s => Math.Abs(segment.Length - s.Length));
            return sequence.Where(sg => Math.Abs(segment.Length - sg.Length) == minDifference).FirstOrDefault();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<MultiPointsSegment> SelectSegmentsByAngleTo(this IEnumerable<MultiPointsSegment> sequence, MultiPointsSegment segment, double maxAngle) {
            return sequence.Where(s => Math.Abs(MultiPointsSegment.AngleBetween(segment, s)) < maxAngle);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<MultiPointsSegment> SelectSegmentsByDistanceTo(this IEnumerable<MultiPointsSegment> sequence, MultiPointsSegment segment, double maxDistance) {
            return sequence.Where(s => segment.DistanceToNearestPoint(s) < maxDistance);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace ConvergenceEngine.Models.Mapping.Extensions.Ops {

    internal static class Selector { // IEnumerable<Segment> Extension class

        public static IEnumerable<Tuple<Segment, Segment>> SelectSimilarTo(this IEnumerable<Segment> current, IEnumerable<Segment> another,
            double maxDistancePercent = 5.0, double maxAngleDegrees = 3.0) {

            foreach (var segment in current) {

                var currentMaxDistance = Math.Min(segment.PointA.Y, segment.PointB.Y) / 100.0 * maxDistancePercent;

                Segment similar = another.SelectSimilarSegmentTo(segment, currentMaxDistance, maxAngleDegrees);
                if (similar != null) {
                    yield return new Tuple<Segment, Segment>(segment, similar);
                }
            }
        }

        public static Segment SelectSimilarSegmentTo(this IEnumerable<Segment> sequence, Segment segment, double maxDistance, double maxAngle) {
            var selection = sequence.SelectSegmentsByDistanceTo(segment, maxDistance).Intersect(sequence.SelectSegmentsByAngleTo(segment, maxAngle));
            if (selection.Count() > 1) {
                return selection.SelectSegmentWithNearestLengthTo(segment);
            }
            return selection.FirstOrDefault();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Segment SelectSegmentWithNearestLengthTo(this IEnumerable<Segment> sequence, Segment segment) {
            var minDifference = sequence.Min(s => Math.Abs(segment.Length - s.Length));
            return sequence.Where(sg => Math.Abs(segment.Length - sg.Length) == minDifference).FirstOrDefault();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<Segment> SelectSegmentsByAngleTo(this IEnumerable<Segment> sequence, Segment segment, double maxAngle) {
            return sequence.Where(s => Math.Abs(Segment.AngleBetween(segment, s)) < maxAngle);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<Segment> SelectSegmentsByDistanceTo(this IEnumerable<Segment> sequence, Segment segment, double maxDistance) {
            return sequence.Where(s => segment.DistanceToNearestPoint(s) < maxDistance);
        }
    }
}
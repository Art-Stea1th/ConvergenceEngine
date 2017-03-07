using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace ConvergenceEngine.Models.Mapping.Extensions {

    using Segments;
    using Infrastructure.Interfaces;

    internal static class Selector { // IEnumerable<ISegment> Extension class

        public static IEnumerable<Tuple<ISegment, ISegment>> SelectNearestTo(
            this IEnumerable<ISegment> current, IEnumerable<ISegment> another, double maxDistancePercent = 5.0, double maxAngleDegrees = 3.0) {

            foreach (var segment in current) {
                var currentMaxDistance = Math.Min(segment.PointA.Y, segment.PointB.Y) / 100.0 * maxDistancePercent;

                ISegment similar = another.SelectNearestTo(segment, currentMaxDistance, maxAngleDegrees);
                if (similar != null) {
                    yield return new Tuple<ISegment, ISegment>(segment, similar);
                }
            }
        }

        public static ISegment SelectNearestTo(this IEnumerable<ISegment> sequence, ISegment segment, double maxDistance, double maxAngle) {
            var selection = sequence.SelectByDistanceTo(segment, maxDistance).Intersect(sequence.SelectByAngleTo(segment, maxAngle));
            if (selection.Count() > 1) {
                return selection.SelectWithNearestLengthTo(segment);
            }
            return selection.FirstOrDefault();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ISegment SelectWithNearestLengthTo(this IEnumerable<ISegment> sequence, ISegment segment) {
            var minDifference = sequence.Min(s => Math.Abs(segment.Length - s.Length));
            return sequence.Where(sg => Math.Abs(segment.Length - sg.Length) == minDifference).FirstOrDefault();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<ISegment> SelectByAngleTo(this IEnumerable<ISegment> sequence, ISegment segment, double maxAngle) {
            return sequence.Where(s => Math.Abs(Segment.AngleBetween(segment, s)) < maxAngle);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<ISegment> SelectByDistanceTo(this IEnumerable<ISegment> sequence, ISegment segment, double maxDistance) {
            return sequence.Where(s => segment.DistanceToNearestPoint(s) < maxDistance);
        }
    }
}
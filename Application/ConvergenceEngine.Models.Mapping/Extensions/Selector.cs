using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace ConvergenceEngine.Models.Mapping.Extensions {

    using Segments;
    using Infrastructure.Interfaces;

    internal static class Selector { // IEnumerable<ISegment> Extension class

        public static IEnumerable<Tuple<ISegment, ISegment>> SelectNearestTo(
            this IEnumerable<ISegment> current, IEnumerable<ISegment> another, double maxDistancePercent, double maxAngleDegrees) {

            foreach (var segment in current) {
                var currentMaxDistance = Math.Min(segment.PointA.Y, segment.PointB.Y) / 100.0 * maxDistancePercent;

                ISegment similar = another.SelectNearestTo(segment, currentMaxDistance, maxAngleDegrees);
                if (similar != null) {
                    yield return new Tuple<ISegment, ISegment>(segment, similar);
                }
            }
        }

        // Nearest

        public static ISegment SelectNearestTo(
            this IEnumerable<ISegment> sequence, ISegment segment, double maxDistance, double maxAngleDegrees) {
            var selection = sequence.SelectByDistanceTo(segment, maxDistance)
                .Intersect(sequence.SelectByAngleTo(segment, maxAngleDegrees));
            if (selection.Count() > 1) {
                return selection.SelectWithNearestLengthTo(segment);
            }
            return selection.FirstOrDefault();
        }

        public static KeyValuePair<int, ISegment> SelectNearestTo(
            this IEnumerable<KeyValuePair<int, ISegment>> sequence, ISegment segment, double maxDistance, double maxAngleDegrees) {
            var selection = sequence.SelectByDistanceTo(segment, maxDistance)
                .Intersect(sequence.SelectByAngleTo(segment, maxAngleDegrees));
            if (selection.Count() > 1) {
                return selection.SelectWithNearestLengthTo(segment);
            }
            return selection.FirstOrDefault();
        }

        // by Nearest Length

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ISegment SelectWithNearestLengthTo(this IEnumerable<ISegment> sequence, ISegment segment) {
            var minDifference = sequence.Min(s => Math.Abs(segment.Length - s.Length));
            return sequence.Where(sg => Math.Abs(segment.Length - sg.Length) == minDifference).FirstOrDefault();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static KeyValuePair<int, ISegment> SelectWithNearestLengthTo(this IEnumerable<KeyValuePair<int, ISegment>> sequence, ISegment segment) {
            var minDifference = sequence.Min(s => Math.Abs(segment.Length - s.Value.Length));
            return sequence.Where(sg => Math.Abs(segment.Length - sg.Value.Length) == minDifference).FirstOrDefault();
        }

        // by Angle

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<ISegment> SelectByAngleTo(this IEnumerable<ISegment> sequence, ISegment segment, double maxAngleDegrees) {
            return sequence.Where(s => Math.Abs(Segment.AngleBetween(segment, s)) < maxAngleDegrees);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<KeyValuePair<int, ISegment>> SelectByAngleTo(this IEnumerable<KeyValuePair<int, ISegment>> sequence, ISegment segment, double maxAngleDegrees) {
            return sequence.Where(s => Math.Abs(Segment.AngleBetween(segment, s.Value)) < maxAngleDegrees);
        }

        // by Distance

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<ISegment> SelectByDistanceTo(this IEnumerable<ISegment> sequence, ISegment segment, double maxDistance) {
            return sequence.Where(s => segment.DistanceToNearestPoint(s) < maxDistance);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<KeyValuePair<int, ISegment>> SelectByDistanceTo(this IEnumerable<KeyValuePair<int, ISegment>> sequence, ISegment segment, double maxDistance) {
            return sequence.Where(s => segment.DistanceToNearestPoint(s.Value) < maxDistance);
        }
    }
}
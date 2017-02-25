using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;

namespace ConvergenceEngine.Models.Mapping.Navigation.Convergence.Searchers {

    using Extensions;
    using Segmentation;

    internal static class OffsetByIntersectionPoints { // as static TMP

        private const double AbsoluteMinDegrees = 30.0;
        private const double AbsoluteMaxDegrees = 180.0 - AbsoluteMinDegrees;
        private const double MaxDifferenceDegrees = 3.0;
        private const double EtalonDegrees = 90.0;

        public static Vector? SearchBetween(IReadOnlyList<Segment> current, IReadOnlyList<Segment> another) { // quick impl.

            if (current.IsNullOrEmpty() || another.IsNullOrEmpty()) {
                return null;
            }
            if (current.Count != another.Count) {
                throw new ArgumentOutOfRangeException();
            }
            if (current.Count < 2 || another.Count < 2) {
                return null;
            }

            return CalculateDirection(current, another);
        }

        private static Vector? CalculateDirection(IReadOnlyList<Segment> current, IReadOnlyList<Segment> another) {

            var resultDirections = new List<Vector>();
            var resultAngles = new List<double>();

            for (int i = 0; i < current.Count - 1; ++i) {
                for (int j = i + 1; j < current.Count; ++j) {

                    var currentAngle = Segment.AngleBetween(current[i], current[j]); // angle between Current Neighbourns
                    var anotherAngle = Segment.AngleBetween(another[i], another[j]); // angle between Another Neighbourns

                    if (IsPermissibleAngleError(currentAngle, anotherAngle, MaxDifferenceDegrees)) {

                        var averageAbsoluteAngle = Math.Abs((currentAngle + anotherAngle) * 0.5);
                        if (AngleInRange(averageAbsoluteAngle, AbsoluteMinDegrees, AbsoluteMaxDegrees)) { // check range

                            var currentDirection = CalculateDirection(current[i], current[j], another[i], another[j], limit: 3.0);
                            if (currentDirection != null) {
                                resultDirections.Add(currentDirection.Value);
                                resultAngles.Add(GetProximityToTheStraightAngle(averageAbsoluteAngle));
                            }
                        }
                    }
                }
            }
            if (resultDirections.IsEmpty()) {
                return null;
            }
            return AverageWeightedByAnglesDirection(resultAngles, resultDirections);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsPermissibleAngleError(double angleA, double angleB, double maxError) {
            return Math.Abs(angleA - angleB) <= maxError;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool AngleInRange(double angle, double rangeMin, double rangeMax) {
            return rangeMin < angle && angle < rangeMax;
        }

        private static Vector? CalculateDirection(Segment currentA, Segment currentB, Segment anotherA, Segment anotherB, double limit) {

            Point? intersectionA = currentA.IntersectionPointWith(currentB);
            if (intersectionA == null) {
                return null;
            }
            Point? intersectionB = anotherA.IntersectionPointWith(anotherB);
            if (intersectionB == null) {
                return null;
            }
            return intersectionA.Value.ConvergenceTo(intersectionB.Value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double GetProximityToTheStraightAngle(double angle) {
            return 90 - Math.Abs(90 - angle);
        }

        private static Vector AverageWeightedByAnglesDirection(IEnumerable<double> angles, IEnumerable<Vector> directions) {

            var fullAngle = angles.Sum();
            var weights = angles.Select(s => s * 100.0 / fullAngle);

            return directions.DoSequential(weights, (d, w) => d / 100.0 * w).Sum();
        }
    }
}
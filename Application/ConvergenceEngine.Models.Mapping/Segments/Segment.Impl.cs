using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;

namespace ConvergenceEngine.Models.Mapping.Segments {

    using Extensions;
    using Infrastructure.Extensions;
    using Infrastructure.Interfaces;

    internal partial class Segment {

        private static Point Zero = new Point(0.0, 0.0);
        private static Vector BasisX = new Vector(1.0, 0.0);
        private static Vector BasisY = new Vector(0.0, 1.0);

        public double AngleToHorizontal { get { return AngleTo(BasisX); } }
        public double AngleToVertical { get { return AngleTo(BasisY); } }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double AngleTo(Segment segment) {
            return AngleTo(segment.B - segment.A);
        }

        private double AngleTo(Vector vector) {
            //var angle = Vector.AngleBetween((B - A), vector);
            //return Math.Abs(angle) < 90.0 ? angle : angle < 0 ? angle + 180.0 : angle - 180.0;
            return Vector.AngleBetween((B - A), vector);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Segment ShiftedX(double offsetX) {
            return new Segment(A.ShiftedX(offsetX), B.ShiftedX(offsetX));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Segment ShiftedY(double offsetY) {
            return new Segment(A.ShiftedY(offsetY), B.ShiftedY(offsetY));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Segment Shifted(Vector direction) {
            return new Segment(A.Shifted(direction.X, direction.Y), B.Shifted(direction.X, direction.Y));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Segment Shifted(double offsetX, double offsetY) {
            return new Segment(A.Shifted(offsetX, offsetY), B.Shifted(offsetX, offsetY));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Segment RotatedAtZero(double angle) {
            return RotatedAt(angle, Zero);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Segment RotatedAtCenter(double angle) {
            return RotatedAt(angle, Center);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Segment RotatedAt(double angle, Point center) {
            return new Segment(A.RotatedAt(angle, center), B.RotatedAt(angle, center));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Segment RotatedAt(double angle, double centerX, double centerY) {
            return new Segment(A.RotatedAt(angle, centerX, centerY), B.RotatedAt(angle, centerX, centerY));
        }

        public ISegment SelectNearestFrom(IEnumerable<Segment> sequence, double maxDistance, double maxAngle) {
            var selection = sequence
                .Where(s => s.NearestByExtremePointsDistanceTo(this, maxDistance))
                .Where(s => s.NearestByAngleTo(this, maxAngle));

            if (selection.Count() > 1) {
                return selection.MinBy(s => Math.Abs(s.Length - Length));
            }
            return selection.FirstOrDefault();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool NearestByExtremePointsDistanceTo(Segment segment, double maxDistance) {
            return DistanceToNearestExtremePoints(segment) > maxDistance ? false : true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool NearestByAngleTo(Segment segment, double maxAngle) {
            maxAngle = Math.Abs(maxAngle); var realAngle = Math.Abs(AngleTo(segment));
            return realAngle <= maxAngle || 180.0 - realAngle <= maxAngle ? true : false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double DistanceToNearestExtremePoints(Segment segment) {
            return new[] {
                A.DistanceTo(segment.A),
                A.DistanceTo(segment.B),
                B.DistanceTo(segment.A),
                B.DistanceTo(segment.B)
            }.Min();
        }

        internal Point? IntersectPointWith(Segment segment) {

            var lineCoefficients1 = LineCoefficients();
            var lineCoefficients2 = segment.LineCoefficients();

            var a1 = lineCoefficients1.Item1;
            var b1 = lineCoefficients1.Item2;
            var c1 = lineCoefficients1.Item3;

            var a2 = lineCoefficients2.Item1;
            var b2 = lineCoefficients2.Item2;
            var c2 = lineCoefficients2.Item3;

            var commonDenominator = a1 * b2 - a2 * b1;

            if (commonDenominator == 0.0) {
                return null;
            }

            var resultX = -(c1 * b2 - c2 * b1) / commonDenominator;
            var resultY = -(a1 * c2 - a2 * c1) / commonDenominator;

            return new Point(resultX, resultY);
        }

        private Tuple<double, double, double> LineCoefficients() {
            var a = A.Y - B.Y;
            var b = B.X - A.X;
            var c = -(a * A.X) - (b * A.Y);
            return new Tuple<double, double, double>(a, b, c);
        }

        public void ApplyTransform(double offsetX, double offsetY, double angle, bool rotatePrepend = true) {
            if (rotatePrepend) {
                A = A.RotatedAndShifted(offsetX, offsetY, angle);
                B = B.RotatedAndShifted(offsetX, offsetY, angle);
            }
            else {
                A = A.ShiftedAndRotated(offsetX, offsetY, angle);
                B = B.ShiftedAndRotated(offsetX, offsetY, angle);
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace ConvergenceEngine.Models.Mapping.Segments {

    using Infrastructure.Extensions;
    using Infrastructure.Interfaces;

    internal partial class Segment {

        public static readonly Point Zero = new Point(0.0, 0.0);
        public static readonly Vector BasisX = new Vector(1.0, 0.0);
        public static readonly Vector BasisY = new Vector(0.0, 1.0);

        public double AngleToHorizontal => AngleTo(BasisX);
        public double AngleToVertical => AngleTo(BasisY);

        public double AngleTo(ISegment segment) {
            return AngleTo(segment.B - segment.A);
        }

        private double AngleTo(Vector vector) {
            return Vector.AngleBetween((B - A), vector);
        }

        public Segment ShiftedX(double offsetX) {
            return new Segment(A.ShiftedX(offsetX), B.ShiftedX(offsetX));
        }

        public Segment ShiftedY(double offsetY) {
            return new Segment(A.ShiftedY(offsetY), B.ShiftedY(offsetY));
        }

        public Segment Shifted(Vector direction) {
            return new Segment(A.Shifted(direction.X, direction.Y), B.Shifted(direction.X, direction.Y));
        }

        public Segment Shifted(double offsetX, double offsetY) {
            return new Segment(A.Shifted(offsetX, offsetY), B.Shifted(offsetX, offsetY));
        }

        public Segment RotatedAtZero(double angle) {
            return RotatedAt(angle, Zero);
        }

        public Segment RotatedAtCenter(double angle) {
            return RotatedAt(angle, Center);
        }

        public Segment RotatedAt(double angle, Point center) {
            return new Segment(A.RotatedAt(angle, center), B.RotatedAt(angle, center));
        }

        public Segment RotatedAt(double angle, double centerX, double centerY) {
            return new Segment(A.RotatedAt(angle, centerX, centerY), B.RotatedAt(angle, centerX, centerY));
        }

        public ISegment SelectNearestFrom(IEnumerable<ISegment> sequence, double maxDistance, double maxAngle) {
            var selection = sequence
                .Where(s => NearestByExtremePointsDistanceTo(s, maxDistance) && NearestByAngleTo(s, maxAngle));

            if (selection.Count() > 1) {
                return selection.MinBy(s => Math.Abs(s.Length - Length));
            }
            return selection.FirstOrDefault();
        }

        public bool NearestByExtremePointsDistanceTo(ISegment segment, double maxDistance) {
            return DistanceToNearestExtremePoints(segment) > Math.Abs(maxDistance) ? false : true;
        }

        public bool NearestByAngleTo(ISegment segment, double maxAngle) {
            maxAngle = Math.Abs(maxAngle); double realAngle = Math.Abs(AngleTo(segment));
            return realAngle <= maxAngle || 180.0 - realAngle <= maxAngle ? true : false;
        }

        public double DistanceToNearestExtremePoints(ISegment segment) {
            return new[] {
                A.DistanceTo(segment.A),
                A.DistanceTo(segment.B),
                B.DistanceTo(segment.A),
                B.DistanceTo(segment.B)
            }.Min();
        }

        internal Point? IntersectPointWith(Segment segment) {

            var lc1 = LineCoefficients();
            var lc2 = segment.LineCoefficients();

            double a1 = lc1.a, b1 = lc1.b, c1 = lc1.c;
            double a2 = lc2.a, b2 = lc2.b, c2 = lc2.c;

            double commonDenominator = a1 * b2 - a2 * b1;

            if (commonDenominator == 0.0) {
                return null;
            }

            double resultX = -(c1 * b2 - c2 * b1) / commonDenominator;
            double resultY = -(a1 * c2 - a2 * c1) / commonDenominator;

            return new Point(resultX, resultY);
        }

        private (double a, double b, double c) LineCoefficients() {
            double a = A.Y - B.Y;
            double b = B.X - A.X;
            double c = -(a * A.X) - (b * A.Y);
            return (a: a, b: b, c: c);
        }
    }
}
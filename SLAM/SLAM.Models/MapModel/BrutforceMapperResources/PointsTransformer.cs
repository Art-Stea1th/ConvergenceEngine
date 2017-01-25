using System;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;

namespace SLAM.Models.MapModel.BrutforceMapperResources {

    internal sealed class PointsTransformer {

        public void Transform(Point[] points, double offsetX, double offsetY, double angle) {
            Rotate(points, angle);
            ShiftXY(points, offsetX, offsetY);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Rotate(Point[] points, double angle) {
            for (int i = 0; i < points.Length; ++i) {
                points[i] = Rotate(points[i], angle);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Point Rotate(Point point, double angle) {
            angle = TrigonometricHelper.RadiansFromDegrees(angle);
            double rx = (point.X * Math.Cos(angle)) + (point.Y * Math.Sin(angle));
            double ry = (point.Y * Math.Cos(angle)) - (point.X * Math.Sin(angle));
            return new Point(rx, ry);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ShiftXY(Point[] points, double x, double y) {
            Matrix matrix = new Matrix();
            matrix.Translate(x, y);
            matrix.Transform(points);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ShiftX(Point[] points, double x) {
            Matrix matrix = new Matrix();
            matrix.Translate(x, 0.0);
            matrix.Transform(points);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ShiftY(Point[] points, double y) {
            Matrix matrix = new Matrix();
            matrix.Translate(0.0, y);
            matrix.Transform(points);
        }
    }
}
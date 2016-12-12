using System;
using System.Windows;
using System.Windows.Media;
using System.Collections.Generic;

namespace SLAM.Models.Map {

    using Data.Readers;
    using System.Runtime.CompilerServices;

    internal sealed class BrutforceMapper : BaseMapper {

        // buffers
        private Point[] prevFrameBuffer; // previous
        private Point[] currFrameBuffer; // current

        // settings X, Y, Angle
        private int xStep, xRange, resultX;
        private int yStep, yRange, resultY;
        private double aStep, aRange, resultAngle;

        public BrutforceMapper(DataProvider dataProvider) : base(dataProvider) {
            Configure(1.0, 5.0, 60.0, 30.0);
        }

        private void Configure(double xKmpH, double yKmpH, double degreePerSec, double fps) {

            xRange = PixelPerFrameBySpeed(xKmpH, fps) * 2;
            yRange = PixelPerFrameBySpeed(yKmpH, fps) * 2;
            aRange = (degreePerSec / fps) * 2;

            xStep = 1; yStep = 1; aStep = 0.25;
        }

        private int PixelPerFrameBySpeed(double speedKmH, double fps) {
            double mPerH = speedKmH * 1000.0;
            double cmPerH = mPerH * 100.0;
            double cmPerMin = cmPerH / 60.0;
            double cmPerSec = cmPerMin / 60.0;
            double cmPerFrame = cmPerSec / fps;
            return (int)Math.Round(cmPerFrame); // 1 cm = 1 pixel
        }

        protected override void NextFrameProceed() {
            DataProvider.GetNextFrameTo(out currFrameBuffer);
            currFrameBuffer = NormalizeFrame(currFrameBuffer).ToArray();
            AddNextFrameToResultMap();
        }

        private List<Point> NormalizeFrame(Point[] points) {

            List<Point> result = new List<Point>();

            foreach (var point in points) {
                if (!PointInSequenceExists(point, result)) {
                    result.Add(point);
                }
            }
            return result;
        }

        private void AddNextFrameToResultMap() {
            if (prevFrameBuffer == null || ResultMap == null) {

                prevFrameBuffer = new Point[currFrameBuffer.Length];
                Array.Copy(currFrameBuffer, prevFrameBuffer, currFrameBuffer.Length);

                ResultMap = new Point[currFrameBuffer.Length];
                Array.Copy(currFrameBuffer, ResultMap, currFrameBuffer.Length);                
                return;
            }
            BrutforceNextMoving();            
            prevFrameBuffer = currFrameBuffer;
        }

        private void BrutforceNextMoving() {

            List<Point> minDefference = new List<Point>(currFrameBuffer);

            double rotateA = 0.0;
            int shiftX = 0, shiftY = 0;

            for (double a = aRange + aStep; a != 0.0; a = Bounce(a, aStep)) {
                /// rotate ex.  +3.0, -6.0, +5.5 -5.0, +4.5, -4.0, +3.5, -3.0, +2.5, -2.0, +1.5, -1.0, +0.5 ...                
                Rotate(currFrameBuffer, a <= aRange ? a : aRange / 2);
                rotateA += a <= aRange ? a : aRange / 2;

                for (int y = yRange + yStep; y != 0; y = Bounce(y, yStep)) {
                    /// shift-y ex.  +5, -10, +9, -8, +7, -6, +5, -4, +3, -2, +1 ...
                    ShiftY(currFrameBuffer, y <= yRange ? y : yRange / 2);
                    shiftY += y <= yRange ? y : yRange / 2;

                    for (int x = xRange + xStep; x != 0; x = Bounce(x, xStep)) {
                        /// shift-x ex.  +1 -2 +1
                        ShiftX(currFrameBuffer, x <= xRange ? x : xRange / 2);
                        shiftX += x <= xRange ? x : xRange / 2;

                        List<Point> currentDifference = GetDifference(prevFrameBuffer, currFrameBuffer);
                        if (currentDifference.Count < minDefference.Count) {
                            minDefference = currentDifference;
                            resultX = -shiftX; resultY = -shiftY; resultAngle = -rotateA;
                        }
                    }
                }
                if (Math.Abs(a) < aStep) { a = 0.0; } // <- infinity protection
            }
            //Console.WriteLine($"Done! -- x: {resultX}, y: {resultY}, angle: {resultAngle} --");

            if (resultX != 0 && resultY != 0 && resultAngle != 0.0) {
                Transform(ResultMap, resultX, resultY, resultAngle);
                MergeDifferenceToMap(minDefference);
            }            
        }

        private void MergeDifferenceToMap(List<Point> difference) {

            List<Point> result = new List<Point>(ResultMap);

            foreach (var point in difference) {
                result.Add(point);
            }
            ResultMap = result.ToArray();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private double Bounce(double value, double step) {
            return value <= 0
                ? value + (-value * 2 - step)
                : value - (value * 2 - step);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int Bounce(int value, int step) {
            return value <= 0
                ? value + (-value * 2 - step)
                : value - (value * 2 - step);
        }

        private void Transform(Point[] points, int x, int y, double angle) {
            Rotate(points, angle);
            foreach (var point in points) {
                point.Offset(x, y);
            }
        }

        private void Rotate(Point[] points, double angle) {
            Matrix matrix = new Matrix();
            matrix.RotateAt(angle, 0.0, 0.0);
            matrix.Transform(points);
        }

        private void ShiftX(Point[] points, int x) {
            foreach (var point in points) {
                point.Offset(x, 0);
            }
        }

        private void ShiftY(Point[] points, int y) {
            foreach (var point in points) {
                point.Offset(0, y);
            }
        }

        private List<Point> GetDifference(Point[] prevBuffer, Point[] currBuffer) {

            List<Point> result = new List<Point>();

            foreach (var point in currBuffer) {
                if (!PointInSequenceExists(point, prevBuffer)) {
                    result.Add(point);
                }
            }
            return result;
        }

        private bool PointInSequenceExists(Point point, IEnumerable<Point> sequence) {
            foreach (var sPoint in sequence) {
                if (HitPoint(point, sPoint)) {
                    return true;
                }
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool HitPoint(Point pointA, Point pointB) {
            return
                Math.Abs(pointA.X - pointB.X) < xStep * 2 &&
                Math.Abs(pointA.Y - pointB.Y) < xStep * 2;
        }
    }
}
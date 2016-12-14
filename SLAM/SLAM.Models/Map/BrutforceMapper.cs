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
        private double xStep, xRange, resultX;
        private double yStep, yRange, resultY;
        private double aStep, aRange, resultAngle;

        public BrutforceMapper(DataProvider dataProvider) : base(dataProvider) {
            Configure(1.0, 7.0, 120.0, 30.0);
        }

        private void Configure(double xKmpH, double yKmpH, double degreePerSec, double fps) {

            xRange = PixelPerFrameBySpeed(xKmpH, fps) * 2;
            yRange = PixelPerFrameBySpeed(yKmpH, fps) * 2;
            aRange = (degreePerSec / fps) * 2;

            xStep = 1.0;
            yStep = 1.0;
            aStep = 0.25;
        }

        private double PixelPerFrameBySpeed(double speedKmH, double fps) {
            double mPerH = speedKmH * 1000.0;
            double cmPerH = mPerH * 100.0;
            double cmPerMin = cmPerH / 60.0;
            double cmPerSec = cmPerMin / 60.0;
            double cmPerFrame = cmPerSec / fps;
            return Math.Round(cmPerFrame, 1); // 1 cm = 1 pixel
        }

        protected override void NextFrameProceed() {
            DataProvider.GetNextFrameTo(out currFrameBuffer);
            currFrameBuffer = NormalizeFrame(currFrameBuffer).ToArray();
            AddNextFrameToResultMap();
        }

        private List<Point> NormalizeFrame(Point[] points) {

            List<Point> result = new List<Point>();

            foreach (var point in points) {
                if (!PointInSequenceExists(result, point, 1.0, 1.0)) {
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
            double shiftX = 0, shiftY = 0;

            for (double a = aRange + aStep; a != 0.0; a = Bounce(a, aStep)) {
                /// rotate ex.  +3.0, -6.0, +5.5 -5.0, +4.5, -4.0, +3.5, -3.0, +2.5, -2.0, +1.5, -1.0, +0.5 ...                
                Rotate(currFrameBuffer, a <= aRange ? a : aRange / 2);
                rotateA += a <= aRange ? a : aRange / 2;

                for (double y = yRange + yStep; y != 0; y = Bounce(y, yStep)) {
                    /// shift-y ex.  +5, -10, +9, -8, +7, -6, +5, -4, +3, -2, +1 ...
                    ShiftY(currFrameBuffer, y <= yRange ? y : yRange / 2);
                    shiftY += y <= yRange ? y : yRange / 2;

                    for (double x = xRange + xStep; x != 0; x = Bounce(x, xStep)) {
                        /// shift-x ex.  +1 -2 +1 ...
                        ShiftX(currFrameBuffer, x <= xRange ? x : xRange / 2);
                        shiftX += x <= xRange ? x : xRange / 2;

                        List<Point> currentDifference = GetDifference(prevFrameBuffer, currFrameBuffer, xStep, yStep);
                        if (currentDifference.Count < minDefference.Count) {
                            minDefference = currentDifference;
                            resultX = -shiftX; resultY = -shiftY; resultAngle = -rotateA;
                        }
                    }
                }
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
            double result = value <= 0.0
                ? value + (-value * 2.0 - step)
                : value - (value * 2.0 - step);
            return Math.Abs(result) < step ? 0.0 : /*Math.Round(*/result/*, 1)*/;
        }

        private void Transform(Point[] points, double x, double y, double angle) {
            Matrix matrix = new Matrix();
            matrix.Rotate(angle);
            matrix.Translate(x, y);
            matrix.Transform(points);
        }

        private void Rotate(Point[] points, double angle) {
            Matrix matrix = new Matrix();
            matrix.Rotate(angle);
            matrix.Transform(points);
        }

        private void ShiftX(Point[] points, double x) {
            Matrix matrix = new Matrix();
            matrix.Translate(x, 0.0);
            matrix.Transform(points);
        }

        private void ShiftY(Point[] points, double y) {
            Matrix matrix = new Matrix();
            matrix.Translate(0.0, y);
            matrix.Transform(points);
        }

        private List<Point> GetDifference(Point[] prevBuffer, Point[] currBuffer, double Xlimit, double Ylimit) {

            List<Point> result = new List<Point>();

            foreach (var point in currBuffer) {
                if (!PointInSequenceExists(prevBuffer, point, Xlimit, Ylimit)) {
                    result.Add(point);
                }
            }
            return result;
        }

        private bool PointInSequenceExists(IEnumerable<Point> sequence, Point point, double Xlimit, double Ylimit) {
            foreach (var sPoint in sequence) {
                if (HitPoint(point, sPoint, Xlimit, Ylimit)) {
                    return true;
                }
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool HitPoint(Point pointA, Point pointB, double Xlimit, double Ylimit) {
            return
                Math.Abs(pointA.X - pointB.X) < Xlimit &&
                Math.Abs(pointA.Y - pointB.Y) < Ylimit;
        }
    }
}
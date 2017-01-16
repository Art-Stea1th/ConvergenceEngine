using System;
using System.Windows;
using System.Windows.Media;
using System.Collections.Generic;
using System.Runtime.CompilerServices;


namespace SLAM.Models.MapModel {

    using DataModel.Readers;    

    internal sealed class BrutforceMapper : BaseMapper {

        // buffers
        private Point[] prevFrameBuffer; // previous
        private Point[] currFrameBuffer; // current

        // settings X, Y, Angle
        private float xStep, xRange, resultX;
        private float yStep, yRange, resultY;
        private float aStep, aRange, resultAngle;

        public BrutforceMapper(DataProvider dataProvider) : base(dataProvider) {
            Configure(1.0f, 5.0f, 90.0f, 30.0f);
        }

        private void Configure(float xKmpH, float yKmpH, float degreePerSec, float fps) {

            xRange = PixelPerFrameBySpeed(xKmpH, fps) * 2;
            yRange = PixelPerFrameBySpeed(yKmpH, fps) * 2;
            aRange = (degreePerSec / fps) * 2;

            xStep = 1.0f;
            yStep = 1.0f;
            aStep = 0.1f;
        }

        private float PixelPerFrameBySpeed(float speedKmH, float fps) {
            float mPerH = speedKmH * 1000.0f;
            float cmPerH = mPerH * 100.0f;
            float cmPerMin = cmPerH / 60.0f;
            float cmPerSec = cmPerMin / 60.0f;
            float cmPerFrame = cmPerSec / fps;
            return (float)Math.Round(cmPerFrame); // 1 cm = 1 pixel
        }

        protected override void NextFrameProceed() {
            DataProvider.GetNextFrameTo(out currFrameBuffer);
            currFrameBuffer = NormalizeFrame(currFrameBuffer).ToArray();
            AddNextFrameToResultMap();
        }

        private List<Point> NormalizeFrame(Point[] points) {

            List<Point> result = new List<Point>();

            foreach (var point in points) {
                if (!PointInSequenceExists(result, point, 2.0f, 2.0f)) {
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

            float rotateA = 0.0f;
            float shiftX = 0.0f, shiftY = 0.0f;

            for (float a = aRange + aStep; a != 0.0; a = Bounce(a, aStep)) {
                /// rotate ex.  +3.0, -6.0, +5.5 -5.0, +4.5, -4.0, +3.5, -3.0, +2.5, -2.0, +1.5, -1.0, +0.5 ...                
                Rotate(currFrameBuffer, a <= aRange ? a : aRange / 2);
                rotateA += a <= aRange ? a : aRange / 2;

                for (float y = yRange + yStep; y != 0; y = Bounce(y, yStep)) {
                    /// shift-y ex.  +5, -10, +9, -8, +7, -6, +5, -4, +3, -2, +1 ...
                    ShiftY(currFrameBuffer, y <= yRange ? y : yRange / 2);
                    shiftY += y <= yRange ? y : yRange / 2;

                    for (float x = xRange + xStep; x != 0; x = Bounce(x, xStep)) {
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
        private float Bounce(float value, float step) {
            float result = value <= 0.0f
                ? value + (-value * 2.0f - step)
                : value - (value * 2.0f - step);
            return Math.Abs(result) < step ? 0.0f : (float)Math.Round(result, 1);
        }

        private void Transform(Point[] points, float x, float y, float angle) {
            Matrix matrix = new Matrix();
            matrix.Rotate(angle);
            //matrix.Translate(x, y);
            matrix.Transform(points);
        }

        private void Rotate(Point[] points, float angle) {
            Matrix matrix = new Matrix();
            matrix.Rotate(angle);
            matrix.Transform(points);
        }

        private void ShiftX(Point[] points, float x) {
            //Matrix matrix = new Matrix();
            //matrix.Translate(x, 0.0);
            //matrix.Transform(points);
        }

        private void ShiftY(Point[] points, float y) {
            //Matrix matrix = new Matrix();
            //matrix.Translate(0.0, y);
            //matrix.Transform(points);
        }

        private List<Point> GetDifference(Point[] prevBuffer, Point[] currBuffer, float Xlimit, float Ylimit) {

            List<Point> result = new List<Point>();

            foreach (var point in currBuffer) {
                if (!PointInSequenceExists(prevBuffer, point, Xlimit, Ylimit)) {
                    result.Add(point);
                }
            }
            return result;
        }

        private bool PointInSequenceExists(IEnumerable<Point> sequence, Point point, float Xlimit, float Ylimit) {
            foreach (var sPoint in sequence) {
                if (HitPoint(point, sPoint, Xlimit, Ylimit)) {
                    return true;
                }
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool HitPoint(Point pointA, Point pointB, float Xlimit, float Ylimit) {
            return
                Math.Abs(pointA.X - pointB.X) < Xlimit &&
                Math.Abs(pointA.Y - pointB.Y) < Ylimit;
        }
    }
}
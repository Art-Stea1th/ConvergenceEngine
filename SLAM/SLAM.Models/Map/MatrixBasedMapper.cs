using System;
using System.Windows;
using System.Windows.Media;


namespace SLAM.Models.Map {

    using Data.Readers;    

    internal sealed class MatrixBasedMapper : BaseMaper {        

        protected Point[] previousPointsBuffer;
        protected Point[] currentPointsBuffer;

        protected Point[] estimatedPointsBuffer;

        //private Matrix matrix;

        int maxHitPoints;

        private double translateXRange, translateYRange, rotateAngleRange;
        private double translateXStep, translateYStep, rotateAngleStep;
        private double translateX = 0.0, translateY = 0.0, rotateAngle = 0.0;

        public MatrixBasedMapper(DataProvider dataProvider) : base(dataProvider) {
            Configure(5.0, 1.0, 90.0, 30.0);
        }

        private void Configure(double xKmpH, double yKmpH, double degreePerSec, double fps) {

            translateXRange = PixelPerFrameBySpeed(xKmpH, fps);
            translateYRange = PixelPerFrameBySpeed(yKmpH, fps);
            rotateAngleRange = DegreePerFrame(degreePerSec, fps);

            translateXStep = 1.0;
            translateYStep = 1.0;
            rotateAngleStep = 0.5;
        }

        private double PixelPerFrameBySpeed(double speedKmH, double fps) {
            double mPerH = speedKmH * 1000.0;
            double cmPerH = speedKmH * 100.0; 
            double cmPerMin = cmPerH * 0.6;
            double cmPerSec = cmPerMin * 0.6;
            double cmPerFrame = cmPerSec * (1.0 / fps);
            return cmPerFrame; // <-- 1 cm = 1 pixel
        }
        private double DegreePerFrame(double degreePerSec, double fps) {
            return degreePerSec * (1.0 / fps);
        }

        protected override void NextFrameProceed() {
            previousPointsBuffer = currentPointsBuffer;
            ResetTransform();
            DataProvider.GetNextFrameTo(out currentPointsBuffer);
            if (EstimatedBufferInitialize()) {                
                FindTransform();
            }            
        }

        private bool EstimatedBufferInitialize() {
            if (previousPointsBuffer == null) {
                return false;
            }
            if (estimatedPointsBuffer == null) {
                estimatedPointsBuffer = new Point[previousPointsBuffer.Length];
            }            
            Array.Copy(previousPointsBuffer, estimatedPointsBuffer, estimatedPointsBuffer.Length);
            return true;
        }
        
        private void ResetTransform() {
            //matrix = new Matrix();
            translateX = translateY = rotateAngle = 0.0;
            maxHitPoints = 0;
        }

        private void FindTransform() {

            // prepare points
            //ApplyTransform(estimatedPointsBuffer, translateXRange * -0.5, translateYRange * -0.5, rotateAngleRange * -0.5);

            //int maxHitPoints = 0;

            maxHitPoints = HitPointsCount(estimatedPointsBuffer, previousPointsBuffer);

            for (double x = -translateXRange; x <= translateXRange; x += translateXStep) {
                for (double y = -translateYRange; y <= translateYRange; y += translateYStep) {
                    for (double angle = -rotateAngleRange; angle <= rotateAngleRange; angle += rotateAngleStep) {

                        ApplyTransform(estimatedPointsBuffer, x, y, angle);

                        int currentHitPoints = HitPointsCount(estimatedPointsBuffer, currentPointsBuffer);
                        if (currentHitPoints > maxHitPoints) {
                            maxHitPoints = currentHitPoints;
                            translateX = x; translateY = y; rotateAngle = angle;
                        }
                        Array.Copy(previousPointsBuffer, estimatedPointsBuffer, estimatedPointsBuffer.Length); // reset
                    }
                }
            }
        }

        private void ApplyTransform(Point[] points, double offsetX, double offsetY, double angle) {

            Matrix matrix = new Matrix();
            double centerX = DataProvider.FrameInfo.Width * 0.5;
            double centerY = DataProvider.FrameInfo.Height - 1;
            matrix.RotateAt(angle, centerX, centerY);
            matrix.Transform(points);

            for (int i = 0; i < points.Length; ++i) {
                points[i].X += offsetX;
                points[i].Y += offsetY;
            }            
        }

        private int HitPointsCount(Point[] pointsBufferA, Point[] pointsBufferB) {

            int resultHits = 0;
            for (int i = 0; i < pointsBufferA.Length; ++i) {
                for (int j = 0; j < pointsBufferB.Length; ++j) {
                    if (HitPoint(pointsBufferA[i], pointsBufferB[j])) {
                        ++resultHits;
                    }
                }
            }
            return resultHits;
        }

        private bool HitPoint(Point pointA, Point pointB) {
            return
                Math.Abs(pointA.X - pointB.X) < translateXStep &&
                Math.Abs(pointA.Y - pointB.Y) < translateYStep;
        }
    }
}
using System;
using System.Windows;
using System.Windows.Media;


namespace SLAM.Models.Map {

    using Data.Readers;    

    internal sealed class MatrixBasedMapper : BaseMaper {        

        protected Point[] prevPointsBuffer;
        protected Point[] nextPointsBuffer;

        protected Point[] tmpPointsBuffer;

        private Matrix matrix;

        private double translateXRange, translateYRange, rotateAngleRange;
        private double translateXStep, translateYStep, rotateAngleStep;
        private double translateX = 0.0, translateY = 0.0, rotateAngle = 0.0;

        public MatrixBasedMapper(DataProvider dataProvider) : base(dataProvider) {
            Configure();
        }

        private void Configure() {

            translateXStep = 0.2;
            translateYStep = 0.2;
            rotateAngleStep = 0.2;

            translateXRange = 12.0;
            translateYRange = 12.0;
            rotateAngleRange = 20.0;            
        }

        protected override void NextFrameProceed() {
            prevPointsBuffer = nextPointsBuffer;
            DataProvider.GetNextFrameTo(out nextPointsBuffer);
            TempBufferInitialize();
            ResetTransform();
        }

        private void TempBufferInitialize() {
            if (tmpPointsBuffer == null) {
                tmpPointsBuffer = new Point[nextPointsBuffer.Length];
            }            
            Array.Copy(nextPointsBuffer, tmpPointsBuffer, tmpPointsBuffer.Length);           
        }
        
        private void ResetTransform() {
            matrix = new Matrix();
            translateX = translateY = rotateAngle = 0.0;
        }

        private void FindTransform() {

            // prepare points
            ApplyTransform(tmpPointsBuffer, translateXRange * -0.5, translateYRange * -0.5, rotateAngleRange * -0.5);

            for (double x = 0.0; x <= translateX; x += translateXStep) {
                for (double y = 0.0; y <= translateYRange; y += translateYStep) {
                    for (double angle = 0.0; angle <= rotateAngleRange; angle += rotateAngleStep) {

                    }
                }
            }
        }

        private void ApplyTransform(Point[] points, double offsetX, double offsetY, double angle) {
            for (int i = 0; i < points.Length; ++i) {
                points[i].X += offsetX;
                points[i].Y += offsetY;
            }
            matrix.RotateAt(angle, DataProvider.FrameInfo.Width * 0.5, DataProvider.FrameInfo.Height - 1);
            matrix.Transform(points);
        }
    }
}
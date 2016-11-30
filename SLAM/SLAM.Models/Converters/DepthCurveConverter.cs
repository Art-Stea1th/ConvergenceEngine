using System;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace SLAM.Models.Converters {

    internal sealed class DepthCurveConverter : DataConverter, IDataConverter {

        private Color viewportCurveColor;

        private int middleLineIndex;
        private int startIndex, endIndex;

        private Point3D[] middleHorizontalLineFromPointCloud;        

        public DepthCurveConverter(DepthFrameSequenceInfo frameInfo) : base(frameInfo) {
            Initialize();
            InitializePoints3DBuffer();
        }

        private void Initialize() {
            viewportCurveColor = Color.FromArgb(255, 255, 192, 128);

            middleLineIndex = FrameInfo.Height / 2;
            startIndex = GetLinearIndex(0, middleLineIndex, FrameInfo.Width);
            endIndex = GetLinearIndex(FrameInfo.Width - 1, middleLineIndex, FrameInfo.Width);
        }

        private void InitializePoints3DBuffer() {
            middleHorizontalLineFromPointCloud = new Point3D[FrameInfo.Width];
            for (int i = 0; i < FrameInfo.Width; ++i) {
                middleHorizontalLineFromPointCloud[i].X = i; // FrameInfo.Width / 2 - i;
                middleHorizontalLineFromPointCloud[i].Y = middleLineIndex;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void UpdatePointCloudMiddleLineFromRawInput(byte[] rawInput) {
            for (int i = startIndex; i < endIndex; ++i) {
                short depth = GetDepthFromRawFrameAt(rawInput, i * sizeof(short));
                middleHorizontalLineFromPointCloud[i - startIndex].Z = depth;
            }
        }

        public override void ConvertRawDataToViewportFrame(byte[] rawInput, byte[] viewportOutput) {

            HorizontalMirror(rawInput);
            UpdatePointCloudMiddleLineFromRawInput(rawInput);

            for (int i = 0; i < FrameInfo.Width; ++i) {

                double x = middleHorizontalLineFromPointCloud[i].X; // 0   - 639
                double y = middleHorizontalLineFromPointCloud[i].Y; // 240 - 240
                double z = middleHorizontalLineFromPointCloud[i].Z; // 800 - 4000

                if (z < FrameInfo.MinDepth || z > FrameInfo.MaxDepth) { continue; }

                double deltaAngleBetweenRays = ((FrameInfo.NominalHorizontalFOV * 0.5) / FrameInfo.Width) * x;

                double resultX, resultY;
                PolarToRectangular(z, deltaAngleBetweenRays, out resultX, out resultY);
                //PerspectiveToRectangle(x, y, z, out resultX, out resultY);
                
                // --- Convert to viewport ---

                int imageX = (int)(resultX * 0.1);
                int imageY = (int)(resultY * 0.1);

                int resultLinearIndex = GetLinearIndex(imageX * sizeof(int), imageY, FrameInfo.Width * sizeof(int));

                if (resultLinearIndex >= 0) {
                    SetColorToViewportByteArray(viewportOutput, resultLinearIndex, viewportCurveColor);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void PolarToRectangular(double radius, double angle, out double x, out double y) {
            x = radius * Math.Cos(angle * Math.PI / 180.0);
            y = radius * Math.Sin(angle * Math.PI / 180.0);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void PerspectiveToRectangle(double x, double y, double z, out double rectX, out double rectY) {

            double cx = 339.307;
            double cy = 242.739;
            double fx = 1 / 594.214;
            double fy = 1 / 591.0405;
            rectX = (x - cx) * z * fx;
            //rectY = (y - cy) * z * fy;
            rectY = z;
        }
    }
}
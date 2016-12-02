using System.Runtime.CompilerServices;
using System.Windows.Media;
using System.Windows.Media.Media3D;


namespace SLAM.Models.Old.DataProcessors {

    internal sealed class TopDepthFrameProcessor : DataProcessor, IDataProcessor {

        private Color viewportCurveColor;

        private int middleLineX, middleLineY;
        private int startIndex, endIndex;

        private Point3D[] middleHorizontalLineFromPointCloud;        

        public TopDepthFrameProcessor(DepthFrameSequenceInfo frameInfo) : base(frameInfo) {
            Initialize();
            InitializePoints3DBuffer();
        }

        private void Initialize() {
            viewportCurveColor = Color.FromArgb(255, 255, 192, 128);

            middleLineX = FrameInfo.Width / 2;
            middleLineY = FrameInfo.Height / 2;           
            startIndex = GetLinearIndex(0, middleLineY, FrameInfo.Width);
            endIndex = GetLinearIndex(FrameInfo.Width - 1, middleLineY, FrameInfo.Width);
        }

        private void InitializePoints3DBuffer() {
            middleHorizontalLineFromPointCloud = new Point3D[FrameInfo.Width];
            for (int i = 0; i < FrameInfo.Width; ++i) {
                middleHorizontalLineFromPointCloud[i].X = i;
                middleHorizontalLineFromPointCloud[i].Y = middleLineY;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void UpdatePointCloudMiddleLineFromRawInput(byte[] rawInput) {
            for (int i = startIndex; i < endIndex; ++i) {
                short depth = GetDepthFromRawFrameAt(rawInput, i * sizeof(short));
                middleHorizontalLineFromPointCloud[i - startIndex].Z = depth;
            }
        }

        public override void CalculateViewportFrame(byte[] rawInput, byte[] viewportOutput) {

            UpdatePointCloudMiddleLineFromRawInput(rawInput);

            for (int i = 0; i < FrameInfo.Width; ++i) {

                double x = middleHorizontalLineFromPointCloud[i].X; //   0 - 639
                double y = middleHorizontalLineFromPointCloud[i].Y; // 240 - 240
                double z = middleHorizontalLineFromPointCloud[i].Z; // 800 - 4000

                if (z < FrameInfo.MinDepth || z > FrameInfo.MaxDepth) { continue; }

                double resultX, resultY, resultZ;
                PerspectiveToRectangle(x, y, z, out resultX, out resultY, out resultZ);

                // --- Convert to viewport ---

                int imageX = (int)resultX + 320; // shift right on 1/2 Frame-Width
                int imageY = 480 - (int)resultY; // shift  down on  1  Frame-Height and flip vertical

                int resultLinearIndex = GetLinearIndex(imageX * sizeof(int), imageY, FrameInfo.Width * sizeof(int));

                SetColorToViewportByteArray(viewportOutput, resultLinearIndex, viewportCurveColor);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void PerspectiveToRectangle(double x, double y, double z, out double resultX, out double resultY, out double resultZ) {

            z *= 0.1; double factor = (0.003501 * 0.5) * z;

            resultX = (x - 320) * factor;
            resultY = z;
            resultZ = y * factor;
        }
    }
}
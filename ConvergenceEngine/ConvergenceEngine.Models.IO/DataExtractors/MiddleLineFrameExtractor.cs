using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Windows;

namespace ConvergenceEngine.Models.IO.DataExtractors {

    internal sealed class MiddleLineFrameExtractor {

        private readonly int frameWidth, frameHeight;
        private readonly int frameMinDepth, frameMaxDepth;

        internal MiddleLineFrameExtractor(FrameSequenceInfo frameInfo) {
            frameWidth = frameInfo.Width;
            frameHeight = frameInfo.Height;
            frameMinDepth = frameInfo.MinDepth;
            frameMaxDepth = frameInfo.MaxDepth;
        }

        internal MiddleLineFrameExtractor(int width, int height, int minDepth, int maxDepth) {
            frameWidth = width;
            frameHeight = height;
            frameMinDepth = minDepth;
            frameMaxDepth = maxDepth;
        }

        internal Point[] ExtractMiddleLine(byte[] rawFrameBuffer) {

            Point[] result = new Point[frameWidth];

            int i = 0;
            foreach (var nextPoint in AdaptedSequenceFrom(rawFrameBuffer)) {
                result[i] = nextPoint; ++i;
            }
            if (i < frameWidth) {
                TruncateResultSequence(ref result, i);
            }
            return result;
        }

        private void TruncateResultSequence(ref Point[] pointSequence, int newLength) { // compress
            Point[] result = new Point[newLength];
            for (int i = 0; i < newLength; ++i) {
                result[i] = pointSequence[i];
            }
            pointSequence = result;
        }

        private IEnumerable<Point> AdaptedSequenceFrom(byte[] rawFrameBuffer) {

            Point bufferedPoint = new Point(0.0, 0.0);
            int middleLineX = frameWidth / 2;
            int middleLineY = frameHeight / 2;
            int offset = GetLinearIndex(0, middleLineY, frameWidth);

            for (int i = 0; i < frameWidth; ++i) {

                double x = i;                                                                    //   0 ... 639
                double y = middleLineY;                                                          // 240 ... 240
                double z = GetDepthFromRawFrameAt(rawFrameBuffer, (i + offset) * sizeof(short)); // 800 ... 4000

                if (z < frameMinDepth || z > frameMaxDepth) { continue; }

                double resultX, resultY, resultZ;
                PerspectiveToRectangle(x, y, z, out resultX, out resultY, out resultZ);

                bufferedPoint.X = resultX; // -320 ... 319
                bufferedPoint.Y = resultY; //    0 ... 480

                yield return bufferedPoint;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private short GetDepthFromRawFrameAt(byte[] array, int index) {
            short result = array[index], lowByte = array[index + 1];
            result <<= 8; result |= lowByte;                         // <-- depth short construct
            return result >>= 3;                                     // <-- remove 3 unused low bits & return
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int GetLinearIndex(int x, int y, int width) {
            return width * y + x;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void PerspectiveToRectangle(double x, double y, double z, out double resultX, out double resultY, out double resultZ) {

            z *= 0.1; double factor = (0.003501 * 0.5) * z;

            resultX = (x - 320.0) * factor;
            resultY = z;
            resultZ = y * factor;
        }
    }
}
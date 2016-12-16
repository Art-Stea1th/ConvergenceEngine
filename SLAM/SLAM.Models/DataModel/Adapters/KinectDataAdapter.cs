using System;
using System.Windows;
using SLAM.Models.DataModel.Readers;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace SLAM.Models.DataModel.Adapters {

    internal sealed class KinectDataAdapter : IDataAdapter {        

        public DataProvider DataProvider { get; private set; }

        public KinectDataAdapter(DataProvider dataProvider) {
            DataProvider = dataProvider;
        }

        public Point[] GetAdapted(byte[] rawFrameBuffer) {

            Point[] result = new Point[DataProvider.FrameInfo.Width];

            int i = 0;
            foreach (var nextPoint in AdaptedSequenceFrom(rawFrameBuffer)) {
                result[i] = nextPoint; ++i;
            }
            if (i < DataProvider.FrameInfo.Width) {
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
            int middleLineX = DataProvider.FrameInfo.Width / 2;
            int middleLineY = DataProvider.FrameInfo.Height / 2;
            int offset = GetLinearIndex(0, middleLineY, DataProvider.FrameInfo.Width);

            for (int i = 0; i < DataProvider.FrameInfo.Width; ++i) {

                double x = i;                                                                    //   0 - 639
                double y = middleLineY;                                                          // 240 - 240
                double z = GetDepthFromRawFrameAt(rawFrameBuffer, (i + offset) * sizeof(short)); // 800 - 4000

                if (z < DataProvider.FrameInfo.MinDepth || z > DataProvider.FrameInfo.MaxDepth) { continue; }

                double resultX, resultY, resultZ;
                PerspectiveToRectangle(x, y, z, out resultX, out resultY, out resultZ);

                bufferedPoint.X = resultX; // -320 - 319
                bufferedPoint.Y = resultY; //    0 - 480

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
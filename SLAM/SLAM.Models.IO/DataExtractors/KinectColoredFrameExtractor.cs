using System.Runtime.CompilerServices;
using System.Windows.Media;

namespace SLAM.Models.IO.DataExtractors {

    using Readers;

    internal sealed class KinectColoredFrameExtractor {

        private DataProvider dataProvider;

        private bool thisInitialized;

        private Color nearColor;
        private Color farColor;
        private Color[] intensity;

        private byte[] frontDepthFrameBuffer;

        public KinectColoredFrameExtractor(DataProvider dataProvider) {
            this.dataProvider = dataProvider;
        }

        internal byte[] ExtractActualColoredDepthFrame(Color nearColor, Color farColor) {

            Initialize(nearColor, farColor);

            byte[] nextRawFrameData;
            dataProvider.GetNextRawFrameTo(out nextRawFrameData);

            int colorPixelIndex = 0;
            for (int i = 0; i < dataProvider.FrameInfo.Length; ++i) {

                short depth = GetDepthFromRawFrameAt(nextRawFrameData, i * sizeof(short));

                if (depth < dataProvider.FrameInfo.MinDepth) {
                    SetColorToViewportByteArray(
                        frontDepthFrameBuffer, colorPixelIndex, nearColor);
                }
                else if (depth > dataProvider.FrameInfo.MaxDepth) {
                    SetColorToViewportByteArray(
                        frontDepthFrameBuffer, colorPixelIndex, farColor);
                }
                else {
                    SetColorToViewportByteArray(
                        frontDepthFrameBuffer, colorPixelIndex, intensity[depth - dataProvider.FrameInfo.MinDepth]);
                }
                colorPixelIndex += sizeof(int);
            }
            return frontDepthFrameBuffer;
        }

        private void Initialize(Color nearColor, Color farColor) {
            if (!thisInitialized) {
                InitializeColorBuffers(nearColor, farColor);
                InitializeViewportsBuffers();
                thisInitialized = true;
            }
        }

        private void InitializeColorBuffers(Color nearColor, Color farColor) {

            this.nearColor = nearColor;
            this.farColor = farColor;

            double intencityStep = 192.0 / dataProvider.FrameInfo.DepthRange;

            intensity = new Color[dataProvider.FrameInfo.DepthRange];
            for (int i = 0; i < intensity.Length; ++i) {
                byte colorComponent = (byte)(byte.MaxValue - (i * intencityStep));
                intensity[i] = Color.FromArgb(255, colorComponent, colorComponent, colorComponent);
            }

        }

        private void InitializeViewportsBuffers() {
            frontDepthFrameBuffer = new byte[dataProvider.FrameInfo.Length * sizeof(int)];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int GetLinearIndex(int x, int y, int width) {
            return width * y + x;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private short GetDepthFromRawFrameAt(byte[] array, int index) {
            short result = array[index], lowByte = array[index + 1];
            result <<= 8; result |= lowByte;                         // <-- depth short construct
            return result >>= 3;                                     // <-- remove 3 unused low bits & return
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SetColorToViewportByteArray(byte[] viewportByteArray, int startIndex, Color color) {
            viewportByteArray[startIndex] = color.B;
            viewportByteArray[++startIndex] = color.G;
            viewportByteArray[++startIndex] = color.R;
            viewportByteArray[++startIndex] = color.A;
        }
    }
}
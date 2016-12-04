using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;


namespace SLAM.Models {

    using Data.Readers;    

    internal sealed class FramesConverter {

        private DataProvider dataProvider;

        private bool thisInitialized;

        private Color nearColor;
        private Color farColor;
        private Color[] intensity;

        //private byte[] fullMapFrameBuffer;
        private byte[] topDepthFrameBuffer;
        private byte[] frontDepthFrameBuffer;

        public FramesConverter(DataProvider dataProvider) {
            this.dataProvider = dataProvider;
            thisInitialized = false;
        }

        private void Initialize() {
            if (!thisInitialized) {
                InitializeColorBuffers();
                InitializeViewportsBuffers();
                thisInitialized = true;
            }
        }

        private void InitializeViewportsBuffers() {
            //fullMapFrameBuffer = new byte[dataProvider.FrameInfo.Length * sizeof(int)]; ??
            //topDepthFrameBuffer = new byte[dataProvider.FrameInfo.Length * sizeof(int)]; // need to reallocate for correct redraw !
            frontDepthFrameBuffer = new byte[dataProvider.FrameInfo.Length * sizeof(int)];
        }

        private void InitializeColorBuffers() {

            nearColor = Color.FromArgb(255, 0, 128, 192);
            farColor = Color.FromArgb(255, 0, 0, 30);

            double intencityStep = 192.0 / dataProvider.FrameInfo.DepthRange;

            intensity = new Color[dataProvider.FrameInfo.DepthRange];
            for (int i = 0; i < intensity.Length; ++i) {
                byte colorComponent = (byte)(byte.MaxValue - (i * intencityStep));
                intensity[i] = Color.FromArgb(255, colorComponent, colorComponent, colorComponent);
            }
            
        }

        internal byte[] GetActualMapFrame() {
            Initialize();

            return topDepthFrameBuffer;
            //return fullMapFrameBuffer;
        }

        internal byte[] GetActualTopDepthFrame() {
            Initialize();

            Point[] nextFrameData;
            dataProvider.GetNextFrameTo(out nextFrameData);

            topDepthFrameBuffer = new byte[dataProvider.FrameInfo.Length * sizeof(int)];

            foreach (var point in nextFrameData) {
                int index = GetLinearIndex((int)point.X, (int)point.Y, dataProvider.FrameInfo.Width);
                SetColorToViewportByteArray(topDepthFrameBuffer, index * sizeof(int), nearColor);
            }
            return topDepthFrameBuffer;
        }

        internal byte[] GetActualFrontDepthFrame() {
            Initialize();

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
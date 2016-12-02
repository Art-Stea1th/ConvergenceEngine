using System.Runtime.CompilerServices;
using System.Windows.Media;

namespace SLAM.Models.Old.DataProcessors {

    internal abstract class DataProcessor : IDataProcessor {

        public DepthFrameSequenceInfo FrameInfo { get; private set; }

        internal protected DataProcessor(DepthFrameSequenceInfo frameInfo) {
            FrameInfo = frameInfo;
        }

        public abstract void CalculateViewportFrame(byte[] rawInput, byte[] viewportOutput);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected int GetLinearIndex(int x, int y, int width) {
            return width * y + x;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected short GetDepthFromRawFrameAt(byte[] array, int index) {
            short result = array[index], lowByte = array[index + 1];
            result <<= 8; result |= lowByte;                         // <-- depth short construct
            return result >>= 3;                                     // <-- remove 3 unused low bits & return
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void SetColorToViewportByteArray(byte[] viewportByteArray, int startIndex, Color color) {
            viewportByteArray[startIndex]   = color.B;
            viewportByteArray[++startIndex] = color.G;
            viewportByteArray[++startIndex] = color.R;
            viewportByteArray[++startIndex] = color.A;
        }
    }
}
using System;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace SLAM.Models.Converters {

    internal abstract class DataConverter : IDataConverter {

        public DepthFrameSequenceInfo FrameInfo { get; private set; }

        internal protected DataConverter(DepthFrameSequenceInfo frameInfo) {
            FrameInfo = frameInfo;
        }

        public abstract void ConvertRawDataToViewportFrame(byte[] rawInput, byte[] viewportOutput);

        // Horizontal Mirror RAW Frame and swap [hi <--> low] bytes for each Int16\short depth
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void HorizontalMirror(byte[] frame) {

            int width  = FrameInfo.Width * sizeof(short);
            int height = FrameInfo.Height;

            for (int y = 0; y < height; ++y) {
                for (int x = 0; x < width / 2; ++x) {

                    int linearIndexNormal = GetLinearIndex(x, y, width);
                    int linearIndexInverse = GetLinearIndex((width - 1 - x), y, width);

                    byte tmp = frame[linearIndexNormal];
                    frame[linearIndexNormal] = frame[linearIndexInverse];
                    frame[linearIndexInverse] = tmp;
                }
            }
        }

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


        // --- Math Magic --- It is not yet used ---

        private double AngleBetweenRays(Point3D ray1, Point3D ray2) {
            double dotProduct = ray1.X * ray2.X + ray1.Y * ray2.Y + ray1.X * ray2.Z;
            double magnitude1 = MagnitudeOfRay(ray1);
            double magnitude2 = MagnitudeOfRay(ray2); ;
            return Math.Acos(dotProduct / (magnitude1 * magnitude2));
        }

        private double AngleBetweenRays(double x1, double y1, double z1, double x2, double y2, double z2) {
            double dotProduct = x1 * x2 + y1 * y2 + x1 * z2;
            double magnitude1 = MagnitudeOfRay(x1, y1, z1);
            double magnitude2 = MagnitudeOfRay(x2, y2, z2);
            return Math.Acos(dotProduct / (magnitude1 * magnitude2));
        }

        private double MagnitudeOfRay(Point3D ray) {
            return Math.Sqrt(Math.Pow(ray.X, 2.0) + Math.Pow(ray.Y, 2.0) + Math.Pow(ray.Z, 2.0));
        }

        private double MagnitudeOfRay(double x, double y, double z) {
            return Math.Sqrt(Math.Pow(x, 2.0) + Math.Pow(y, 2.0) + Math.Pow(z, 2.0));
        }
    }
}
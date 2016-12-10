using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SLAM.Views.Controls.Custom {

    internal sealed class PointRenderer {

        private Point min, max;

        internal double MinX { get { return min.X; } }
        internal double MinY { get { return min.Y; } }

        internal double MaxX { get { return max.X; } }
        internal double MaxY { get { return max.Y; } }

        internal double OffsetX { get; private set; }
        internal double OffsetY { get; private set; }

        internal ImageSource Render(Point[] newPoints, Color newColor) {

            if (newPoints == null || newPoints.Length < 1) { return null; }

            UpdateLimits(newPoints);
            return CreateBitmap(newPoints, newColor);
        }

        private void UpdateLimits(Point[] points) {

            min = new Point(points[0].X, points[0].Y);
            max = new Point(points[0].X, points[0].Y);

            foreach (var point in points) {

                if (point.X > max.X) { max.X = point.X; }
                else
                if (point.X < min.X) { min.X = point.X; }

                if (point.Y > max.Y) { max.Y = point.Y; }
                else
                if (point.Y < min.Y) { min.Y = point.Y; }
            }
            OffsetX = -min.X;
            OffsetY = -min.Y;
        }

        private ImageSource CreateBitmap(Point[] points, Color color) {

            Point localMin = min, localMax = max;

            localMin.Offset(OffsetX, OffsetY);
            localMax.Offset(OffsetX, OffsetY);

            if (localMax.X < 1 || localMax.Y < 1) { return null; }

            int width = (int)localMax.X + 1;
            int height = (int)localMax.Y + 1;

            WriteableBitmap result = new WriteableBitmap(width, height, 96.0, 96.0, PixelFormats.Bgr32, null);
            byte[] fullFrameBuffer = new byte[width * height * sizeof(int)];

            foreach (var point in points) {
                point.Offset(OffsetX, OffsetY);
                int index = GetLinearIndex((int)point.X, (int)point.Y, width);
                SetColorToViewportByteArray(fullFrameBuffer, index * sizeof(int), color);
            }

            result.WritePixels(new Int32Rect(0, 0, width, height), fullFrameBuffer, result.PixelWidth * sizeof(int), 0);
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int GetLinearIndex(int x, int y, int width) {
            return width * y + x;
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
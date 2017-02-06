using System;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SLAM.Views.Controls.Custom {

    internal sealed class PointRenderer {

        private Point min, max;
        private int width, height;

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

            max = new Point(0, 0);

            foreach (var point in points) {
                double
                    x = Math.Abs(point.X),
                    y = Math.Abs(point.Y);

                if (x > max.X) { max.X = x; }
                if (y > max.Y) { max.Y = y; }
            }

            min = new Point(-max.X, -max.Y);

            OffsetX = max.X + 1;
            OffsetY = max.Y + 1;

            width = ((int)OffsetX) * 2 + 1;
            height = ((int)OffsetY) /** 2 + 1*/;
        }

        private ImageSource CreateBitmap(Point[] points, Color color) {

            if (max.X < 1 || max.Y < 1) { return null; }

            WriteableBitmap result = new WriteableBitmap(width, height, 96.0, 96.0, PixelFormats.Bgr32, null);
            byte[] fullFrameBuffer = new byte[width * height * sizeof(int)];

            foreach (var point in points) {
                point.Offset(OffsetX, /*OffsetY*/0);
                int index = GetLinearIndex((int)point.X, height - ((int)point.Y), width);
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
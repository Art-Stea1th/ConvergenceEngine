using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SLAM.Views.Controls.Custom {

    internal sealed class PointRenderer {

        internal ImageSource TmpRenderMethod(Point[] points) {

            if (points == null || points.Length < 1) { return null; }

            Point min = new Point(points[0].X, points[0].Y);
            Point max = new Point(points[0].X, points[0].Y);

            foreach (var point in points) {
                if (point.X < min.X) { min.X = point.X; }
                if (point.Y < min.Y) { min.Y = point.Y; }

                if (point.X > max.X) { max.X = point.X; }
                if (point.Y > max.Y) { max.Y = point.Y; }
            }

            double offsetX = -min.X, offsetY = -min.Y;

            min.Offset(offsetX, offsetY);
            max.Offset(offsetX, offsetY);

            if (max.X < 1 || max.Y < 1) {
                return null;
            }

            int width = (int)max.X + 1;
            int height = (int)max.Y + 1;

            WriteableBitmap resultSource
                = new WriteableBitmap(width, height, 96.0, 96.0, PixelFormats.Bgr32, null);

            byte[] fullFrameBuffer = new byte[width * height * sizeof(int)];

            foreach (var point in points) {
                point.Offset(offsetX, offsetY);
                int index = GetLinearIndex((int)point.X, (int)point.Y, width);
                SetColorToViewportByteArray(fullFrameBuffer, index * sizeof(int), Color.FromArgb(0, 255, 0, 0));
            }

            resultSource.WritePixels(
                new Int32Rect(0, 0, width, height),
                fullFrameBuffer, resultSource.PixelWidth * sizeof(int), 0);

            return resultSource;
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
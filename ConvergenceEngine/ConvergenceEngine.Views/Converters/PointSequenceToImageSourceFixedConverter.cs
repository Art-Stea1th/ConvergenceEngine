using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ConvergenceEngine.Views.Converters {

    [ValueConversion(typeof(IEnumerable<Point>), typeof(ImageSource))]
    public class PointSequenceToImageSourceFixedConverter : IValueConverter {

        private const int width = 640, height = 480, offsetX = width / 2, offsetY = 0;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            IEnumerable<Point> newPoints = value as IEnumerable<Point>;

            Color newColor = (parameter as SolidColorBrush).Color;

            if (newPoints == null || newPoints.Count() < 1) { return null; }

            return CreateBitmap(newPoints, newColor);
        }

        private ImageSource CreateBitmap(IEnumerable<Point> points, Color color) {

            WriteableBitmap result = new WriteableBitmap(width, height, 96.0, 96.0, PixelFormats.Bgr32, null);
            byte[] fullFrameBuffer = new byte[width * height * sizeof(int)];

            foreach (var point in points) {
                point.Offset(offsetX, offsetY);
                int index = GetLinearIndex((int)point.X, (height - 1) - ((int)point.Y), width);
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

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotSupportedException();
        }
    }
}
using System;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ConvergenceEngine.Views.AppCustomControls {

    [TemplatePart(Name = FullFrameViewport.PartImageName, Type = typeof(Image))]
    public class FullFrameViewport : Control {

        public Color NearColor {
            get { return (Color)GetValue(nearColorProperty); }
            set { SetValue(nearColorProperty, value); }
        }

        public Color FarColor {
            get { return (Color)GetValue(farColorProperty); }
            set { SetValue(farColorProperty, value); }
        }

        public short[,] Data {
            get { return (short[,])GetValue(dataProperty); }
            set { SetValue(dataProperty, value); }
        }

        public double DataMinValue {
            get { return (double)GetValue(dataMinValueProperty); }
            set { SetValue(dataMinValueProperty, value); }
        }

        public double DataMaxValue {
            get { return (double)GetValue(dataMaxValueProperty); }
            set { SetValue(dataMaxValueProperty, value); }
        }

        public static readonly DependencyProperty nearColorProperty;
        public static readonly DependencyProperty farColorProperty;

        public static readonly DependencyProperty dataProperty;
        public static readonly DependencyProperty dataMinValueProperty;
        public static readonly DependencyProperty dataMaxValueProperty;

        static FullFrameViewport() {

            DefaultStyleKeyProperty
                .OverrideMetadata(typeof(FullFrameViewport), new FrameworkPropertyMetadata(typeof(FullFrameViewport)));

            nearColorProperty = DependencyProperty.Register("NearColor", typeof(Color), typeof(FullFrameViewport),
                new FrameworkPropertyMetadata(Color.FromArgb(255, 0, 128, 192), FrameworkPropertyMetadataOptions.AffectsRender));

            farColorProperty = DependencyProperty.Register("FarColor", typeof(Color), typeof(FullFrameViewport),
                new FrameworkPropertyMetadata(Color.FromArgb(255, 0, 0, 30), FrameworkPropertyMetadataOptions.AffectsRender));

            dataProperty = DependencyProperty.Register("Data", typeof(short[,]), typeof(FullFrameViewport),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

            dataMinValueProperty = DependencyProperty.Register("DataMinValue", typeof(double), typeof(FullFrameViewport),
                new FrameworkPropertyMetadata(800.0, FrameworkPropertyMetadataOptions.AffectsRender, null, CoerceDataMinMaxValue));

            dataMaxValueProperty = DependencyProperty.Register("DataMaxValue", typeof(double), typeof(FullFrameViewport),
                new FrameworkPropertyMetadata(4000.0, FrameworkPropertyMetadataOptions.AffectsRender, null, CoerceDataMinMaxValue));
        }

        private const string PartImageName = "PART_Image";
        private Image image;

        private Color[] intensityBuffer;
        private byte[] frameBuffer;

        private static object CoerceDataMinMaxValue(DependencyObject o, object value) {
            var result = (double)value;
            return result < short.MinValue ? short.MinValue : result > short.MaxValue ? short.MaxValue : result;
        }

        public override void OnApplyTemplate() {
            image = GetTemplateChild(PartImageName) as Image;
            intensityBuffer = GenerateIntensityBuffer();
        }

        protected override void OnRender(DrawingContext drawingContext) {
            UpdateImageData();
        }

        private void UpdateImageData() {

            if (Data == null) {
                image.Source = NewBitmap(4, 3);
                return;
            }

            PrepareFrameBuffer();

            if (frameBuffer == null) {
                image.Source = NewBitmap(4, 3);
                return;
            }

            int width = Data.GetLength(0), height = Data.GetLength(1);

            var bitmap = NewBitmap(width, height);
            bitmap.WritePixels(new Int32Rect(0, 0, width, height), frameBuffer, width * sizeof(int), 0);

            bitmap.Freeze();
            image.Source = bitmap;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private WriteableBitmap NewBitmap(int width, int height) {
            return new WriteableBitmap(width > 1 ? width : 1, height > 1 ? height : 1, 96.0, 96.0, PixelFormats.Bgr32, null);
        }

        private void PrepareFrameBuffer() {

            if (Data != null) {

                int width = Data.GetLength(0), height = Data.GetLength(1);
                int length = width * height * sizeof(int);

                if (frameBuffer == null || frameBuffer.Length != length) {
                    frameBuffer = new byte[length];
                }

                for (int y = 0; y < height; ++y) {
                    for (int x = 0; x < width; ++x) {

                        short depth = Data[x, y];
                        int colorPixelIndex = GetLinearIndex(x * sizeof(int), y, width * sizeof(int));

                        if (depth < DataMinValue) {
                            SetColorToFrameBuffer(colorPixelIndex, NearColor);
                        }
                        else if (depth > DataMaxValue) {
                            SetColorToFrameBuffer(colorPixelIndex, FarColor);
                        }
                        else {
                            SetColorToFrameBuffer(colorPixelIndex, intensityBuffer[depth - (short)DataMinValue]);
                        }
                    }
                }
            }
        }

        private Color[] GenerateIntensityBuffer() {

            double depthRange = Math.Abs(DataMaxValue - DataMinValue);
            double intencityStep = 192.0 / depthRange;

            intensityBuffer = new Color[(int)depthRange];
            for (int i = 0; i < intensityBuffer.Length; ++i) {
                byte colorComponent = (byte)(byte.MaxValue - (i * intencityStep));
                intensityBuffer[i] = Color.FromRgb(colorComponent, colorComponent, colorComponent);
            }
            return intensityBuffer;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int GetLinearIndex(int x, int y, int width) {
            return width * y + x;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SetColorToFrameBuffer(int startIndex, Color color) {
            frameBuffer[startIndex] = color.B;
            frameBuffer[++startIndex] = color.G;
            frameBuffer[++startIndex] = color.R;
        }
    }
}
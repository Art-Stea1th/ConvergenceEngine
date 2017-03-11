using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ConvergenceEngine.Views.AppCustomControls {

    [TemplatePart(Name = FullFrameViewport.PartImageName, Type = typeof(Image))]
    public class FullFrameViewport : Control {

        public Color NearColor {
            get { return (Color)GetValue(NearColorProperty); }
            set { SetValue(NearColorProperty, value); }
        }

        public Color FarColor {
            get { return (Color)GetValue(FarColorProperty); }
            set { SetValue(FarColorProperty, value); }
        }

        public short[,] Data {
            get { return (short[,])GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        public static readonly DependencyProperty NearColorProperty;
        public static readonly DependencyProperty FarColorProperty;

        public static readonly DependencyProperty DataProperty;

        static FullFrameViewport() {

            DefaultStyleKeyProperty
                .OverrideMetadata(typeof(FullFrameViewport), new FrameworkPropertyMetadata(typeof(FullFrameViewport)));

            NearColorProperty = DependencyProperty.Register("NearColor", typeof(Color), typeof(FullFrameViewport),
                new FrameworkPropertyMetadata(Colors.White, (s, e) => (s as FullFrameViewport).UpdateImageData()));

            FarColorProperty = DependencyProperty.Register("FarColor", typeof(Color), typeof(FullFrameViewport),
                new FrameworkPropertyMetadata(Colors.Black, (s, e) => (s as FullFrameViewport).UpdateImageData()));

            DataProperty = DependencyProperty.Register("Data", typeof(short[,]), typeof(FullFrameViewport),
                new FrameworkPropertyMetadata(null, (s, e) => (s as FullFrameViewport).UpdateImageData()));
        }

        private const string PartImageName = "PART_Image";
        private Image image;

        private Color[] intensityBuffer;
        private byte[] frameBuffer;

        private int width = 4;
        private int height = 3;

        private short minDepth = 800;
        private short maxDepth = 4000;

        public override void OnApplyTemplate() {
            image = GetTemplateChild(PartImageName) as Image;
            intensityBuffer = GenerateIntensityBuffer();
            UpdateImageData();
        }

        private void UpdateImageData() {

            if (image == null) {
                return;
            }

            if (Data == null) {
                image.Source = NewBitmap(width, height);
                return;
            }

            PrepareFrameBuffer();

            if (frameBuffer == null) {
                image.Source = NewBitmap(width, height);
                return;
            }

            var bitmap = NewBitmap(width, height);
            bitmap.WritePixels(new Int32Rect(0, 0, width, height), frameBuffer, width * sizeof(int), 0);

            bitmap.Freeze();
            image.Source = bitmap;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private WriteableBitmap NewBitmap(int width, int height) {
            return new WriteableBitmap(width > 1 ? width : 1, height > 1 ? height : 1, 96.0, 96.0, PixelFormats.Bgra32, null);
        }

        private void PrepareFrameBuffer() {

            if (Data != null) {

                width = Data.GetLength(0);
                height = Data.GetLength(1);
                int length = width * height * sizeof(int);

                if (frameBuffer == null || frameBuffer.Length != length) {
                    frameBuffer = new byte[length];
                }

                for (int y = 0; y < height; ++y) {
                    for (int x = 0; x < width; ++x) {

                        short depth = Data[x, y];
                        int colorPixelIndex = GetLinearIndex(x * sizeof(int), y, width * sizeof(int));

                        if (depth < minDepth) {
                            SetColorToFrameBuffer(colorPixelIndex, NearColor);
                        }
                        else if (depth > maxDepth) {
                            SetColorToFrameBuffer(colorPixelIndex, FarColor);
                        }
                        else {
                            SetColorToFrameBuffer(colorPixelIndex, intensityBuffer[maxDepth - depth]);
                        }
                    }
                }
            }
        }

        private Color[] GenerateIntensityBuffer() {

            int depthRange = maxDepth - minDepth;
            double intencityStep = 255.0 / depthRange;

            intensityBuffer = new Color[depthRange];
            for (int i = 0; i < intensityBuffer.Length; ++i) {
                byte colorComponent = (byte)((i * intencityStep));
                intensityBuffer[i] = Color.FromArgb(255, colorComponent, colorComponent, colorComponent);
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
            frameBuffer[++startIndex] = color.A;
        }
    }
}
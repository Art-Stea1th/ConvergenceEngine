using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ConvergenceEngine.Views.AppCustomControls {

    [TemplatePart(Name = PartImageName, Type = typeof(Image))]
    public class FullFrameViewport : Control {

        public const string PartImageName = "PART_Image";

        public Color NearColor {
            get => (Color)GetValue(NearColorProperty);
            set => SetValue(NearColorProperty, value);
        }

        public Color FarColor {
            get => (Color)GetValue(FarColorProperty);
            set => SetValue(FarColorProperty, value);
        }

        public short[,] Data {
            get => (short[,])GetValue(DataProperty);
            set => SetValue(DataProperty, value);
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

        private Image _image;

        private Color[] _intensityBuffer;
        private byte[] _frameBuffer;

        private int _width = 4;
        private int _height = 3;

        private short _minDepth = 800;
        private short _maxDepth = 4000;

        public override void OnApplyTemplate() {
            _image = GetTemplateChild(PartImageName) as Image;
            _intensityBuffer = GenerateIntensityBuffer();
            UpdateImageData();
        }

        private void UpdateImageData() {

            if (_image == null) {
                return;
            }

            if (Data == null) {
                _image.Source = NewBitmap(_width, _height);
                return;
            }

            PrepareFrameBuffer();

            if (_frameBuffer == null) {
                _image.Source = NewBitmap(_width, _height);
                return;
            }

            var bitmap = NewBitmap(_width, _height);
            bitmap.WritePixels(new Int32Rect(0, 0, _width, _height), _frameBuffer, _width * sizeof(int), 0);

            bitmap.Freeze();
            _image.Source = bitmap;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private WriteableBitmap NewBitmap(int width, int height) {
            return new WriteableBitmap(width > 1 ? width : 1, height > 1 ? height : 1, 96.0, 96.0, PixelFormats.Bgra32, null);
        }

        private void PrepareFrameBuffer() {

            if (Data != null) {

                _width = Data.GetLength(0);
                _height = Data.GetLength(1);
                int length = _width * _height * sizeof(int);

                if (_frameBuffer == null || _frameBuffer.Length != length) {
                    _frameBuffer = new byte[length];
                }

                for (int y = 0; y < _height; ++y) {
                    for (int x = 0; x < _width; ++x) {

                        short depth = Data[x, y];
                        int colorPixelIndex = GetLinearIndex(x * sizeof(int), y, _width * sizeof(int));

                        if (depth < _minDepth) {
                            SetColorToFrameBuffer(colorPixelIndex, NearColor);
                        }
                        else if (depth > _maxDepth) {
                            SetColorToFrameBuffer(colorPixelIndex, FarColor);
                        }
                        else {
                            SetColorToFrameBuffer(colorPixelIndex, _intensityBuffer[_maxDepth - depth]);
                        }
                    }
                }
            }
        }

        private Color[] GenerateIntensityBuffer() {

            int depthRange = _maxDepth - _minDepth;
            double intencityStep = 255.0 / depthRange;

            _intensityBuffer = new Color[depthRange];
            for (int i = 0; i < _intensityBuffer.Length; ++i) {
                byte colorComponent = (byte)((i * intencityStep));
                _intensityBuffer[i] = Color.FromArgb(255, colorComponent, colorComponent, colorComponent);
            }
            return _intensityBuffer;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int GetLinearIndex(int x, int y, int width) => width * y + x;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SetColorToFrameBuffer(int startIndex, Color color) {
            _frameBuffer[startIndex] = color.B;
            _frameBuffer[++startIndex] = color.G;
            _frameBuffer[++startIndex] = color.R;
            _frameBuffer[++startIndex] = color.A;
        }
    }
}
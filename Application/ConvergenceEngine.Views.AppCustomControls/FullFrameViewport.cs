using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ConvergenceEngine.Views.AppCustomControls {

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

        public short DataMinValue {
            get { return (short)GetValue(dataMaxValueProperty); }
            set { SetValue(dataMinValueProperty, value); }
        }

        public short DataMaxValue {
            get { return (short)GetValue(dataMaxValueProperty); }
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
                new FrameworkPropertyMetadata(Color.FromArgb(255, 0, 128, 192), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

            farColorProperty = DependencyProperty.Register("FarColor", typeof(Color), typeof(FullFrameViewport),
                new FrameworkPropertyMetadata(Color.FromArgb(255, 0, 0, 30), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

            dataProperty = DependencyProperty.Register("Data", typeof(Color), typeof(FullFrameViewport),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

            dataMinValueProperty = DependencyProperty.Register("DataMinValue", typeof(short), typeof(FullFrameViewport),
                new FrameworkPropertyMetadata(800, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

            dataMaxValueProperty = DependencyProperty.Register("DataMaxValue", typeof(short), typeof(FullFrameViewport),
                new FrameworkPropertyMetadata(4000, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        }

        private Color[] intensityBuffer;
        private byte[] frameBuffer;

        public Image ImageData { get; private set; }

        public override void OnApplyTemplate() {
            intensityBuffer = GenerateIntensityBuffer();
        }

        

        private Image UpdateImageData() {

            FillFrameBuffer();

            throw new NotImplementedException();
        }

        private void FillFrameBuffer() {

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
                            SetColorToFrameBuffer(colorPixelIndex, intensityBuffer[depth - DataMinValue]);
                        }
                    }
                }
            }
            frameBuffer = null;            
        }

        private Color[] GenerateIntensityBuffer() {

            double depthRange = Math.Abs(DataMaxValue - DataMinValue);
            double intencityStep = 192.0 / depthRange;

            intensityBuffer = new Color[(int)depthRange];
            for (int i = 0; i < intensityBuffer.Length; ++i) {
                byte colorComponent = (byte)(byte.MaxValue - (i * intencityStep));
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
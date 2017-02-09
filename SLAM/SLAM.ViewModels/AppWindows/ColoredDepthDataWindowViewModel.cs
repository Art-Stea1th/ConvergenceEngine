using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;


namespace SLAM.ViewModels.AppWindows {

    using Models.Mapping;

    internal class ColoredDepthDataWindowViewModel : ViewportWindowViewModel {

        private WriteableBitmap rawData;

        public ImageSource RawData {
            get { return rawData; }
            set { Set(ref rawData, (WriteableBitmap)value); }
        }

        internal ColoredDepthDataWindowViewModel() {
            Title = "RAW Data";
            Initialize();
        }

        public override void Initialize() {
            RawData = new WriteableBitmap(640, 480, 96.0, 96.0, PixelFormats.Bgr32, null);
        }

        public override void UpdateFrom(Model model) {
            byte[] rawPixels = model.GetActualColoredDepthFrame(Color.FromArgb(255, 0, 128, 192), Color.FromArgb(255, 0, 0, 30));

            if (rawPixels != null) {
                rawData.WritePixels(new Int32Rect(0, 0, 640, 480), rawPixels, rawData.PixelWidth * sizeof(int), 0);
            }
        }
    }
}
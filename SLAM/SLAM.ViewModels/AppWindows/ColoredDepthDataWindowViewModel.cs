using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SLAM.ViewModels.AppWindows {

    using Models.Mapping;

    public sealed class ColoredDepthDataWindowViewModel : ViewModelBase {

        private Model model;
        private WriteableBitmap rawData;

        public ImageSource RawData {
            get { return rawData; }
            set { Set(ref rawData, (WriteableBitmap)value); }
        }

        internal ColoredDepthDataWindowViewModel(Model model) {
            Initialize();
            this.model = model;
            model.OnModelUpdated += Update;
        }

        public void Initialize() {
            RawData = new WriteableBitmap(640, 480, 96.0, 96.0, PixelFormats.Bgr32, null);
        }

        public void Update() {
            byte[] rawPixels = model.GetActualColoredDepthFrame(Color.FromArgb(255, 0, 128, 192), Color.FromArgb(255, 0, 0, 30));

            if (rawPixels != null) {
                rawData = new WriteableBitmap(640, 480, 96.0, 96.0, PixelFormats.Bgr32, null);
                rawData.WritePixels(new Int32Rect(0, 0, 640, 480), rawPixels, rawData.PixelWidth * sizeof(int), 0);
                RawData.Freeze();
                RawData = rawData;
            }
        }
    }
}
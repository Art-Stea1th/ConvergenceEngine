using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SLAM.Views.Controls {

    public partial class RectangularCSViewport : Border, INotifyPropertyChanged {

        #region DependencyProperties // --------------------------------------------------------------------------------

        public Point[] Point2DVector {
            get { return (Point[])GetValue(Point2DVectorProperty); }
            set { SetValue(Point2DVectorProperty, value); }
        }
        public static readonly DependencyProperty Point2DVectorProperty;

        static RectangularCSViewport() {

            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(RectangularCSViewport), new FrameworkPropertyMetadata(typeof(RectangularCSViewport)));

            var point2DVectorPropertyMetadata = new FrameworkPropertyMetadata();
            point2DVectorPropertyMetadata.BindsTwoWayByDefault = true;
            point2DVectorPropertyMetadata.AffectsRender = true;

            Point2DVectorProperty =
                DependencyProperty.Register(
                    "Point2DVector", typeof(Point[]), typeof(RectangularCSViewport), point2DVectorPropertyMetadata);
        }

        #endregion // --------------------------------------------------------------------------------------------------

        #region INotifyPropertyChanged // ------------------------------------------------------------------------------

        private void Set<T>(ref T oldValue, T newValue, [CallerMemberName] string propertyName = null) {
            oldValue = newValue;
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion // --------------------------------------------------------------------------------------------------

        private WriteableBitmap viewportData;

        private int width;
        private int height;

        public ImageSource ViewportData {
            get { return viewportData; }
            set { Set(ref viewportData, (WriteableBitmap)value); }
        }
        public int Width {
            get { return width; }
            set { Set(ref width, value); }
        }
        public int Height {
            get { return height; }
            set { Set(ref height, value); }
        }        

        public RectangularCSViewport() {
            InitializeComponent();
        }

        protected override void OnRender(DrawingContext drawingContext) {
            if (Point2DVector == null) {
                return;
            }
            ReCalculateViewportDataSize();
        }

        private void ReCalculateViewportDataSize() {
            foreach (var point in Point2DVector) {

            }
        }
    }
}
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;


namespace SLAM.Views.CustomControls {

    [TemplatePart(Name = MathDataViewport.ViewportSurfaceName, Type = typeof(Image))]
    public class MathDataViewport : Control {

        #region DependencyProperties

        public static readonly DependencyProperty DataProperty;

        public Point[] Data {
            get { return (Point[])GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        static MathDataViewport() {

            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(MathDataViewport), new FrameworkPropertyMetadata(typeof(MathDataViewport)));

            var dataPropertyMetadata = new FrameworkPropertyMetadata();
            dataPropertyMetadata.DefaultValue = null;
            dataPropertyMetadata.BindsTwoWayByDefault = true;
            dataPropertyMetadata.AffectsRender = true;
            dataPropertyMetadata.PropertyChangedCallback = OnDataPropertyChangedCallback;

            DataProperty =
                DependencyProperty.Register(
                    "Data", typeof(Point[]), typeof(MathDataViewport), dataPropertyMetadata);
        }

        private static void OnDataPropertyChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            var viewport = sender as MathDataViewport;
            viewport.Data = (Point[])e.NewValue;
        }

        #endregion

        #region TemplateParts

        private const string ViewportSurfaceName = "PART_ViewportSurface";
        private Image viewportSurface;

        #endregion

        #region InnerData

        MathDataViewportRenderer renderer;

        #endregion

        public override void OnApplyTemplate() {
            renderer = new MathDataViewportRenderer();
            viewportSurface = GetTemplateChild(ViewportSurfaceName) as Image;
            if (viewportSurface != null) {
                viewportSurface.Width = 640.0;
                viewportSurface.Height = 480.0;
                OnRender(null);
            }
        }

        protected override void OnRender(DrawingContext drawingContext) {
            viewportSurface.Source = renderer.TmpRenderMethod(Data);
        }        
    }
}
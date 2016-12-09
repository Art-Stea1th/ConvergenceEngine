using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;


namespace SLAM.Views.Controls.Custom {

    [TemplatePart(Name = ViewportSurfaceName, Type = typeof(Image))]
    public class MathDataViewport : Control {

        #region TemplateParts

        private const string ViewportSurfaceName = "PART_ViewportSurface";
        private Image viewportSurface;

        #endregion

        #region DependencyProperties

        public static readonly DependencyProperty DataProperty;
        public static readonly DependencyProperty ProportionsProperty;

        public Point[] Data {
            get { return (Point[])GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        public Point Proportions {
            get { return (Point)GetValue(ProportionsProperty); }
            set { SetValue(ProportionsProperty, value); }
        }

        static MathDataViewport() {

            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(MathDataViewport), new FrameworkPropertyMetadata(typeof(MathDataViewport)));

            var dataPropertyMetadata =
                new FrameworkPropertyMetadata(null,
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault |
                FrameworkPropertyMetadataOptions.AffectsRender);

            var proportionsPropertyMetadata =
                new FrameworkPropertyMetadata(new Point(4.0, 3.0),
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault |
                FrameworkPropertyMetadataOptions.AffectsRender);

            DataProperty =
                DependencyProperty.Register(
                    "Data", typeof(Point[]), typeof(MathDataViewport), dataPropertyMetadata);

            ProportionsProperty =
                DependencyProperty.Register(
                    "Proportions", typeof(Point), typeof(MathDataViewport), proportionsPropertyMetadata);
        }

        #endregion        

        #region InnerData

        PointRenderer renderer;

        #endregion

        public override void OnApplyTemplate() {
            renderer = new PointRenderer();
            viewportSurface = GetTemplateChild(ViewportSurfaceName) as Image;
            if (viewportSurface != null) {
                OnRender(null);
            }
        }

        protected override void OnRender(DrawingContext drawingContext) {
            viewportSurface.Width = Proportions.X;
            viewportSurface.Height = Proportions.Y;
            viewportSurface.Source = renderer.TmpRenderMethod(Data);
        }        
    }
}
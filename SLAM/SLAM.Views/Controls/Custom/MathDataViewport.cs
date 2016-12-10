using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace SLAM.Views.Controls.Custom {

    [TemplatePart(Name = ViewportSurfaceName, Type = typeof(Image))]
    public sealed class MathDataViewport : Control {

        #region TemplateParts

        private const string ViewportSurfaceName = "PART_ViewportSurface";
        private const string XAxisName = "PART_Axis";

        private Image viewportSurface;
        private Path axis;

        #endregion

        #region DependencyProperties

        public static readonly DependencyProperty DataProperty;
        public static readonly DependencyProperty ProportionsProperty;
        public static readonly DependencyProperty AxisVisibleProperty;

        public static readonly DependencyProperty DataColorProperty;
        public static readonly DependencyProperty AxisColorProperty;

        public Point[] Data {
            get { return (Point[])GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        public Point Proportions {
            get { return (Point)GetValue(ProportionsProperty); }
            set { SetValue(ProportionsProperty, value); }
        }

        public bool AxisVisible {
            get { return (bool)GetValue(AxisVisibleProperty); }
            set { SetValue(AxisVisibleProperty, value); }
        }

        public Color DataColor {
            get { return (Color)GetValue(DataColorProperty); }
            set { SetValue(DataColorProperty, value); }
        }

        public Color AxisColor {
            get { return (Color)GetValue(AxisColorProperty); }
            set { SetValue(AxisColorProperty, value); }
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

            var axisVisiblePropertyMetadata =
                new FrameworkPropertyMetadata(true,
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault |
                FrameworkPropertyMetadataOptions.AffectsRender);

            var dataColorPropertyMetadata =
                new FrameworkPropertyMetadata(Colors.Gray,
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault |
                FrameworkPropertyMetadataOptions.AffectsRender);

            var axisColorPropertyMetadata =
                new FrameworkPropertyMetadata(Colors.Red,
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault |
                FrameworkPropertyMetadataOptions.AffectsRender);

            DataProperty =
                DependencyProperty.Register(
                    "Data", typeof(Point[]), typeof(MathDataViewport), dataPropertyMetadata);

            ProportionsProperty =
                DependencyProperty.Register(
                    "Proportions", typeof(Point), typeof(MathDataViewport), proportionsPropertyMetadata);

            AxisVisibleProperty =
                DependencyProperty.Register(
                    "AxisVisible", typeof(bool), typeof(MathDataViewport), axisVisiblePropertyMetadata);

            DataColorProperty =
                DependencyProperty.Register(
                    "DataColor", typeof(Color), typeof(MathDataViewport), dataColorPropertyMetadata);

            AxisColorProperty =
                DependencyProperty.Register(
                    "AxisColor", typeof(Color), typeof(MathDataViewport), axisColorPropertyMetadata);
        }

        #endregion        

        #region InnerData

        PointRenderer renderer;

        #endregion

        public override void OnApplyTemplate() {
            renderer = new PointRenderer();
            viewportSurface = GetTemplateChild(ViewportSurfaceName) as Image;
            axis = GetTemplateChild(XAxisName) as Path;
            OnRender(null);
        }

        protected override void OnRender(DrawingContext drawingContext) {
            viewportSurface.Width = Proportions.X;
            viewportSurface.Height = Proportions.Y;
            viewportSurface.Source = renderer.Render(Data, DataColor);

            axis.Visibility = AxisVisible ? Visibility.Visible : Visibility.Collapsed;
            axis.Stroke = new SolidColorBrush(AxisColor);
        }        
    }
}
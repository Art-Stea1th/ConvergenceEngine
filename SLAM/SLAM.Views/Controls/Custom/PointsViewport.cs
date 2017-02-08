using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace SLAM.Views.Controls.Custom {

    [TemplatePart(Name = ViewportSurfaceName, Type = typeof(Image))]
    public sealed class PointsViewport : Control {

        #region TemplateParts

        private const string ViewportSurfaceName = "PART_ViewportSurface";
        private const string XAxisName = "PART_Axis";
        private const string XPositiveLabelName = "PART_XPositive";
        private const string XNegativeLabelName = "PART_XNegative";
        private const string YPositiveLabelName = "PART_YPositive";
        //private const string YNegativeLabelName = "PART_YNegative";

        private Image viewportSurface;
        private Path axis;
        private Label xPositive, xNegative, yPositive, yNegative;

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

        static PointsViewport() {

            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(PointsViewport), new FrameworkPropertyMetadata(typeof(PointsViewport)));

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
                    "Data", typeof(Point[]), typeof(PointsViewport), dataPropertyMetadata);

            ProportionsProperty =
                DependencyProperty.Register(
                    "Proportions", typeof(Point), typeof(PointsViewport), proportionsPropertyMetadata);

            AxisVisibleProperty =
                DependencyProperty.Register(
                    "AxisVisible", typeof(bool), typeof(PointsViewport), axisVisiblePropertyMetadata);

            DataColorProperty =
                DependencyProperty.Register(
                    "DataColor", typeof(Color), typeof(PointsViewport), dataColorPropertyMetadata);

            AxisColorProperty =
                DependencyProperty.Register(
                    "AxisColor", typeof(Color), typeof(PointsViewport), axisColorPropertyMetadata);
        }

        #endregion        

        #region InnerData

        PointRenderer renderer;

        #endregion

        public override void OnApplyTemplate() {
            renderer = new PointRenderer();
            viewportSurface = GetTemplateChild(ViewportSurfaceName) as Image;
            axis = GetTemplateChild(XAxisName) as Path;
            xPositive = GetTemplateChild(XPositiveLabelName) as Label;
            xNegative = GetTemplateChild(XNegativeLabelName) as Label;
            yPositive = GetTemplateChild(YPositiveLabelName) as Label;
            //yNegative = GetTemplateChild(YNegativeLabelName) as Label;
            OnRender(null);
        }

        protected override void OnRender(DrawingContext drawingContext) {
            UpdateViewportSurface();
            UpdateAxis();
            UpdateAxisLabels();
        }

        private void UpdateViewportSurface() {
            viewportSurface.Width = Proportions.X;
            viewportSurface.Height = Proportions.Y;
            viewportSurface.Source = renderer.Render(Data, DataColor);
        }

        private void UpdateAxis() {

            if (AxisVisible) {
                axis.Stroke = new SolidColorBrush(AxisColor);
                axis.Visibility = Visibility.Visible;
                return;
            }
            axis.Visibility = Visibility.Collapsed;
        }

        private void UpdateAxisLabels() {
            if (AxisVisible) {

                xPositive.Foreground = new SolidColorBrush(DataColor);
                xNegative.Foreground = new SolidColorBrush(DataColor);
                yPositive.Foreground = new SolidColorBrush(DataColor);
                //yNegative.Foreground = new SolidColorBrush(DataColor);

                xPositive.Content = (int)renderer.MaxX;
                xNegative.Content = (int)renderer.MinX;
                yPositive.Content = (int)renderer.MaxY;
                //yNegative.Content = (int)renderer.MinY;

                xPositive.Visibility = Visibility.Visible;
                xNegative.Visibility = Visibility.Visible;
                yPositive.Visibility = Visibility.Visible;
                //yNegative.Visibility = Visibility.Visible;
                return;
            }
            xPositive.Visibility = Visibility.Collapsed;
            xNegative.Visibility = Visibility.Collapsed;
            yPositive.Visibility = Visibility.Collapsed;
            //yNegative.Visibility = Visibility.Collapsed;
        }
    }
}
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SLAM.Views.CustomControls {

    [TemplatePart(Name = MathDataViewport.ViewportSurfaceControlName, Type = typeof(ImageSource))]
    public class MathDataViewport : Control {        

        #region DependencyProperties

        public static readonly DependencyProperty DataProperty;

        public Point[] Data {
            get { return (Point[])GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        static MathDataViewport() {            

            var dataPropertyMetadata = new FrameworkPropertyMetadata();
            dataPropertyMetadata.DefaultValue = null;
            dataPropertyMetadata.BindsTwoWayByDefault = true;
            dataPropertyMetadata.AffectsRender = true;
            dataPropertyMetadata.PropertyChangedCallback = OnDataPropertyChangedCallback;

            DataProperty =
                DependencyProperty.Register(
                    "Data", typeof(Point[]), typeof(MathDataViewport), dataPropertyMetadata);

            // DefaultStyleKeyProperty.OverrideMetadata(typeof(MathDataViewport), dataPropertyMetadata);
        }

        #endregion

        #region TemplateParts

        private const string ViewportSurfaceControlName = "PART_ViewportSurfaceSource";
        private ImageSource  viewportSurfaceControl;

        #endregion

        public override void OnApplyTemplate() {
            viewportSurfaceControl = GetTemplateChild(ViewportSurfaceControlName) as ImageSource;
        }

        private static void OnDataPropertyChangedCallback(DependencyObject dObject, DependencyPropertyChangedEventArgs e) {
            throw new NotImplementedException();
        }

        protected override void OnRender(DrawingContext drawingContext) {
            // base.OnRender(drawingContext);
        }
    }
}
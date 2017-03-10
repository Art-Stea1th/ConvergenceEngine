using System.Windows;
using System.Windows.Controls;

namespace ConvergenceEngine.Views.AppCustomControls {
    using System.Windows.Media;
    using Infrastructure.Interfaces;

    public class MapViewport : Control {

        public IMap Map {
            get { return (IMap)GetValue(MapProperty); }
            set { SetValue(MapProperty, value); }
        }

        public bool ShowMapPoints {
            get { return (bool)GetValue(ShowMapPointsProperty); }
            set { SetValue(ShowMapPointsProperty, value); }
        }

        public bool ShowMapSegments {
            get { return (bool)GetValue(ShowMapSegmentsProperty); }
            set { SetValue(ShowMapSegmentsProperty, value); }
        }

        public bool ShowCurrentSegments {
            get { return (bool)GetValue(ShowCurrentSegmentsProperty); }
            set { SetValue(ShowCurrentSegmentsProperty, value); }
        }

        public bool ShowSensorPath {
            get { return (bool)GetValue(ShowSensorPathProperty); }
            set { SetValue(ShowSensorPathProperty, value); }
        }

        public bool ShowSensorPosition {
            get { return (bool)GetValue(ShowSensorPositionProperty); }
            set { SetValue(ShowSensorPositionProperty, value); }
        }

        public static readonly DependencyProperty MapProperty;

        public static readonly DependencyProperty ShowMapPointsProperty;
        public static readonly DependencyProperty ShowMapSegmentsProperty;

        public static readonly DependencyProperty ShowCurrentSegmentsProperty;

        public static readonly DependencyProperty ShowSensorPathProperty;
        public static readonly DependencyProperty ShowSensorPositionProperty;

        private Point min, max;

        private double Width { get { return max.X - min.X; } }
        private double Height { get { return max.Y - min.Y; } }

        static MapViewport() {

            DefaultStyleKeyProperty
                .OverrideMetadata(typeof(MapViewport), new FrameworkPropertyMetadata(typeof(MapViewport)));

            MapProperty = DependencyProperty.Register("Map", typeof(IMap), typeof(MapViewport),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

            ShowMapPointsProperty = DependencyProperty.Register("ShowMapPoints", typeof(bool), typeof(MapViewport),
                new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsRender));

            ShowMapSegmentsProperty = DependencyProperty.Register("ShowMapSegments", typeof(bool), typeof(MapViewport),
                new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsRender));

            ShowCurrentSegmentsProperty = DependencyProperty.Register("ShowCurrentSegments", typeof(bool), typeof(MapViewport),
                new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsRender));

            ShowSensorPathProperty = DependencyProperty.Register("ShowSensorPath", typeof(bool), typeof(MapViewport),
                new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsRender));

            ShowSensorPositionProperty = DependencyProperty.Register("ShowSensorPosition", typeof(bool), typeof(MapViewport),
                new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsRender));
        }

        public override void OnApplyTemplate() {
            base.OnApplyTemplate();
        }

        protected override void OnRender(DrawingContext drawingContext) {
            base.OnRender(drawingContext);
        }

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ConvergenceEngine.Views.AppCustomControls {

    using Infrastructure.Interfaces;
    using Infrastructure.Extensions;

    [TemplatePart(Name = PartMapPointsName, Type = typeof(Image))]
    [TemplatePart(Name = PartMapSegmentsName, Type = typeof(Path))]
    [TemplatePart(Name = PartActualSegmentsName, Type = typeof(Path))]
    [TemplatePart(Name = PartDepthSensorPathName, Type = typeof(Polyline))]
    [TemplatePart(Name = PartDepthSensorPositionName, Type = typeof(Polygon))]
    public class MapperViewport : Control {

        public const string PartMapPointsName = "PART_MapPoints";
        public const string PartMapSegmentsName = "PART_MapSegments";
        public const string PartActualSegmentsName = "PART_ActualSegments";
        public const string PartDepthSensorPathName = "PART_DepthSensorPath";
        public const string PartDepthSensorPositionName = "PART_DepthSensorPosition";

        public IEnumerable<ISegment> MapSegments {
            get => (IEnumerable<ISegment>)GetValue(MapSegmentsProperty);
            set => SetValue(MapSegmentsProperty, value);
        }

        public IEnumerable<ISegment> ActualSegments {
            get => (IEnumerable<ISegment>)GetValue(ActualSegmentsProperty);
            set => SetValue(ActualSegmentsProperty, value);
        }

        public bool ShowMapPoints {
            get => (bool)GetValue(ShowMapPointsProperty);
            set => SetValue(ShowMapPointsProperty, value);
        }

        public bool ShowMapSegments {
            get => (bool)GetValue(ShowMapSegmentsProperty);
            set => SetValue(ShowMapSegmentsProperty, value);
        }

        public bool ShowActualSegments {
            get => (bool)GetValue(ShowActualSegmentsProperty);
            set => SetValue(ShowActualSegmentsProperty, value);
        }

        public bool ShowDepthSensorPath {
            get => (bool)GetValue(ShowDepthSensorPathProperty);
            set => SetValue(ShowDepthSensorPathProperty, value);
        }

        public bool ShowDepthSensorPosition {
            get => (bool)GetValue(ShowDepthSensorPositionProperty);
            set => SetValue(ShowDepthSensorPositionProperty, value);
        }

        public static readonly DependencyProperty MapSegmentsProperty;
        public static readonly DependencyProperty ActualSegmentsProperty;

        public static readonly DependencyProperty ShowMapPointsProperty;
        public static readonly DependencyProperty ShowMapSegmentsProperty;

        public static readonly DependencyProperty ShowActualSegmentsProperty;

        public static readonly DependencyProperty ShowDepthSensorPathProperty;
        public static readonly DependencyProperty ShowDepthSensorPositionProperty;

        static MapperViewport() {

            DefaultStyleKeyProperty
                .OverrideMetadata(typeof(MapperViewport), new FrameworkPropertyMetadata(typeof(MapperViewport)));

            MapSegmentsProperty = DependencyProperty.Register("MapSegments", typeof(IEnumerable<ISegment>), typeof(MapperViewport),
                new FrameworkPropertyMetadata(null, (s, e) => (s as MapperViewport).UpdateMapSegments()));

            ActualSegmentsProperty = DependencyProperty.Register("ActualSegments", typeof(IEnumerable<ISegment>), typeof(MapperViewport),
                 new FrameworkPropertyMetadata(null, (s, e) => (s as MapperViewport).UpdateActualSegments()));

            ShowMapPointsProperty = DependencyProperty.Register("ShowMapPoints", typeof(bool), typeof(MapperViewport),
                new FrameworkPropertyMetadata(true, (s, e) =>
                OnShowPropertyChanged((s as MapperViewport)._mapPoints, (bool)e.NewValue, (s as MapperViewport).ReDrawMapPoints)));

            ShowMapSegmentsProperty = DependencyProperty.Register("ShowMapSegments", typeof(bool), typeof(MapperViewport),
                new FrameworkPropertyMetadata(true, (s, e) =>
                OnShowPropertyChanged((s as MapperViewport)._mapSegments, (bool)e.NewValue, (s as MapperViewport).UpdateMapSegments)));

            ShowActualSegmentsProperty = DependencyProperty.Register("ShowActualSegments", typeof(bool), typeof(MapperViewport),
                new FrameworkPropertyMetadata(true, (s, e) =>
                OnShowPropertyChanged((s as MapperViewport)._actualSegments, (bool)e.NewValue, (s as MapperViewport).UpdateActualSegments)));

            ShowDepthSensorPathProperty = DependencyProperty.Register("ShowDepthSensorPath", typeof(bool), typeof(MapperViewport),
                new FrameworkPropertyMetadata(true, (s, e) =>
                OnShowPropertyChanged((s as MapperViewport)._depthSensorPath, (bool)e.NewValue, (s as MapperViewport).ReDrawDepthSensorPath)));

            ShowDepthSensorPositionProperty = DependencyProperty.Register("ShowDepthSensorPosition", typeof(bool), typeof(MapperViewport),
                new FrameworkPropertyMetadata(true, (s, e) =>
                OnShowPropertyChanged((s as MapperViewport)._depthSensorPosition, (bool)e.NewValue, (s as MapperViewport).UpdateDepthSensorPosition)));
        }


        private static void OnShowPropertyChanged(FrameworkElement element, bool visible, Action updateCallback) {
            if (visible) {
                updateCallback.Invoke();
                element.Visibility = Visibility.Visible;
            }
            else {
                element.Visibility = Visibility.Hidden;
            }
        }        

        private Image _mapPoints;             // Source - WriteableBitmap
        private Path _mapSegments;            // Data   - PathGeometry
        private Path _actualSegments;         // Data   - PathGeometry
        private Polyline _depthSensorPath;    // Points - PointCollection
        private Polygon _depthSensorPosition; // Points - PointCollection

        private Point _min = new Point(double.MaxValue, double.MaxValue);
        private Point _max = new Point(double.MinValue, double.MinValue);

        private double AreaWidth => (_max.X - _min.X) + 1;
        private double AreaHeight => (_max.Y - _min.Y) + 1;

        public override void OnApplyTemplate() {
            InitializeParts();
            DrawArrowAt(_depthSensorPosition, AreaWidth / 2, AreaHeight / 2);
        }

        private void InitializeParts() {
            _mapPoints = GetTemplateChild(PartMapPointsName) as Image;
            _mapSegments = GetTemplateChild(PartMapSegmentsName) as Path;
            _actualSegments = GetTemplateChild(PartActualSegmentsName) as Path;
            _depthSensorPath = GetTemplateChild(PartDepthSensorPathName) as Polyline;
            _depthSensorPosition = GetTemplateChild(PartDepthSensorPositionName) as Polygon;
        }


        private void Update() {

            ReDrawDepthSensorPath();
            UpdateDepthSensorPosition();
        }

        private void UpdateMapSegments() {
            if (MapSegments == null || MapSegments.Count() < 1) {
                return;
            }
            UpdateLimitsBy(MapSegments);
            if (ShowMapSegments) {
                RedrawSegments(_mapSegments, MapSegments);
            }
        }

        private void UpdateActualSegments() {
            if (ActualSegments == null || ActualSegments.Count() < 1) {
                return;
            }
            UpdateLimitsBy(MapSegments == null || MapSegments.Count() < 1 ? ActualSegments : MapSegments);
            if (ShowActualSegments) {
                RedrawSegments(_actualSegments, ActualSegments);
            }
        }

        private void UpdateLimitsBy(IEnumerable<ISegment> segments) {

            if (segments == null || segments.Count() < 1) {
                return;
            }

            _min = new Point(double.MaxValue, double.MaxValue);
            _max = new Point(double.MinValue, double.MinValue);

            foreach (var segment in segments) {
                foreach (var point in segment) {
                    if (point.X < _min.X) { _min.X = point.X; }
                    else
                    if (point.X > _max.X) { _max.X = point.X; }
                    if (point.Y < _min.Y) { _min.Y = point.Y; }
                    else
                    if (point.Y > _max.Y) { _max.Y = point.Y; }
                }
            }
        }

        private void ReDrawMapPoints() {
            if (ShowMapPoints) {
                //mapPoints.Source = NewBitmap((int)Width, (int)Height);
                return;
            }
        }

        private void ReDrawDepthSensorPath() {
            if (ShowDepthSensorPath) {
                return;
            }
        }

        private void UpdateDepthSensorPosition() {
            if (ShowDepthSensorPosition) {
                //if (MapperData?.CameraPath != null) {
                //    var last = MapperData.CameraPath.Last();
                //    Point position = new Point(last.X - min.X, (Height - 1) - (last.Y - min.Y));
                //    DrawArrowAt(depthSensorPosition, position.X, position.Y, -last.A);
                //}
                //else {
                //    depthSensorPosition.Points.Clear();
                //}
            }
        }

        private void DrawArrowAt(Polygon polygon, double x = 0.0, double y = 0.0, double angle = 0.0) {
            polygon.Points.Clear();
            polygon.Points.Add(new Point(-15, 20).RotatedAndShifted(x, y, angle));
            polygon.Points.Add(new Point(0, -20).RotatedAndShifted(x, y, angle));
            polygon.Points.Add(new Point(15, 20).RotatedAndShifted(x, y, angle));
        }

        private void RedrawSegments(Path path, IEnumerable<ISegment> segments) {
            var geometry = new PathGeometry();

            // Border
            //geometry.Figures.Add(new PathFigure(
            //            new Point(0, 0),
            //            new List<LineSegment> {
            //                new LineSegment(new Point(Width, 0), true),
            //                new LineSegment(new Point(Width, Height), true),
            //                new LineSegment(new Point(0, Height), true),
            //                new LineSegment(new Point(0, 0), true)
            //            }, false));

            if (segments != null) {
                // Linear Data
                foreach (var segment in segments) {

                    if (segment != null) {
                        var startPoint = Fixed(segment.A);
                        var lineSegments = new List<LineSegment> { new LineSegment(Fixed(segment.B), true) };
                        var figure = new PathFigure(startPoint, lineSegments, false);
                        geometry.Figures.Add(figure);
                    }
                }
            }
            path.Data = geometry;

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Point Fixed(Point point) {
            return FixToSystemScreenCoordinate(FixToPositiveOnly(point, _min.X, _min.Y), AreaHeight);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Point FixToSystemScreenCoordinate(Point point, double screenHeight) {
            return new Point(point.X, (screenHeight - 1) - point.Y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Point FixToPositiveOnly(Point point, double minX, double minY) {
            return new Point(point.X - minX, point.Y - minY);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private WriteableBitmap NewBitmap(int width, int height, bool withAlpha = false) {
            return new WriteableBitmap(width > 1 ? width : 1, height > 1 ? height : 1, 96.0, 96.0,
                withAlpha ? PixelFormats.Bgra32 : PixelFormats.Bgr32, null);
        }
    }
}
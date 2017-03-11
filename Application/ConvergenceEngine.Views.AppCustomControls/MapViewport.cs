using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ConvergenceEngine.Views.AppCustomControls {

    using Infrastructure.Interfaces;
    using System.Linq;

    [TemplatePart(Name = MapViewport.PartMapPointsName, Type = typeof(Image))]
    [TemplatePart(Name = MapViewport.PartMapSegmentsName, Type = typeof(Path))]
    [TemplatePart(Name = MapViewport.PartCurrentSegmentsName, Type = typeof(Path))]
    [TemplatePart(Name = MapViewport.PartDepthSensorPathName, Type = typeof(Polyline))]
    [TemplatePart(Name = MapViewport.PartDepthSensorPositionName, Type = typeof(Polygon))]
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

        public bool ShowDepthSensorPath {
            get { return (bool)GetValue(ShowDepthSensorPathProperty); }
            set { SetValue(ShowDepthSensorPathProperty, value); }
        }

        public bool ShowDepthSensorPosition {
            get { return (bool)GetValue(ShowDepthSensorPositionProperty); }
            set { SetValue(ShowDepthSensorPositionProperty, value); }
        }

        public static readonly DependencyProperty MapProperty;

        public static readonly DependencyProperty ShowMapPointsProperty;
        public static readonly DependencyProperty ShowMapSegmentsProperty;

        public static readonly DependencyProperty ShowCurrentSegmentsProperty;

        public static readonly DependencyProperty ShowDepthSensorPathProperty;
        public static readonly DependencyProperty ShowDepthSensorPositionProperty;        

        static MapViewport() {

            DefaultStyleKeyProperty
                .OverrideMetadata(typeof(MapViewport), new FrameworkPropertyMetadata(typeof(MapViewport)));

            MapProperty = DependencyProperty.Register("Map", typeof(IMap), typeof(MapViewport),
                new FrameworkPropertyMetadata(null, (s, e) => (s as MapViewport).Update()));

            ShowMapPointsProperty = DependencyProperty.Register("ShowMapPoints", typeof(bool), typeof(MapViewport),
                new FrameworkPropertyMetadata(true, (s, e) =>
                OnShowPropertyChanged((s as MapViewport).mapPoints, (bool)e.NewValue, (s as MapViewport).ReDrawMapPoints)));

            ShowMapSegmentsProperty = DependencyProperty.Register("ShowMapSegments", typeof(bool), typeof(MapViewport),
                new FrameworkPropertyMetadata(true, (s, e) =>
                OnShowPropertyChanged((s as MapViewport).mapSegments, (bool)e.NewValue, (s as MapViewport).ReDrawMapSegments)));

            ShowCurrentSegmentsProperty = DependencyProperty.Register("ShowCurrentSegments", typeof(bool), typeof(MapViewport),
                new FrameworkPropertyMetadata(true, (s, e) =>
                OnShowPropertyChanged((s as MapViewport).currentSegments, (bool)e.NewValue, (s as MapViewport).ReDrawCurrentSegments)));

            ShowDepthSensorPathProperty = DependencyProperty.Register("ShowDepthSensorPath", typeof(bool), typeof(MapViewport),
                new FrameworkPropertyMetadata(true, (s, e) =>
                OnShowPropertyChanged((s as MapViewport).depthSensorPath, (bool)e.NewValue, (s as MapViewport).ReDrawDepthSensorPath)));

            ShowDepthSensorPositionProperty = DependencyProperty.Register("ShowDepthSensorPosition", typeof(bool), typeof(MapViewport),
                new FrameworkPropertyMetadata(true, (s, e) =>
                OnShowPropertyChanged((s as MapViewport).depthSensorPosition, (bool)e.NewValue, (s as MapViewport).UpdateDepthSensorPosition)));
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

        private const string PartMapPointsName = "PART_MapPoints";
        private const string PartMapSegmentsName = "PART_MapSegments";
        private const string PartCurrentSegmentsName = "PART_CurrentSegments";
        private const string PartDepthSensorPathName = "PART_DepthSensorPath";
        private const string PartDepthSensorPositionName = "PART_DepthSensorPosition";

        private Image mapPoints;             // Source - WriteableBitmap
        private Path mapSegments;            // Data   - PathGeometry                                             
        private Path currentSegments;        // Data   - PathGeometry
        private Polyline depthSensorPath;    // Points - PointCollection
        private Polygon depthSensorPosition; // Points - PointCollection

        private Point min = new Point(double.MaxValue, double.MaxValue);
        private Point max = new Point(double.MinValue, double.MinValue);

        private double Width { get { return max.X - min.X; } }
        private double Height { get { return max.Y - min.Y; } }

        public override void OnApplyTemplate() {
            mapPoints = GetTemplateChild(PartMapPointsName) as Image;
            mapSegments = GetTemplateChild(PartMapSegmentsName) as Path;
            currentSegments = GetTemplateChild(PartCurrentSegmentsName) as Path;
            depthSensorPath = GetTemplateChild(PartDepthSensorPathName) as Polyline;
            depthSensorPosition = GetTemplateChild(PartDepthSensorPositionName) as Polygon;
        }

        private void Update() {
            if (Map == null) {
                return;
            }
            UpdateLimitsBy(Map.Segments != null && Map.Segments.Count() > 0 ? Map.Segments : Map.CurrentSegments);
            ReDrawMapPoints();
            ReDrawMapSegments();
            ReDrawCurrentSegments();
            ReDrawDepthSensorPath();
            UpdateDepthSensorPosition();
        }

        private void UpdateLimitsBy(IEnumerable<ISegment> segments) {
            if (segments == null) {
                return;
            }
            foreach (var segment in segments) {
                foreach (var point in segment) {
                    if (point.X < min.X) { min.X = point.X; }
                    else
                    if (point.X > max.X) { max.X = point.X; }
                    if (point.Y < min.Y) { min.Y = point.Y; }
                    else
                    if (point.Y > max.X) { max.Y = point.Y; }
                }
            }
        }        

        private void ReDrawMapPoints() {
            if (ShowMapPoints) {
                //Console.WriteLine($"Map Points");
                return;
            }
        }

        private void ReDrawMapSegments() {
            if (ShowMapSegments) {
                //Console.WriteLine($"Map Segments");
                return;
            }
        }

        private void ReDrawCurrentSegments() {
            if (ShowCurrentSegments) {
                //Console.WriteLine($"Current Segments");
                RedrawSegments(currentSegments, Map?.CurrentSegments);
                return;
            }            
        }

        private void ReDrawDepthSensorPath() {
            if (ShowDepthSensorPath) {
                //Console.WriteLine($"Sensor Path");
                return;
            }            
        }

        private void UpdateDepthSensorPosition() {
            if (ShowDepthSensorPosition) {
                //Console.WriteLine($"Sensor Position");
                return;
            }
        }

        private void RedrawSegments(Path path, IEnumerable<ISegment> segments) {
            PathGeometry geometry = new PathGeometry();

            // Border
            geometry.Figures.Add(new PathFigure(
                        new Point(0, 0),
                        new List<LineSegment> {
                            new LineSegment(new Point(Width, 0), true),
                            new LineSegment(new Point(Width, Height), true),
                            new LineSegment(new Point(0, Height), true),
                            new LineSegment(new Point(0, 0), true)
                        }, false));

            if (segments != null) {
                // Linear Data
                foreach (var segment in segments) {

                    if (segment != null) {
                        Point startPoint = FixPosition(segment.PointA);
                        var lineSegments = new List<LineSegment> { new LineSegment(FixPosition(segment.PointB), true) };
                        PathFigure figure = new PathFigure(startPoint, lineSegments, false);
                        geometry.Figures.Add(figure);
                    }
                }
            }
            path.Data = geometry;
            
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Point FixPosition(Point point) {
            var result = new Point(point.X /*+ Width / 2*/, /*(Height - 1)*/ - point.Y);
            Console.WriteLine(result);
            return result;
        }
    }
}
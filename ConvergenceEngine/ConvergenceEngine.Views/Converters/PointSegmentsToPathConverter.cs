using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ConvergenceEngine.Views.Converters {

    [ValueConversion(typeof(IEnumerable<Tuple<Point, Point>>), typeof(PathGeometry))]
    public class PointSegmentsToPathConverter : IValueConverter {

        private const int width = 640, height = 480;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {

            var segments = (IEnumerable<Tuple<Point, Point>>)value;
            PathGeometry geometry = new PathGeometry();

            // Border
            geometry.Figures.Add(new PathFigure(
                        new Point(0, 0),
                        new List<LineSegment> {
                            new LineSegment(new Point(width, 0), true),
                            new LineSegment(new Point(width, height), true),
                            new LineSegment(new Point(0, height), true),
                            new LineSegment(new Point(0, 0), true)
                        }, false));

            // Linear Data
            if (segments != null) {
                foreach (var segment in segments) {

                    if (segment != null) {
                        Point startPoint = FlipVertical(segment.Item1);
                        var lineSegments = new List<LineSegment> { new LineSegment(FlipVertical(segment.Item2), true) };
                        PathFigure figure = new PathFigure(startPoint, lineSegments, false);
                        geometry.Figures.Add(figure);
                    }                    
                }
            }
            return geometry;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotSupportedException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Point FlipVertical(Point point) {
            var result = new Point(point.X + width / 2, (height - 1) - point.Y);
            return result;
        }
    }
}
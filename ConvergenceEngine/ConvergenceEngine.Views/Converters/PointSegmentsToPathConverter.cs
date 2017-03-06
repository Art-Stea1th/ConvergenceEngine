using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace ConvergenceEngine.Views.Converters {

    //[ValueConversion(typeof(IEnumerable<Tuple<Point, Point>>), typeof(PathGeometry))]
    public class PointSegmentsToPathConverter : IMultiValueConverter {

        private double width = 640, height = 480;

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {

            return null;

            if (values == null || values.Length < 2) { return null; }

            var size = (Point)values[0];
            var segments = (IEnumerable<Tuple<Point, Point>>)values[1];

            width = size.X; height = size.Y;
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

            if (segments != null) {
                // Linear Data
                foreach (var segment in segments) {

                    if (segment != null) {
                        Point startPoint = FixPosition(segment.Item1);
                        var lineSegments = new List<LineSegment> { new LineSegment(FixPosition(segment.Item2), true) };
                        PathFigure figure = new PathFigure(startPoint, lineSegments, false);
                        geometry.Figures.Add(figure);
                    }
                }
            }
            return geometry;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
            throw new NotSupportedException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Point FixPosition(Point point) {
            var result = new Point(point.X + width / 2, (height - 1) - point.Y);
            return result;
        }
    }
}
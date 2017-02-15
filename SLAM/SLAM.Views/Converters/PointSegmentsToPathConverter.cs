using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace SLAM.Views.Converters {

    [ValueConversion(typeof(IEnumerable<Tuple<Point, Point>>), typeof(PathGeometry))]
    public class PointSegmentsToPathConverter : IValueConverter {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {

            var segments = (IEnumerable<Tuple<Point, Point>>)value;
            PathGeometry geometry = new PathGeometry();
            
            // Border
            geometry.Figures.Add(new PathFigure(
                        new Point(-224, -60),
                        new List<LineSegment> {
                            new LineSegment(new Point(224, -60), true),
                            new LineSegment(new Point(224, -410), true),
                            new LineSegment(new Point(-224, -410), true),
                            new LineSegment(new Point(-224, -60), true)
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
            return new Point(point.X, -point.Y);
        }
    }
}
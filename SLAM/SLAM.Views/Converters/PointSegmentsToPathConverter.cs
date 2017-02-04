using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace SLAM.Views.Converters {

    public class PointSegmentsToPathConverter : IValueConverter {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {

            var segments = (IEnumerable<IEnumerable<Point>>)value;
            if (segments != null) {

                PathGeometry geometry = new PathGeometry();
                foreach (var segment in segments) {

                    if (segment != null) {
                        Point startPoint = FlipVertical(segment.First());
                        List<LineSegment> lineSegments = new List<LineSegment>();
                        foreach (var point in segment.Skip(1)) {
                            lineSegments.Add(new LineSegment(FlipVertical(point), true));
                        }
                        PathFigure figure = new PathFigure(startPoint, lineSegments, false);
                        geometry.Figures.Add(figure);
                    }                    
                }
                return geometry;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }

        private Point FlipVertical(Point point) {
            return new Point(point.X, -point.Y);
        }
    }
}
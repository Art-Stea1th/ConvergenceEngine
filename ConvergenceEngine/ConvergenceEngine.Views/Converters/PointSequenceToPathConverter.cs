using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ConvergenceEngine.Views.Converters {

    [ValueConversion(typeof(IEnumerable<Point>), typeof(PathGeometry))]
    public class PointSequenceToPathConverter : IValueConverter {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {

            IEnumerable<Point> newPoints = value as IEnumerable<Point>;
            if (newPoints == null || newPoints.Count() < 1) {
                return null;
            }
            return GetGeometry(newPoints);
        }

        private PathGeometry GetGeometry(IEnumerable<Point> sequence) {

            PathGeometry geometry = new PathGeometry();            

            if (sequence != null) {
                Point? prevPoint = null;
                Point? currPoint = null;

                foreach (var point in sequence) {

                    prevPoint = currPoint;
                    currPoint = point;

                    PathFigure figure = null;

                    if (prevPoint != null) {
                        var lineSegments = new List<LineSegment> { new LineSegment(point, true) };
                        figure = new PathFigure(prevPoint.Value, lineSegments, false);
                    }

                    if (figure != null) {
                        geometry.Figures.Add(figure);
                    }                   
                }
            }
            return geometry;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotSupportedException();
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using System.Windows;


namespace SLAM.Models.Mapping.ComplexMapperResources {

    using Extensions;

    internal sealed class DataBeautifier {

        public IEnumerable<Point> RemoveArtifacts(IEnumerable<Point> points, int size = 2, double threshold = 7.5) {

            var result = new List<Point>();
            var segment = new List<Point>();

            foreach (var point in points) {
                if (segment.IsEmpty() || point.DistanceTo(segment.Last()) < threshold) {
                    segment.Add(point);
                }
                else {
                    if (segment.Count > size) {
                        result.AddRange(segment);
                    }
                    segment.Clear();
                    segment.Add(point);
                }
            }

            if (segment.Count > size) {
                result.AddRange(segment);
            }
            return result;
        }

        public IEnumerable<Point> NormalizeFrame(IEnumerable<Point> points, double threshold) {

            Point firstPoint = points.First();
            yield return firstPoint;

            foreach (var point in points.Skip(1)) {
                
                if (firstPoint.DistanceTo(point) >= threshold) {
                    yield return point;
                    firstPoint = point;
                }
            }
        }
    }
}
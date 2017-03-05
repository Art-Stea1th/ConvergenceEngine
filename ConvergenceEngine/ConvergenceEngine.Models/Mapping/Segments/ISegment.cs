using System.Collections;
using System.Collections.Generic;
using System.Windows;

namespace ConvergenceEngine.Models.Mapping.Segments {

    public interface ISegment : IReadOnlyList<Point>, IReadOnlyCollection<Point>, IEnumerable<Point>, IEnumerable {

        Point PointA { get; }
        Point PointB { get; }
        Point CenterPoint { get; }

        double Length { get; }

        double DistanceToNearestPoint(ISegment segment);
        void ApplyTransform(double offsetX, double offsetY, double angle, bool rotatePrepend = true);
    }
}
using System.Collections.Generic;
using System.Windows;


namespace ConvergenceEngine.Models.Mapping.Segments {

    public interface ISegment : IReadOnlyList<Point>, IReadOnlyCollection<Point>, IEnumerable<Point> {

        Point PointA { get; }
        Point PointB { get; }
        Point CenterPoint { get; }
        double Length { get; }

        double DistanceToNearestPoint(ISegment segment);
    }
}
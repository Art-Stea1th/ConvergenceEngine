using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace ConvergenceEngine.Models.Mapping {

    using Extensions;
    using Infrastructure.Interfaces;
    using Segments;

    public sealed class Mapper : IMapper {

        public event Action<IMapData> OnMapUpdate;

        private const double AllowedDivergencePercent = 3.0;

        private const double MaxDistancePercentToPrev = 5.0;
        private const double MaxAngleDegreesToPrev = 3.0;

        private const double MaxDistancePercentToMap = 5.0;
        private const double MaxAngleDegreesToMap = 3.0;

        private Map map;

        private IEnumerable<Segment> prev;
        private IEnumerable<Segment> next;
        private List<INavigationInfo> path;

        public void HandleNextData(IEnumerable<Point> points) {

            next = points.Segmentate(AllowedDivergencePercent).Select(s => new Segment(s));

            if (map == null) {
                if (next.Count() > 0) {
                    InitializeWith(next);
                    prev = next;
                }
                return;
            }

            // ---

            NavigationInfo prevNavInfo = new NavigationInfo(path.Last());
            next = TransformedSegments(next, prevNavInfo);

            // ---

            var nearestPairsToPrev = next.SelectNearestTo(prev, MaxDistancePercentToPrev, MaxAngleDegreesToPrev, prevNavInfo.X, prevNavInfo.Y);
            NavigationInfo convergenceToPrev = nearestPairsToPrev.ComputeConvergence(MaxDistancePercentToPrev, MaxAngleDegreesToPrev, prevNavInfo.X, prevNavInfo.Y);

            // ---

            next = TransformedSegments(next, convergenceToPrev);
            var nearestOnly = TransformedSegments(nearestPairsToPrev.Select(sp => sp.Item1), convergenceToPrev);

            NavigationInfo nextNavInfo = prevNavInfo + convergenceToPrev;

            // ---

            var nearestPairsToMap = nearestOnly.SelectNearestTo(map, MaxDistancePercentToMap, MaxAngleDegreesToMap, nextNavInfo.X, nextNavInfo.Y);
            NavigationInfo convergenceToMap = nearestPairsToMap.ComputeConvergence(MaxDistancePercentToMap, MaxAngleDegreesToMap, nextNavInfo.X, nextNavInfo.Y);

            // ---

            next = TransformedSegments(next, convergenceToMap);
            nearestOnly = TransformedSegments(nearestOnly, convergenceToMap);

            nextNavInfo += convergenceToMap;

            // ---

            foreach (var segment in nearestOnly) {
                map.AddSegment(segment);
            }
            path.Add(nextNavInfo);
            prev = next;

            OnMapUpdate?.Invoke(new MapData(map, next, path));
        }

        private void InitializeWith(IEnumerable<Segment> segments) {
            map = new Map(segments);
            path = new List<INavigationInfo> { new NavigationInfo(0.0, 0.0, 0.0) };
        }

        private IEnumerable<Segment> TransformedSegments(IEnumerable<Segment> segments, INavigationInfo navigationInfo) {
            foreach (var segment in segments) {
                yield return segment
                    .RotatedAt(navigationInfo.A, navigationInfo.X, navigationInfo.Y)
                    .Shifted(navigationInfo.X, navigationInfo.Y);
            }
        }
    }
}
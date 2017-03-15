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

        private const double AllowedDivergencePercent = 5.0;

        private const double MaxDistancePercentToPrev = 5.0;
        private const double MaxAngleDegreesToPrev = 3.0;

        private const double MaxDistancePercentToMap = 5.0;
        private const double MaxAngleDegreesToMap = 3.0;

        private Map map;

        private IEnumerable<MapSegment> prev;
        private IEnumerable<MapSegment> next;
        private List<INavigationInfo> path;

        public void HandleNextData(IEnumerable<Point> points) {

            next = points.Segmentate(AllowedDivergencePercent);

            if (map == null) {
                if (next.Count() > 0) {
                    InitializeWith(next);
                    prev = next;
                }
                return;
            }

            NavigationInfo prevNavInfo = new NavigationInfo(path.Last());
            TransformedSegments(next, prevNavInfo);

            // ---

            var nearestPairsToPrev = next.SelectNearestTo(prev, MaxDistancePercentToPrev, MaxAngleDegreesToPrev);
            NavigationInfo convergenceToPrev = nearestPairsToPrev.ComputeConvergence(MaxDistancePercentToPrev, MaxAngleDegreesToPrev);

            TransformedSegments(next, convergenceToPrev);

            // ---

            var nearestOnly = nearestPairsToPrev.Select(sp => sp.Item1);

            // ---

            var nearestPairsToMap = nearestOnly.SelectNearestTo(map, MaxDistancePercentToMap, MaxAngleDegreesToMap);
            NavigationInfo convergenceToMap = nearestPairsToMap.ComputeConvergence(MaxDistancePercentToMap, MaxAngleDegreesToMap);

            TransformedSegments(next, convergenceToMap);

            // ---

            foreach (var segment in nearestOnly) {
                map.AddSegment(segment as MapSegment);
            }
            path.Add(prevNavInfo + convergenceToPrev + convergenceToMap);
            prev = next;

            OnMapUpdate?.Invoke(new MapData(map, next, path));
        }

        private void InitializeWith(IEnumerable<MapSegment> segments) {
            map = new Map(segments);
            path = new List<INavigationInfo> { new NavigationInfo(0.0, 0.0, 0.0) };
        }

        //private NavigationInfo CalculateNavigationInfoBy(IEnumerable<ISegment> segments) {

        //}

        private void TransformedSegments(IEnumerable<MapSegment> segments, INavigationInfo navigationInfo) {
            foreach (var segment in segments) {
                segment.ApplyTransform(navigationInfo.X, navigationInfo.Y, navigationInfo.A);
            }
        }
    }
}
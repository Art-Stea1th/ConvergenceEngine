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

        private const double MaxDistancePercentToMap = 10.0;
        private const double MaxAngleDegreesToMap = 6.0;

        private Map map;

        private List<MultiPointSegment> prev;
        private List<MultiPointSegment> next;
        private List<NavigationInfo> cameraPath;

        public void HandleNextData(IEnumerable<Point> points) {

            next = new List<MultiPointSegment>(points.Segmentate(AllowedDivergencePercent));

            if (map == null) {
                if (next.Count > 0) {
                    InitializeWith(next);
                    prev = next;
                }
                return;
            }

            NavigationInfo prevNavInfo = cameraPath.Last();
            ApplyTransformToSegments(next, prevNavInfo);

            // ---

            var nearestPairsToPrev = next.SelectNearestTo(prev, MaxDistancePercentToPrev, MaxAngleDegreesToPrev);
            NavigationInfo convergenceToPrev = nearestPairsToPrev.ComputeConvergence(MaxDistancePercentToPrev, MaxAngleDegreesToPrev);

            ApplyTransformToSegments(next, convergenceToPrev);

            // ---

            var nearestOnly = nearestPairsToPrev.Select(sp => sp.Item1);

            // ---

            var nearestPairsToMap = nearestOnly.SelectNearestTo(map, MaxDistancePercentToMap, MaxAngleDegreesToMap);
            NavigationInfo convergenceToMap = nearestPairsToMap.ComputeConvergence(MaxDistancePercentToMap, MaxAngleDegreesToMap);

            ApplyTransformToSegments(next, convergenceToMap);

            // ---

            foreach (var segment in nearestOnly) {
                map.AddSegment(segment);
            }
            cameraPath.Add(prevNavInfo + convergenceToPrev + convergenceToMap);
            prev = next;

            OnMapUpdate?.Invoke(new MapData(map, next, cameraPath));
        }

        private void InitializeWith(IEnumerable<MultiPointSegment> segments) {
            map = new Map(next);
            cameraPath = new List<NavigationInfo> { new NavigationInfo(0.0, 0.0, 0.0) };
        }

        private void ApplyTransformToSegments(IEnumerable<ISegment> segments, NavigationInfo navigationInfo) {
            foreach (var segment in segments) {
                segment.ApplyTransform(navigationInfo.X, navigationInfo.Y, navigationInfo.A);
            }
        }
    }
}
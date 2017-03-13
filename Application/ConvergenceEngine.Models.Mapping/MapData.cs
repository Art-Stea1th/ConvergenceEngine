using System.Collections.Generic;

namespace ConvergenceEngine.Models.Mapping {

    using Infrastructure.Interfaces;
    using Segments;
    using System.Linq;

    public sealed class MapData : IMapData {

        public IEnumerable<ISegment> Segments { get; } = null;
        public IEnumerable<ISegment> CurrentSegments { get; } = null;
        public IEnumerable<INavigationInfo> CameraPath { get; } = null;

        internal MapData(
            IEnumerable<ISegment> segments,
            IEnumerable<ISegment> currentSegments,
            IEnumerable<INavigationInfo> cameraPath) {
            if (segments != null && segments.Count() > 1 && segments.First().Count() > 1) {
                Segments = new List<MultiPointSegment>(segments.Select(s => new MultiPointSegment(s)));
            }
            if (currentSegments != null && currentSegments.Count() > 0 && currentSegments.First().Count() > 1) {
                CurrentSegments = new List<MultiPointSegment>(currentSegments.Select(s => new MultiPointSegment(s)));
            }
            if (cameraPath != null && cameraPath.Count() > 0) {
                CameraPath = new List<NavigationInfo>(cameraPath.Select(cp => new NavigationInfo(cp.X, cp.Y, cp.A)));
            }
        }
    }
}
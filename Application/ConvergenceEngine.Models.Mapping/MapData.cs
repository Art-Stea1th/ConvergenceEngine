using System.Linq;
using System.Collections.Generic;

namespace ConvergenceEngine.Models.Mapping {

    using Extensions;
    using Infrastructure.Interfaces;
    using Segments;

    public sealed class MapData : IMapData {

        public IEnumerable<ISegment> Segments { get; } = null;
        public IEnumerable<ISegment> CurrentSegments { get; } = null;
        public IEnumerable<INavigationInfo> CameraPath { get; } = null;

        internal MapData(
            IEnumerable<ISegment> segments, IEnumerable<ISegment> currentSegments, IEnumerable<INavigationInfo> cameraPath) {
            if (segments != null && segments.Count() > 1 && segments.First().Count() > 1) {
                Segments = segments.Select(s => new MultiPointSegment(s.ThinOutSorted(5.0))).ToList();
            }
            if (currentSegments != null && currentSegments.Count() > 0 && currentSegments.First().Count() > 1) {
                CurrentSegments = currentSegments.Select(s => new MultiPointSegment(s.ThinOutSorted(5.0))).ToList();
            }
            if (cameraPath != null && cameraPath.Count() > 0) {
                CameraPath = cameraPath.Select(cp => new NavigationInfo(cp.X, cp.Y, cp.A)).ToList();
            }
        }
    }
}
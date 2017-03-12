using System.Collections.Generic;

namespace ConvergenceEngine.Models.Mapping {

    using Infrastructure.Interfaces;

    public sealed class MapData : IMapData {

        public IEnumerable<ISegment> Segments { get; }
        public IEnumerable<ISegment> CurrentSegments { get; }
        public IEnumerable<INavigationInfo> CameraPath { get; }

        internal MapData(
            IEnumerable<ISegment> segments,
            IEnumerable<ISegment> currentSegments,
            IEnumerable<INavigationInfo> cameraPath) {
            Segments = segments;
            CurrentSegments = currentSegments;
            CameraPath = cameraPath;
        }
    }
}
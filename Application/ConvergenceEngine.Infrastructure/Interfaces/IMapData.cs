using System.Collections.Generic;

namespace ConvergenceEngine.Infrastructure.Interfaces {

    public interface IMapData {

        IEnumerable<ISegment> Segments { get; }
        IEnumerable<ISegment> CurrentSegments { get; }
        IEnumerable<INavigationInfo> CameraPath { get; }
    }
}
using System.Collections.Generic;

namespace ConvergenceEngine.Infrastructure.Interfaces {

    public interface IMap {

        IEnumerable<ISegment> AllSegments { get; }
        IEnumerable<ISegment> CurrentSegments { get; }
        IEnumerable<INavigationInfo> CameraPath { get; }
    }
}
using System.Collections.Generic;

namespace ConvergenceEngine.Infrastructure.Interfaces {

    public interface IMap {

        IEnumerable<ISegment> AllSegments { get; }
        IEnumerable<ISegment> LastSegments { get; }
        IEnumerable<INavigationInfo> CameraPath { get; }
    }
}
using System;
using System.Collections.Generic;
using System.Windows;

namespace ConvergenceEngine.Infrastructure.Interfaces {

    public interface IMapper {

        event Action OnMapperUpdate;

        int ActualFrameIndex { get; set; }
        int AdditionalFrameIndexOffset { get; set; }

        IEnumerable<ISegment> ActualFrame { get; }
        IEnumerable<ISegment> AdditionalFrame { get; }
        IEnumerable<ISegment> Map { get; }

        void HandleNextData(IEnumerable<Point> nextDepthLine);
    }
}
using System;
using System.Collections.Generic;
using System.Windows;

namespace ConvergenceEngine.Infrastructure.Interfaces {

    public interface IMapper {

        event Action<IMapData> OnMapUpdate;
        void HandleNextData(IEnumerable<Point> nextDepthLine);
    }
}
using System;
using System.Collections.Generic;
using System.Windows;

namespace ConvergenceEngine.Infrastructure.Interfaces {

    public interface IMap {

        event Action<IMapData> OnMapUpdate;
        void HandleNextData(IEnumerable<Point> nextDepthLine);
    }
}
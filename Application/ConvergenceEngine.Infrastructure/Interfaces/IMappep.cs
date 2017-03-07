using System;
using System.Collections.Generic;
using System.Windows;

namespace ConvergenceEngine.Infrastructure.Interfaces {

    public interface IMappep {

        event Action<IEnumerable<IMap>> OnMapUpdate;
        void HandleNextData(IEnumerable<Point> nextDepthLine);
    }
}
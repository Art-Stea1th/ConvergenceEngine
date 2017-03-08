using System;
using System.Collections.Generic;
using System.Windows;

namespace ConvergenceEngine.Infrastructure.Interfaces {

    public interface IMapper {

        event Action<IMap> OnMapUpdate;
        void HandleNextData(IEnumerable<Point> nextDepthLine);
    }
}
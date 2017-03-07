using System;
using System.Collections.Generic;
using System.Windows;

namespace ConvergenceEngine.Infrastructure.Interfaces {

    public interface IDataProvider : IDisposable {

        event Action<IEnumerable<Point>> OnNextDepthLineReady;
        event Action<IEnumerable<short>> OnNextFullFrameReady;

        int FrameWidth { get; }
        int FrameHeight { get; }

        int MinDepth { get; }
        int MaxDepth { get; }

        void Start();
        void Stop();
    }
}
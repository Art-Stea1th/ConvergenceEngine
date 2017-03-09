using System;
using System.Collections.Generic;
using System.Windows;

namespace ConvergenceEngine.Infrastructure.Interfaces {

    public interface IDataProvider : IDisposable {

        event Action<IEnumerable<Point>> OnNextDepthLineReady;
        event Action<short[,]> OnNextFullFrameReady;
        event Action<DataProviderStates> OnStateChanged;

        DataProviderStates State { get; }

        int FrameWidth { get; }
        int FrameHeight { get; }

        int MinDepth { get; }
        int MaxDepth { get; }

        double FPS { get; set; }

        void Start();
        void Stop();
    }
}
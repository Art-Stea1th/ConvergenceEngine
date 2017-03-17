using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;

namespace ConvergenceEngine.Infrastructure.Interfaces {

    public interface ISegment : IEquatable<ISegment>, IReadOnlyList<Point>, IReadOnlyCollection<Point>, IEnumerable<Point>, IEnumerable {

        Point A { get; }
        Point B { get; }
    }
}
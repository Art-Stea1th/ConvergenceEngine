using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Windows;

namespace ConvergenceEngine.Models.Mapping.Segments {

    using Extensions;
    using Infrastructure.Interfaces;

    internal sealed partial class Segment : ISegment, IEquatable<ISegment>, IReadOnlyList<Point>, IReadOnlyCollection<Point>, IEnumerable<Point>, IEnumerable {

        public Point A { get; private set; }
        public Point B { get; private set; }
        public Point Center => new Point((A.X + B.X) * 0.5, (A.Y + B.Y) * 0.5);
        public double Length => (B - A).Length;

        internal Segment(IEnumerable<Point> linearOrderedPoints) : this(linearOrderedPoints.ApproximateOrdered()) { }
        internal Segment(ISegment segment) : this(segment.A, segment.B) { }
        internal Segment((Point a, Point b) points) : this(points.a, points.b) { }
        internal Segment(Point pointA, Point pointB) { A = pointA; B = pointB; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private IEnumerable<Point> Points() {
            yield return A; yield return B;
        }

        #region Generic Interfaces

        public int Count => 2;

        public Point this[int index] {
            get { switch (index) { case 0: return A; case 1: return B; default: throw new IndexOutOfRangeException(); } }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerator<Point> GetEnumerator() => Points().GetEnumerator();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator IEnumerable.GetEnumerator() => Points().GetEnumerator();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(ISegment segment) => A == segment.A && B == segment.B || A == segment.B && B == segment.A;
        #endregion
    }
}
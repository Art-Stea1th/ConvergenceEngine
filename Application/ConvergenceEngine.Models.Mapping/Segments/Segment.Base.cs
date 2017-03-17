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
        public Point Center { get { return new Point((A.X + B.X) * 0.5, (A.Y + B.Y) * 0.5); } }
        public double Length { get { return (B - A).Length; } }

        internal Segment(IEnumerable<Point> linearOrderedPoints) : this(linearOrderedPoints.ApproximateOrdered()) { }
        internal Segment(ISegment segment) : this(segment.A, segment.B) { }
        internal Segment(Tuple<Point, Point> points) : this(points.Item1, points.Item2) { }
        internal Segment(Point pointA, Point pointB) { A = pointA; B = pointB; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator Tuple<Point, Point>(Segment segment) {
            return Tuple.Create(segment.A, segment.B);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private IEnumerable<Point> Points() {
            yield return A; yield return B;
        }

        #region Generic Interfaces

        public int Count { get { return 2; } }

        public Point this[int index] {
            get { switch (index) { case 0: return A; case 1: return B; default: throw new IndexOutOfRangeException(); } }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerator<Point> GetEnumerator() {
            return Points().GetEnumerator();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(ISegment segment) {
            return A == segment.A && B == segment.B || A == segment.B && B == segment.A;
        }
        #endregion
    }
}
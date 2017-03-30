using System;
using System.Collections;
using System.Collections.Generic;
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
        internal Segment(Segment segment) : this(segment.A, segment.B) { }
        internal Segment(Point pointA, Point pointB) { A = pointA; B = pointB; }

        public static implicit operator (Point a, Point b) (Segment segment) {
            return (a: segment.A, b: segment.B);
        }

        public static implicit operator (double aX, double aY, double bX, double bY) (Segment segment) {
            return (aX: segment.A.X, aY: segment.A.Y, bX: segment.B.X, bY: segment.B.Y);
        }

        public static implicit operator Segment ((Point a, Point b) points) {
            return new Segment(points.a, points.b);
        }

        public static implicit operator Segment ((double aX, double aY, double bX, double bY) coordinates) {
            return new Segment(new Point(coordinates.aX, coordinates.aY), new Point(coordinates.bX, coordinates.bY));
        }

        private IEnumerable<Point> Points() {
            yield return A; yield return B;
        }

        #region Generic Interfaces

        public int Count => 2;

        public Point this[int index] {
            get { switch (index) { case 0: return A; case 1: return B; default: throw new IndexOutOfRangeException(); } }
        }

        public IEnumerator<Point> GetEnumerator() => Points().GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => Points().GetEnumerator();
        public bool Equals(ISegment segment) => A == segment.A && B == segment.B || A == segment.B && B == segment.A;
        #endregion
    }
}
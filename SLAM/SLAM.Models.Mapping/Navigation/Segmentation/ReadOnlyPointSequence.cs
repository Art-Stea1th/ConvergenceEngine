using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;

namespace SLAM.Models.Mapping.Navigation.Segmentation {

    using Extensions;

    internal abstract class ReadOnlyPointSequence : IReadOnlyList<Point>, IReadOnlyCollection<Point>, IEnumerable<Point> {

        protected readonly List<Point> Points;

        internal ReadOnlyPointSequence(IEnumerable<Point> points) {
            Points = new List<Point>(points);
        }

        public Tuple<IEnumerable<Point>, IEnumerable<Point>> SplitBy(int index) {

            if (Count < 3) { throw new InvalidOperationException(); }
            if (index == 0 || index == Count - 1) { throw new ArgumentException(); }
            if (index < 0 || index >= Count) { throw new ArgumentOutOfRangeException(); }

            IEnumerable<Point> left = Points.TakeWhile((p, i) => i <= index);
            IEnumerable<Point> right = Points.SkipWhile((p, i) => i < index);

            return new Tuple<IEnumerable<Point>, IEnumerable<Point>>(left, right);
        }

        public static Tuple<Point, Point> Approximate(IEnumerable<Point> sequence) {

            Point p0 = sequence.First(), pN = sequence.Last();

            double avgX = sequence.Average(p => p.X);
            double avgY = sequence.Average(p => p.Y);
            double avgXY = sequence.Average(p => p.X * p.Y);
            double avgSqX = sequence.Average(p => Math.Pow(p.X, 2));
            double sqAvgX = Math.Pow(avgX, 2);

            double A = (avgXY - avgX * avgY) / (avgSqX - sqAvgX);
            double B = avgY - A * avgX;

            Point olsP0 = new Point(p0.X, A * p0.X + B), olsPN = new Point(pN.X, A * pN.X + B);

            // Trim Y
            Point resultP0 = new Point(p0.X, p0.DistancePointTo(olsP0, olsPN).Y);
            Point resultPN = new Point(pN.X, pN.DistancePointTo(olsP0, olsPN).Y);

            return new Tuple<Point, Point>(resultP0, resultPN);
        }

        #region Generic Interfaces

        public Point this[int index] { get { return Points[index]; } }

        public int Count { get { return Points.Count; } }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerator<Point> GetEnumerator() {
            return Points.GetEnumerator();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
        #endregion
    }
}
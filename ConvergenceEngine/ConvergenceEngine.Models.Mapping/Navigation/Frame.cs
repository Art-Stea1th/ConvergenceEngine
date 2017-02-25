using System.Collections.Generic;
using System.Windows;

namespace ConvergenceEngine.Models.Mapping.Navigation {

    using Extensions;
    using Segmentation;

    public sealed class Frame : SegmentSequence {

        private List<Point> transformedPoints;
        internal IEnumerable<Point> PointsTransformed { get { return transformedPoints; } }

        public NavigationInfo AbsolutePosition { get; internal set; }
        public NavigationInfo RelativePosition { get; internal set; }

        internal Frame(IEnumerable<Point> points) : base(points) { }

        public void SetPosition(NavigationInfo position) {
            AbsolutePosition = position;
            UpdateTransformedPoints();
        }

        private void UpdateTransformedPoints() { // TMP

            var points = new List<Point>(Points);
            transformedPoints = new List<Point>(points.Count);

            for (int i = 0; i < points.Count; i++) {

                Point p = new Point(points[i].X, points[i].Y);
                p = p.Rotate(AbsolutePosition.A);
                p = new Point(p.X + AbsolutePosition.X, p.Y + AbsolutePosition.Y);

                transformedPoints.Add(p);
                //Console.WriteLine(p);
            }
        }
    }
}
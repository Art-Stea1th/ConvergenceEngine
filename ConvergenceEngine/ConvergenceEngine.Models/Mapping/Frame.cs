using ConvergenceEngine.Models.Mapping.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ConvergenceEngine.Models.Mapping {

    using Collections;
    using Extensions;

    public sealed class Frame : SegmentSequence {

        public readonly int Index;

        public NavigationInfo Relative { get; internal set; }
        public NavigationInfo Absolute { get; internal set; }


        private List<Point> transformedPoints; // TMP
        internal IEnumerable<Point> PointsTransformed { get { return transformedPoints; } } // TMP

        internal Frame(int index, IEnumerable<Point> points) : base(points) {
            Index = index;
        }

        internal void SetAbsoluteNavigationInfo(NavigationInfo navInfo) {
            Absolute = navInfo;
            UpdateTransformedPoints();
        }

        internal void SetRelativeNavigationInfo(NavigationInfo navInfo) {
            Relative = navInfo;
            UpdateTransformedPoints();
        }

        internal void UpdateNavigationInfoBy(Frame frame) {

        }

        private void UpdateTransformedPoints() { // TMP

            var points = new List<Point>(Points);
            transformedPoints = new List<Point>(points.Count);

            for (int i = 0; i < points.Count; i++) {

                Point p = new Point(points[i].X, points[i].Y);
                p = p.Rotate(Absolute.Angle);
                p = new Point(p.X + Absolute.Direction.X, p.Y + Absolute.Direction.Y);

                transformedPoints.Add(p);
            }
        }
    }
}

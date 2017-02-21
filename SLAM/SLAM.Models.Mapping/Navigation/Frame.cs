using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace SLAM.Models.Mapping.Navigation {

    using Extensions;
    using Segmentation;

    internal sealed class Frame : SegmentSequence {

        private List<Point> transformedPoints;
        public IEnumerable<Point> PointsTransformed { get { return transformedPoints; } }

        internal NavigationInfo Absolute { get; set; }

        internal Frame(IEnumerable<Point> points) : base(points) { }

        public void SetPosition(NavigationInfo position) {
            Absolute = position;
            UpdateTransformedPoints();
        }

        private void UpdateTransformedPoints() { // TMP

            var points = new List<Point>(Points);
            transformedPoints = new List<Point>(points.Count);

            for (int i = 0; i < points.Count; i++) {

                Point p = new Point(points[i].X, points[i].Y);
                p = p.Rotate(Absolute.Angle);
                p = new Point(p.X + Absolute.Direction.X, p.Y + Absolute.Direction.Y);

                transformedPoints.Add(p);
                //Console.WriteLine(p);
            }
        }
    }
}
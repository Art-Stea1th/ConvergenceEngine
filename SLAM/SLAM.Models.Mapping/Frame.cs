using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace SLAM.Models.Mapping {

    using Extensions;
    using Navigation;
    using PointSequence = List<Point>;

    internal sealed class Frame {

        public NavigationInfo Absolute { get; private set; }

        private readonly PointSequence points;
        private readonly Lazy<SegmentSequence> segments;

        private PointSequence transformedPoints;

        public IEnumerable<Point> Points { get { return points; } }

        public IEnumerable<Point> PointsTransformed { get { return transformedPoints; } } // ??

        public SegmentSequence Segments {
            get { return segments.Value; }
        }

        public IEnumerable<Tuple<Point, Point>> SegmentsAsEnumerableOfTuple {
            get { return Segments.Select(s => (Tuple<Point, Point>)s); }
        }

        public NavigationInfo GetDifferenceTo(Frame frame) {
            return Segments.GetDifferenceTo(frame.Segments);
        }

        public void SetPosition(NavigationInfo position) {
            Absolute = position;
            UpdateTransformedPoints();
        }

        private void UpdateTransformedPoints() { // ??
            transformedPoints = new PointSequence(points.Count);
            for (int i = 0; i < points.Count; i++) {

                

                Point p = new Point(points[i].X, points[i].Y);
                //p.Rotate(Absolute.Angle);
                //p.X += Absolute.Direction.X;
                //p.Y += Absolute.Direction.Y;


                Matrix m = new Matrix();
                m.Rotate(Absolute.Angle);
                m.Translate(Absolute.Direction.X, -Absolute.Direction.Y);
                p = m.Transform(p);



                transformedPoints.Add(p);

                Console.WriteLine(p);
            }
        }

        public Frame(IEnumerable<Point> sequence) {
            points = new PointSequence(sequence);
            segments = new Lazy<SegmentSequence>(() => SegmentSequence.Segmentate(points));
        }
    }
}
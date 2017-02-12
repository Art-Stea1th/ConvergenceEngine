using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SLAM.Models.Mapping {

    using Navigation;
    using PointSequence = List<Point>;

    internal sealed class Frame {

        private readonly PointSequence points;
        private readonly Lazy<SegmentSequence> segments;

        public IEnumerable<Point> Points { get { return points; } }

        public SegmentSequence Segments {
            get { return segments.Value; }
        }

        public IEnumerable<Tuple<Point, Point>> SegmentsAsEnumerableOfTuple {
            get { return Segments.Select(s => (Tuple<Point, Point>)s); }
        }

        public Vector Location { get; set; }
        public Vector Direction { get; set; }

        public Frame(IEnumerable<Point> sequence) {
            points = new PointSequence(sequence);
            segments = new Lazy<SegmentSequence>(() => SegmentSequence.Segmentate(points));
        }
    }
}
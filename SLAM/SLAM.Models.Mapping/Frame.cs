using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SLAM.Models.Mapping {

    using PointSequence = List<Point>;
    using SegmentSequence = List<Segment>;

    internal sealed class Frame {

        private PointSequence points;
        private SegmentSequence segments;

        public IEnumerable<Point> Points { get { return points; } }

        public IEnumerable<IEnumerable<Point>> GetFrameSegments() {
            List<List<Point>> result = new List<List<Point>>();
            foreach (var segment in segments) {
                result.Add(segment.ToList());
            }
            return result;
        }

        public Frame(IEnumerable<Point> sequence) {
            points = new PointSequence(sequence);
            segments = points.Segmentate().Select(psq => new Segment(psq)).ToList();
        }
    }
}
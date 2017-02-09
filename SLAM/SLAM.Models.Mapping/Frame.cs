using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SLAM.Models.Mapping {

    using PointSequence = IList<Point>;
    using SegmentSequence = IList<Segment>;

    internal sealed class Frame {

        private PointSequence source;
        private SegmentSequence segmented;
    }
}
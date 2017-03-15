using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace ConvergenceEngine.Models.Mapping.Segments {

    using Extensions;

    internal sealed class MapSegment : Segment {

        public int Id { get; set; }
        public double Weight { get; set; }

        internal MapSegment(IEnumerable<Point> linearSortedPoints) : base(linearSortedPoints.ApproximateOrdered()) { }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ConvergenceEngine.Models.Mapping.Segments {

    using Extensions.Ops;

    public sealed class MapSegment : MultiPointsSegment {

        private List<MultiPointsSegment> similar;
        private Segment approximated;

        public int Id { get; }

        public override Point PointA { get { return approximated.PointA; } }
        public override Point PointB { get { return approximated.PointB; } }

        internal MapSegment(int id, MultiPointsSegment segment) : base(segment) {
            Id = id;
            similar = new List<MultiPointsSegment> { segment };
            approximated = new Segment(segment.ApproximateSorted());
        }

        internal void Append(MultiPointsSegment segment) {

            // rotate / shift
            // if (segment.Length > approximated.Length) { }

            similar.Add(segment);
            RecalculateApproximated();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void RecalculateApproximated() {
            approximated = new Segment(similar.SelectMany(mps => mps)
                .OrderByLine(approximated.PointA, approximated.PointB).ApproximateSorted());
        }
    }
}
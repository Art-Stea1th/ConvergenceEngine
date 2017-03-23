using System.Collections.Generic;
using System.Linq;

namespace ConvergenceEngine.Models.Mapping.Segments {

    using Extensions;
    using Infrastructure.Extensions;

    internal partial class Segment {

        public static Segment Merged(IEnumerable<Segment> segments) {

            var primary = segments.MaxBy(s => s.Length);
            return new Segment(segments.SelectMany(p => p).OrderByLine(primary.A, primary.B).ApproximateOrdered());
        }
    }
}
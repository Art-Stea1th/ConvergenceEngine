using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;

namespace ConvergenceEngine.Models.Mapping.Segments {

    using Extensions;
    using Infrastructure.Extensions;
    using System;

    internal partial class Segment {

        public static Segment Merged(IEnumerable<Segment> segments) {

            var primary = segments.MaxBy(s => s.Length);
            return new Segment(segments.SelectMany(p => p).OrderByLine(primary.A, primary.B).ApproximateOrdered());
        }
    }
}
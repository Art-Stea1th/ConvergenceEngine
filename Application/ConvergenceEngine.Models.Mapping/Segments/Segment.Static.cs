using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;

namespace ConvergenceEngine.Models.Mapping.Segments {

    using Extensions;
    using Infrastructure.Extensions;

    internal partial class Segment {

        public static Segment Merged(IEnumerable<Segment> segments) {

            var fullLength = segments.Sum(s => s.Length);                      // 100 %
            var weights = segments.Select(s => s.Length * 100.0 / fullLength); // n % of 100 %

            // Calculate Weighted Average Angle

            var degrees = segments.Select(s => s.AngleToHorizontal);
            var weightedAverageDegreesVector = degrees.Sequential(weights, (a, w) => a * 100.0 / w).Sum().DegreesToVector();

            segments = segments.Select(s => s.RotatedAtCenter(s.AngleTo(weightedAverageDegreesVector)));

            // Calculate Weighted Average Offset

            var weightedAverageX = segments.Sequential(weights, (p, w) => p.Center.X * 100.0 / w).Sum();
            var weightedAverageY = segments.Sequential(weights, (p, w) => p.Center.Y * 100.0 / w).Sum();

            var weightedAveragePosition = new Point(weightedAverageX, weightedAverageY);

            segments = segments.Select(s => s.Shifted(s.Center.DistanceVectorTo(weightedAveragePosition)));

            // Merge

            var primary = segments.MaxBy(s => s.Length);
            return new Segment(segments.SelectMany(s => s).OrderByLine(primary.A, primary.B).ApproximateOrdered());
        }
    }
}
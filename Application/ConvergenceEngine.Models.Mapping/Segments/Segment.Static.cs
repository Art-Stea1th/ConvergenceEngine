using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;

namespace ConvergenceEngine.Models.Mapping.Segments {

    using Infrastructure.Extensions;

    internal partial class Segment {

        

        public static Segment Merged(IEnumerable<Segment> segments) {

            var fullLength = segments.Sum(s => s.Length);                      // 100 %
            var weights = segments.Select(s => s.Length * 100.0 / fullLength); // n % of 100 %

            // Angle

            var degrees = segments.Select(s => s.AngleToHorizontal);
            var weightedAverageDegreesVector = degrees.Sequential(weights, (a, w) => a * 100.0 / w).Sum().DegreesToVector();

            segments = segments.Select(s => s.RotatedAtCenter(s.AngleTo(weightedAverageDegreesVector)));

            // Offset

            var averageX = segments.Average(s => s.Center.X);
            var averageY = segments.Average(s => s.Center.Y);

        }


    }
}
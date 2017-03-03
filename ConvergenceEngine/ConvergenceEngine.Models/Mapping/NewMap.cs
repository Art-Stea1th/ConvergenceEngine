using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ConvergenceEngine.Models.Mapping {

    using Extensions.Ops;

    internal sealed class NewMap {

        private List<IEnumerable<Segment>> segmentsAll;
        private List<Segment> segmentsApproximated;

        private NavigationInfo currentPosition;

        private void UpdateUniqueMapSegments() {

        }

        private void NextFrameProceed(int frameIndex, IEnumerable<Point> framePoints) {

            var currentSegments = Segmentate(framePoints);
            var previousSegments = segmentsApproximated;

            var similarSegments = previousSegments.SelectSimilarTo(currentSegments);


        }

        private IEnumerable<Segment> Segmentate(IEnumerable<Point> points) {
            return points.Segmentate(3.0);
        }

    }
}
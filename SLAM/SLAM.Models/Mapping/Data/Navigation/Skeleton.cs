using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SLAM.Models.Mapping.Data.Navigation {

    using Extensions;

    internal sealed class Skeleton {

        private IList<Point> points;

        //private double threshold = 1.0;
        public List<Tuple<int, int>> segmentIndexes;
        //private List<List<Point>> segments;

        internal List<List<Point>> Segments {
            get {
                List<List<Point>> result = new List<List<Point>>();
                foreach (var segment in segmentIndexes) {
                    result.Add(new List<Point> { points[segment.Item1], points[segment.Item2] });
                }
                return result;
            }
        }

        internal Skeleton(IList<Point> points) {
            BuildFrom(points);
        }

        private void BuildFrom(IList<Point> points) {
            this.points = points;
            segmentIndexes = new List<Tuple<int, int>>();
            FindInterpolationIndexes(/*points,*/ 0, points.Count - 1);
            segmentIndexes.Sort();
        }

        private void FindInterpolationIndexes(/*IList<Point> points,*/ int firstIndex, int lastIndex) { // quick impl.

            if (!segmentIndexes.Exists(si => si.Item1 == firstIndex && si.Item2 == lastIndex)) {
                segmentIndexes.Add(new Tuple<int, int>(firstIndex, lastIndex));
            }

            double maxDistance = 0.0;
            int maxDistancePointIndex = 0;

            for (int i = firstIndex; i < lastIndex; ++i) {
                double currentDistance = points[i].DistanceTo(points[firstIndex], points[lastIndex]);
                if (currentDistance > maxDistance) {
                    maxDistance = currentDistance;
                    maxDistancePointIndex = i;
                }
            }

            if (maxDistance >= points[maxDistancePointIndex].Y * 0.02) {
                segmentIndexes.RemoveAll(si => si.Item1 == firstIndex && si.Item2 == lastIndex);

                segmentIndexes.Add(new Tuple<int, int>(firstIndex, maxDistancePointIndex));
                segmentIndexes.Add(new Tuple<int, int>(maxDistancePointIndex, lastIndex));

                FindInterpolationIndexes(/*points,*/ firstIndex, maxDistancePointIndex);
                FindInterpolationIndexes(/*points,*/ maxDistancePointIndex, lastIndex);
            }
            //segmentIndexes.RemoveAll(i => points[i.Item1].DistanceTo(points[i.Item2]) / (i.Item2 - i.Item1) > points[maxDistancePointIndex].Y * 0.03);
        }
    }
}
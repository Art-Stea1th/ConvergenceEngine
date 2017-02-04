using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SLAM.Models.Mapping.Data {

    using Navigation;

    internal sealed class Frame : IEnumerable<Point> {

        private List<Point> points;
        private Skeleton navSkeleton;



        public static explicit operator List<Point>(Frame frame) {
            return frame.points.ToList();
        }

        public static explicit operator Point[] (Frame frame) {
            return frame.points.ToArray();
        }

        public static implicit operator Frame(List<Point> points) {
            return CreateFrom(points);
        }

        public static implicit operator Frame(Point[] points) {
            return CreateFrom(points);
        }

        public static Frame CreateFrom(IEnumerable<Point> points) {
            return new Frame(points);
        }

        private Frame(IEnumerable<Point> points) {
            this.points = new List<Point>(points);
        }

        public IEnumerator<Point> GetEnumerator() {
            return points.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return points.GetEnumerator();
        }
    }
}
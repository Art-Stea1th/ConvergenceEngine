using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace SLAM.Models.Mapping {

    internal sealed class Segment {

        public Point PointA { get; }
        public Point PointB { get; }

        public static implicit operator Segment(Tuple<Point, Point> poinsPair) {
            return new Segment(poinsPair);
        }

        public static explicit operator Tuple<Point, Point>(Segment doublePointSegment) {
            return new Tuple<Point, Point>(doublePointSegment.PointA, doublePointSegment.PointB);
        }

        public Segment(ICollection<Point> sequence) {
            PointA = sequence.First();
            PointB = sequence.Last();
        }

        public Segment(Tuple<Point, Point> pointsPair) {
            PointA = pointsPair.Item1;
            PointB = pointsPair.Item2;
        }

        public Segment(Point pointA, Point pointB) {
            PointA = pointA;
            PointB = PointB;
        }
    }
}
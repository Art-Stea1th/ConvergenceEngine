using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Windows;

namespace SLAM.Models.Map.BrutforceMapperResources {

    internal sealed class IntersectionsSeeker {

        private float errorLimit;

        internal IntersectionsSeeker(float errorLimit = 0.001f) { this.errorLimit = errorLimit; }

        public List<Point> MergePoints(IList<Point> source, IList<Point> target) {

            List<Point> result = new List<Point>(target);

            foreach (var point in source) {
                result.Add(point);
            }
            return result;
        }

        public List<Point> GetDifference(IEnumerable<Point> prevBuffer, IEnumerable<Point> currBuffer, float XError, float YError) {

            List<Point> result = new List<Point>();

            foreach (var point in currBuffer) {
                if (!PointInSequenceExists(prevBuffer, point, XError, YError)) {
                    result.Add(point);
                }
            }
            return result;
        }

        public bool FindNearestPoints(IList<Point> pointsA, IList<Point> pointsB, out int indexA, out int indexB, float limit) {

            indexA = indexB = 0;
            Point minDiff = AbsoluteDifference(pointsA[indexA], pointsB[indexB]);

            for (int a = 0; a < pointsA.Count; ++a) {
                for (int b = 0; b < pointsB.Count; ++b) {
                    Point currDiff = AbsoluteDifference(pointsA[a], pointsB[b]);
                    if (currDiff.X <= minDiff.X && currDiff.Y <= minDiff.Y) {
                        minDiff = currDiff;
                        indexA = a; indexB = b;
                    }
                }
            }
            if (minDiff.X < limit) {
                return true;
            }
            return false;
        }

        public List<Point> NormalizedFrame(IEnumerable<Point> points, float XError, float YError) {

            List<Point> result = new List<Point>();

            foreach (var point in points) {
                if (!PointInSequenceExists(result, point, XError, YError)) {
                    result.Add(point);
                }
            }
            return result;
        }

        public bool PointInSequenceExists(IEnumerable<Point> sequence, Point point, float XError, float YError) {
            foreach (var sPoint in sequence) {
                if (HitPoint(point, sPoint, XError, YError)) {
                    return true;
                }
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool HitPoint(Point pointA, Point pointB, float XError, float YError) {
            Point absoluteDifference = AbsoluteDifference(pointA, pointB);
            return absoluteDifference.X < XError && absoluteDifference.Y < YError;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Point AbsoluteDifference(Point pointA, Point pointB) {
            pointA.X = Math.Abs(pointA.X - pointB.X);
            pointA.Y = Math.Abs(pointA.Y - pointB.Y);
            if (pointA.X == 0.0) { pointA.X += errorLimit; }
            if (pointA.Y == 0.0) { pointA.Y += errorLimit; }
            return pointA;
        }
    }
}
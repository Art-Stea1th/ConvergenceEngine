using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Windows;

namespace SLAM.Models.MapModel.MapperResources {

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

        public List<Point> GetDifference(IEnumerable<Point> prevBuffer, IEnumerable<Point> currBuffer, float error) {

            List<Point> result = new List<Point>();

            foreach (var point in currBuffer) {
                if (!PointInSequenceExists(prevBuffer, point, error)) {
                    result.Add(point);
                }
            }
            return result;
        }

        public bool FindNearestPoints(IList<Point> pointsA, IList<Point> pointsB, out int indexA, out int indexB, float limit) {

            indexA = indexB = 0;
            float minimalDistance = Distance(pointsA[indexA], pointsB[indexB]);

            for (int a = 0; a < pointsA.Count; ++a) {
                for (int b = 0; b < pointsB.Count; ++b) {
                    float currentDistance = Distance(pointsA[a], pointsB[b]);
                    if (currentDistance <= minimalDistance) {
                        minimalDistance = currentDistance;
                        indexA = a; indexB = b;
                    }
                }
            }
            if (minimalDistance < limit) {
                return true;
            }
            return false;
        }

        public List<Point> NormalizedFrame(IEnumerable<Point> points, float error) {

            List<Point> result = new List<Point>();

            foreach (var point in points) {
                if (!PointInSequenceExists(result, point, error)) {
                    result.Add(point);
                }
            }
            return result;
        }

        public bool PointInSequenceExists(IEnumerable<Point> sequence, Point point, float error) {
            foreach (var sPoint in sequence) {
                if (HitPoint(point, sPoint, error)) {
                    return true;
                }
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool HitPoint(Point pointA, Point pointB, float error) {
            float absoluteDifference = Distance(pointA, pointB);
            return absoluteDifference < error;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float Distance(Point pointA, Point pointB) {

            return (float)Math.Sqrt(
                Math.Pow(pointB.X - pointA.X, 2.0) +
                Math.Pow(pointB.Y - pointA.Y, 2.0)
                );
        }
    }
}
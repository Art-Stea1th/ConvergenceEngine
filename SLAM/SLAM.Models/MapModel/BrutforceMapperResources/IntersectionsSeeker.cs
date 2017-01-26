using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Windows;

namespace SLAM.Models.MapModel.ComplexMapperResources {

    internal sealed class IntersectionsSeeker {

        public List<Point> MergePoints(IList<Point> source, IList<Point> target) {

            List<Point> result = new List<Point>(target);

            foreach (var point in source) {
                result.Add(point);
            }
            return result;
        }

        public List<Point> GetDifference(IEnumerable<Point> prevBuffer, IEnumerable<Point> currBuffer, float threshold) {

            List<Point> result = new List<Point>();

            foreach (var point in currBuffer) {
                if (!PointInSequenceExists(prevBuffer, point, threshold)) {
                    result.Add(point);
                }
            }
            return result;
        }

        public bool FindNearestPoints(IList<Point> pointsA, IList<Point> pointsB, out int indexA, out int indexB, float threshold) {

            indexA = indexB = 0;
            float minimalDistance = GetDistance(pointsA[indexA], pointsB[indexB]);

            for (int a = 0; a < pointsA.Count; ++a) {
                for (int b = 0; b < pointsB.Count; ++b) {
                    float currentDistance = GetDistance(pointsA[a], pointsB[b]);
                    if (currentDistance <= minimalDistance) {
                        minimalDistance = currentDistance;
                        indexA = a; indexB = b;
                    }
                }
            }
            if (minimalDistance < threshold) {
                return true;
            }
            return false;
        }

        public List<Point> NormalizedFrame(IEnumerable<Point> points, float threshold) {

            List<Point> result = new List<Point>();

            foreach (var point in points) {
                if (!PointInSequenceExists(result, point, threshold)) {
                    result.Add(point);
                }
            }
            return result;
        }

        public bool PointInSequenceExists(IEnumerable<Point> sequence, Point point, float threshold) {
            foreach (var sPoint in sequence) {
                if (HitPoint(point, sPoint, threshold)) {
                    return true;
                }
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool HitPoint(Point pointA, Point pointB, float threshold) {
            float absoluteDifference = GetDistance(pointA, pointB);
            return absoluteDifference < threshold;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float GetDistance(Point pointA, Point pointB) {

            return (float)Math.Sqrt(
                Math.Pow(pointB.X - pointA.X, 2.0) +
                Math.Pow(pointB.Y - pointA.Y, 2.0)
                );
        }
    }
}
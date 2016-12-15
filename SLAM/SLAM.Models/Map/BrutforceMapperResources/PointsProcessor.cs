using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;

namespace SLAM.Models.Map.BrutforceMapperResources {

    internal sealed class PointsProcessor {


        public void BuildLine(IList<Point> points, int startIndex, out int endIndex) {

        }

        public void Transform(Point[] points, float x, float y, float angle) {
            Matrix matrix = new Matrix();
            matrix.Rotate(angle);
            matrix.Translate(x, y);
            matrix.Transform(points);
        }

        public void Rotate(Point[] points, float angle) {
            Matrix matrix = new Matrix();
            matrix.Rotate(angle);
            matrix.Transform(points);
        }

        public void ShiftXY(Point[] points, float x, float y) {
            Matrix matrix = new Matrix();
            matrix.Translate(x, y);
            matrix.Transform(points);
        }

        public void ShiftX(Point[] points, float x) {
            Matrix matrix = new Matrix();
            matrix.Translate(x, 0.0);
            matrix.Transform(points);
        }

        public void ShiftY(Point[] points, float y) {
            Matrix matrix = new Matrix();
            matrix.Translate(0.0, y);
            matrix.Transform(points);
        }

        public List<Point> MergePoints(IList<Point> source, IList<Point> target) {

            List<Point> result = new List<Point>(target);

            foreach (var point in source) {
                result.Add(point);
            }
            return result;
        }

        public List<Point> GetDifference(IEnumerable<Point> prevBuffer, IEnumerable<Point> currBuffer, float Xlimit, float Ylimit) {

            List<Point> result = new List<Point>();

            foreach (var point in currBuffer) {
                if (!PointInSequenceExists(prevBuffer, point, Xlimit, Ylimit)) {
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

        public List<Point> NormalizedFrame(IEnumerable<Point> points, float Xlimit, float Ylimit) {

            List<Point> result = new List<Point>();

            foreach (var point in points) {
                if (!PointInSequenceExists(result, point, Xlimit, Ylimit)) {
                    result.Add(point);
                }
            }
            return result;
        }

        public bool PointInSequenceExists(IEnumerable<Point> sequence, Point point, float Xlimit, float Ylimit) {
            foreach (var sPoint in sequence) {
                if (HitPoint(point, sPoint, Xlimit, Ylimit)) {
                    return true;
                }
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool HitPoint(Point pointA, Point pointB, float Xlimit, float Ylimit) {
            Point absoluteDifference = AbsoluteDifference(pointA, pointB);
            return absoluteDifference.X < Xlimit && absoluteDifference.Y < Ylimit;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Point AbsoluteDifference(Point pointA, Point pointB) {
            pointA.X = Math.Abs(pointA.X - pointB.X);
            pointA.Y = Math.Abs(pointA.Y - pointB.Y);
            return pointA;
        }
    }
}
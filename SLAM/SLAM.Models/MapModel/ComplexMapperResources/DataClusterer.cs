using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Windows;


namespace SLAM.Models.MapModel.ComplexMapperResources {

    internal sealed class DataClusterer {

        /// <summary>
        /// Clusterize by the distance between points in the ordered collection of points
        /// </summary>
        /// <param name="points">Points collection</param>
        /// <param name="threshold">Distance threshold</param>
        /// <returns>returns the indices of individual segments</returns>
        public IEnumerable<int> SplitByDistance(IList<Point> points, float threshold) {

            var resultIndices = new List<int> { 0 };

            for (int i = 1; i < points.Count; ++i) {
                if (GetDistance(points[i - 1], points[i]) > threshold) {
                    resultIndices.Add(i);
                }
            }
            return resultIndices;
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
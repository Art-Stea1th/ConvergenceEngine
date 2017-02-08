using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace SLAM.Models.Mapping.Data.Navigation {

    internal sealed class Approximator {

        public Tuple<Point, Point> ApproximateByOrdinaryLeastSquares(ICollection<Point> sequence) {

            Point p0 = sequence.First(), pN = sequence.Last();

            double avgX = sequence.Average(p => p.X);
            double avgY = sequence.Average(p => p.Y);
            double avgXY = sequence.Average(p => p.X * p.Y);
            double avgSqX = sequence.Average(p => Math.Pow(p.X, 2));
            double sqAvgX = Math.Pow(avgX, 2);

            double A = (avgXY - avgX * avgY) / (avgSqX - sqAvgX);
            double B = avgY - A * avgX;

            return new Tuple<Point, Point>(new Point(p0.X, A * p0.X + B), new Point(pN.X, A * pN.X + B));
        }
    }
}
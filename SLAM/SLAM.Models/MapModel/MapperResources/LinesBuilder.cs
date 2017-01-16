using System;
using System.Collections.Generic;
using System.Windows;


namespace SLAM.Models.MapModel.MapperResources {

    internal sealed class LinesBuilder {

        public void BuildLine(IList<Point> points, int startIndex, out int endIndex) {

            throw new NotImplementedException();

            endIndex = startIndex;

            double errorLimit = 1.0;
            int minLength = 2;

            // forward direction
            int forwardLength = 0;
            if ((startIndex + minLength) < points.Count) {
                Vector forwardRatio = points[startIndex + 1] - points[startIndex];
                ++forwardLength;
                for (int i = startIndex + minLength; i < points.Count; ++i) {

                    double currentRatioX = points[i].X / forwardRatio.X; // add NonZero check!
                    double currentRatioY = points[i].Y / forwardRatio.Y; // add NonZero check!

                    if (Math.Abs(currentRatioX - currentRatioY) < errorLimit) {
                        ++forwardLength;
                    }
                }
            }

            // backward direction
            int backwardLength = 0;
            if ((startIndex - minLength) >= 0) {
                Vector backwardRatio = points[startIndex] - points[startIndex - 1];
                for (int i = startIndex - 1; i >= 0; --i) {

                    double currentRatioX = points[i].X / backwardRatio.X; // add NonZero check!
                    double currentRatioY = points[i].Y / backwardRatio.Y; // add NonZero check!

                    if (Math.Abs(currentRatioX - currentRatioX) < errorLimit) {
                        ++backwardLength;
                    }
                }
            }
        }
    }
}
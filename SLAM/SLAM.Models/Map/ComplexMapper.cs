using System;
using System.Collections.Generic;
using System.Windows;


namespace SLAM.Models.Map {

    using Data.Readers;
    using BrutforceMapperResources;    

    internal sealed class ComplexMapper : BaseMapper {

        private Point[] prevFrame; // previous
        private Point[] currFrame; // current

        private VirtualOdometry odometry;
        private AnglesCalculator anglesCalculator;
        private PointsTransformer pointsTransformer;
        private IntersectionsSeeker intersectionSeeker;

        public ComplexMapper(DataProvider dataProvider) : base(dataProvider) {
            odometry = new VirtualOdometry();
            anglesCalculator = new AnglesCalculator();
            pointsTransformer = new PointsTransformer();
            intersectionSeeker = new IntersectionsSeeker();
        }

        protected override void NextFrameProceed() {
            DataProvider.GetNextFrameTo(out currFrame);
            currFrame = intersectionSeeker.NormalizedFrame(currFrame, 2.0f, 2.0f).ToArray();
            //ResultMap = currFrame;
            AddNextFrameToResultMap();
        }

        private void AddNextFrameToResultMap() {
            if (prevFrame == null || ResultMap == null) {

                prevFrame = new Point[currFrame.Length];
                Array.Copy(currFrame, prevFrame, currFrame.Length);

                ResultMap = new Point[currFrame.Length];
                Array.Copy(currFrame, ResultMap, currFrame.Length);
                return;
            }
            BrutforceNextMoving();
            prevFrame = currFrame;
        }

        private void BrutforceNextMoving() {

            float XYlimit = 0.5f;

            List<Point> minDifference = intersectionSeeker.GetDifference(prevFrame, currFrame, XYlimit, XYlimit);

            float expX = 0.0f, expY = 0.0f, expA = 0.0f;

            // --- Approximate by Odometry

            if (!odometry.Zero()) {

                float odoX = odometry.ExpectedX;
                float odoY = odometry.ExpectedY;
                float odoA = odometry.ExpectedA;

                pointsTransformer.Transform(prevFrame, odoX, odoY, odoA);

                List<Point> odoDifference = intersectionSeeker.GetDifference(prevFrame, currFrame, XYlimit, XYlimit);
                if (odoDifference.Count < minDifference.Count) {
                    minDifference = odoDifference;
                    expX += odoX; expY += odoY; expA += odoA;
                }
                else {
                    pointsTransformer.Transform(prevFrame, -expX, -expY, -expA);
                }
                ///Console.WriteLine($"Odometry\t-- x: {expX}, y: {expY}, angle: {expA} --");
            }
            else {
                ///Console.WriteLine("Odo err;");
            }

            // --- Approximate by Nearest Points

            int prevNPIndex = 0, currNPIndex = 0;
            if (intersectionSeeker.FindNearestPoints(prevFrame, currFrame, out prevNPIndex, out currNPIndex, 14.0f)) {

                float npX = (float)(currFrame[currNPIndex].X - prevFrame[prevNPIndex].X);
                float npY = (float)(currFrame[currNPIndex].Y - prevFrame[prevNPIndex].Y);

                pointsTransformer.ShiftXY(prevFrame, -npX, -npY);

                List<Point> npDifference = intersectionSeeker.GetDifference(prevFrame, currFrame, XYlimit, XYlimit);

                if (npDifference.Count < minDifference.Count) {
                    minDifference = npDifference;
                    expX -= npX; expY -= npY;
                }
                else {
                    pointsTransformer.ShiftXY(prevFrame, +npX, +npY);
                }
                ///Console.WriteLine($"Nearest\t-- x: {expX}, y: {expY}, angle: {expA} --");
            }
            else {
                ///Console.WriteLine("Nea err;");
            }

            // --- Approximate by Calculated Angle

            int endIndex;
            //pointsProc.BuildLine(prevFrame, prevNPIndex, out endIndex);

            // --- Apply final Result

            pointsTransformer.Transform(ResultMap, expX, expY, expA);
            ResultMap = intersectionSeeker.MergePoints(minDifference, ResultMap).ToArray();

            //odometry.SetLastMove(expX, expY, expA);            
            ///ResultMap = minDifference.ToArray();
        }
    }
}
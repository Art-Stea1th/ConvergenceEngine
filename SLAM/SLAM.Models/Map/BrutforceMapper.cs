using System;
using System.Collections.Generic;
using System.Windows;


namespace SLAM.Models.Map {

    using Data.Readers;
    using BrutforceMapperResources;    

    internal sealed class BrutforceMapper : BaseMapper {

        private Point[] prevFrame; // previous
        private Point[] currFrame; // current

        private VirtualOdometry odometry;
        private AngleCalculator angleCalc;
        private PointsProcessor pointsProc;

        public BrutforceMapper(DataProvider dataProvider) : base(dataProvider) {
            odometry = new VirtualOdometry();
            angleCalc = new AngleCalculator();
            pointsProc = new PointsProcessor();
        }

        protected override void NextFrameProceed() {
            DataProvider.GetNextFrameTo(out currFrame);
            currFrame = pointsProc.NormalizedFrame(currFrame, 2.0f, 2.0f).ToArray();            
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

            float XYlimit = 1.0f;

            List<Point> minDifference = pointsProc.GetDifference(prevFrame, currFrame, XYlimit, XYlimit);

            float expX = 0.0f, expY = 0.0f, expA = 0.0f;

            // Approximate by Odometry

            if (!odometry.Zero()) {

                float odoX = odometry.ExpectedX;
                float odoY = odometry.ExpectedY;
                float odoA = odometry.ExpectedA;

                pointsProc.Transform(prevFrame, odoX, odoY, odoA);

                List<Point> odoDifference = pointsProc.GetDifference(prevFrame, currFrame, XYlimit, XYlimit);
                if (odoDifference.Count < minDifference.Count) {
                    minDifference = odoDifference;
                    expX += odoX; expY += odoY; expA += odoA;
                }
                else {
                    pointsProc.Transform(prevFrame, -expX, -expY, -expA);
                }
                //Console.WriteLine($"Odometry\t-- x: {expX}, y: {expY}, angle: {expA} --");
            }

            // Approximate by Nearest Points

            int prevNPIndex, currNPIndex;
            if (pointsProc.FindNearestPoints(prevFrame, currFrame, out prevNPIndex, out currNPIndex, 2.0f)) {

                float npX = (float)(currFrame[currNPIndex].X - prevFrame[prevNPIndex].X);
                float npY = (float)(currFrame[currNPIndex].Y - prevFrame[prevNPIndex].Y);

                pointsProc.ShiftXY(prevFrame, npX, npY);

                List<Point> npDifference = pointsProc.GetDifference(prevFrame, currFrame, XYlimit, XYlimit);

                if (npDifference.Count < minDifference.Count) {
                    minDifference = npDifference;
                    expX += npX; expY += npY;
                }
                else {
                    pointsProc.ShiftXY(prevFrame, -npX, -npY);
                }
                //Console.WriteLine($"Nearest\t-- x: {expX}, y: {expY}, angle: {expA} --");
            }

            // Approximate by Calculated Angle



            // Apply final Result

            pointsProc.Transform(ResultMap, expX, expY, expA);
            ResultMap = pointsProc.MergePoints(minDifference, ResultMap).ToArray();

            odometry.SetLastMove(expX, expY, expA);            
            //ResultMap = minDifference.ToArray();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Windows;


namespace SLAM.Models.MapModel {

    using DataModel.Readers;
    using MapperResources;    

    internal sealed class ComplexMapper : BaseMapper {

        private Point[] prevFrame; // previous
        private Point[] currFrame; // current

        private OffsetPredictor predictor;
        private AnglesCalculator anglesCalculator;
        private PointsTransformer pointsTransformer;
        private IntersectionsSeeker intersectionSeeker;

        public ComplexMapper(DataProvider dataProvider) : base(dataProvider) {
            predictor = new OffsetPredictor();
            anglesCalculator = new AnglesCalculator();
            pointsTransformer = new PointsTransformer();
            intersectionSeeker = new IntersectionsSeeker();
        }

        protected override void NextFrameProceed() {
            DataProvider.GetNextFrameTo(out currFrame);
            currFrame = intersectionSeeker.NormalizedFrame(currFrame, 2.0f, 2.0f).ToArray();
            ResultMap = currFrame;
            //AddNextFrameToResultMap();
        }

        private void AddNextFrameToResultMap() {
            if (prevFrame == null || ResultMap == null) {

                prevFrame = new Point[currFrame.Length];
                Array.Copy(currFrame, prevFrame, currFrame.Length);

                ResultMap = new Point[currFrame.Length];
                Array.Copy(currFrame, ResultMap, currFrame.Length);
                return;
            }
            CalculateNextMoving();
            prevFrame = currFrame;
        }

        private void CalculateNextMoving() {
            throw new NotImplementedException();
        }        
    }
}
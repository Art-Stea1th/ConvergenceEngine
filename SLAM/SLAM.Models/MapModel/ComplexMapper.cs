using System;
using System.Collections.Generic;
using System.Windows;

namespace SLAM.Models.MapModel {

    using DataModel.Readers;
    using ComplexMapperResources;

    internal sealed class ComplexMapper : BaseMapper {

        private Point[] prevFrame; // previous
        private Point[] currFrame; // current

        public ComplexMapper(DataProvider dataProvider) : base(dataProvider) {

        }

        protected override void NextFrameProceed() {
            DataProvider.GetNextFrameTo(out currFrame);
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

            // step 0  : beautify data
            //      0a : remove artifacts
            //      0b : normalize by distance or not? =)

            // step 1  : conditionally divided into segments (by distance)

            // step 2  : approximate segments by Ordinary Least Squares
            //      2a : cut segments from lines by perpendicular from the start and end points

            // step 3  : conditionally divided into segments (by threshold angle)

            // step 4  : approximate segments by Ordinary Least Squares

            // step 5  : calculate offset & rotation

        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace SLAM.Models.Mapping {

    using Extensions;
    using IO.Readers;
    using ComplexMapperResources;    

    internal sealed class ComplexMapper : BaseMapper {

        private List<Point> prevFrame; // previous
        private List<Point> currFrame; // current

        private DataBeautifier dataBeautifier;

        public ComplexMapper(DataProvider dataProvider) : base(dataProvider) {
            dataBeautifier = new DataBeautifier();
        }

        protected override void NextFrameProceed() {

            Point[] buffer;
            DataProvider.GetNextFrameTo(out buffer);

            ResultMap = new List<Point>(buffer);
            return;

            currFrame =
                dataBeautifier.RemoveArtifacts(
                    dataBeautifier.NormalizeFrame(buffer, 3.0), 2, 7.5) as List<Point>;
            ResultMap = currFrame;
        }

        private void AddNextFrameToResultMap() {
            if (prevFrame.IsNullOrEmpty() || ResultMap.IsNullOrEmpty()) {

                prevFrame = new List<Point>(currFrame.AsEnumerable());
                ResultMap = new List<Point>(currFrame.AsEnumerable());
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
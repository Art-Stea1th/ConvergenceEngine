using System.Collections.Generic;
using System.Windows;

namespace SLAM.Models.Mapping {

    using IO.Readers;
    using IO.DataExtractors;

    internal sealed class Mapper {

        private DataProvider dataProvider;
        private byte[] currentFrameBuffer;

        private KinectMiddleLineFrameExtractor middleLineExtractor;

        public Map Map { get; private set; }

        public List<Point> ResultMap { get; private set; }

        public Mapper(DataProvider dataProvider) {
            this.dataProvider = dataProvider;
            dataProvider.OnNextFrameReady += NextFrameProceed;
            middleLineExtractor = new KinectMiddleLineFrameExtractor(this.dataProvider);
        }

        public void NextFrameProceed() {

            dataProvider.GetNextRawFrameTo(out currentFrameBuffer);

            ResultMap = new List<Point>(middleLineExtractor.ExtractActualMiddleLineDepthFrame(currentFrameBuffer));
            return;
        }

        internal Point[] GetActualMapFrame() {
            return ResultMap.ToArray();
        }
    }
}
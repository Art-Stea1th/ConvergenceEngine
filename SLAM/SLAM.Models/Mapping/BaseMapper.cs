using System.Collections.Generic;
using System.Windows;

namespace SLAM.Models.Mapping {

    using IO.Readers;

    internal abstract class BaseMapper {

        public DataProvider DataProvider { get; private set; }
        public List<Point> ResultMap { get; protected set; }        

        internal BaseMapper(DataProvider dataProvider) {
            DataProvider = dataProvider;
            DataProvider.OnNextFrameReady += NextFrameProceed;
        }

        protected abstract void NextFrameProceed();
    }
}
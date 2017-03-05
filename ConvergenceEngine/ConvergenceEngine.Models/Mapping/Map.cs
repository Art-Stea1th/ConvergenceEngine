using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace ConvergenceEngine.Models.Mapping {

    using IO.DataExtractors;
    using IO.Readers;

    public sealed class Map : MapBase {

        private DataProvider dataProvider;
        private byte[] currentFrameBuffer;

        private MiddleLineFrameExtractor middleLineExtractor;
        public event Action OnFrameUpdate;

        internal Map(DataProvider dataProvider) {
            ReInitializeData(dataProvider);
        }

        internal void ReInitializeData(DataProvider dataProvider) {
            this.dataProvider = dataProvider;
            dataProvider.OnNextFrameReady += Update;
            ClearData();
        }

        private void Update() {
            if (middleLineExtractor == null) { // TMP
                middleLineExtractor = new MiddleLineFrameExtractor(dataProvider.FrameInfo);
            }

            dataProvider.GetNextRawFrameTo(out currentFrameBuffer);
            var points = middleLineExtractor.ExtractMiddleLine(currentFrameBuffer);

            if (points.Length < 1) {
                return;
            }

            NextFrameDataProceed(points);
            OnFrameUpdate.Invoke();
        }
    }
}
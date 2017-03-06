using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace ConvergenceEngine.Models.Mapping {

    using IO.DataExtractors;
    using IO.Readers;
    using System.Runtime.CompilerServices;

    public sealed class Mapper {

        private DataProvider dataProvider;
        private byte[] currentFrameBuffer;

        private MiddleLineFrameExtractor middleLineExtractor;

        public event Action OnFrameUpdate;
        public int ActualIndex { get; private set; }
        public Map Map { get; private set; }

        internal Mapper(DataProvider dataProvider) {
            Map = new Map();
            ReInitializeData(dataProvider);
        }

        internal void ReInitializeData(DataProvider dataProvider) {
            this.dataProvider = dataProvider;
            dataProvider.OnNextFrameReady += Update;
            ActualIndex = -1;
            Map.ClearData();
        }

        private void Update() {
            if (middleLineExtractor == null) { // TMP
                middleLineExtractor = new MiddleLineFrameExtractor(dataProvider.FrameInfo);
            }

            if (CorrectIndex(dataProvider.FrameIndex)) {
                ActualIndex = dataProvider.FrameIndex;

                dataProvider.GetNextRawFrameTo(out currentFrameBuffer);
                var points = middleLineExtractor.ExtractMiddleLine(currentFrameBuffer);

                if (points.Length < 1) {
                    return;
                }
                Map.NextFrameDataProceed(points);
                OnFrameUpdate?.Invoke();
                return;
            }
            Console.WriteLine($"{dataProvider.FrameIndex}, {ActualIndex}");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool CorrectIndex(int nextIndex) {
            return nextIndex - ActualIndex == 1 ? true : false;
        }
    }
}
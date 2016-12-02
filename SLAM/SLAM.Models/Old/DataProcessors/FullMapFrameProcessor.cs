using System;

namespace SLAM.Models.Old.DataProcessors {

    internal sealed class FullMapFrameProcessor : DataProcessor, IDataProcessor {

        public FullMapFrameProcessor(DepthFrameSequenceInfo frameInfo) : base(frameInfo) { }

        public override void CalculateViewportFrame(byte[] rawInput, byte[] viewportOutput) {
            throw new NotImplementedException();
        }
    }
}
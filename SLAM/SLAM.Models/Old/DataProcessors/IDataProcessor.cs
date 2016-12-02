namespace SLAM.Models.Old.DataProcessors {


    internal interface IDataProcessor {

        DepthFrameSequenceInfo FrameInfo { get; }

        void CalculateViewportFrame(byte[] rawIn, byte[] viewportOut);
    }
}
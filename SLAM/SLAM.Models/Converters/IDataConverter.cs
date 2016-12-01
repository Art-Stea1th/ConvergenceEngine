namespace SLAM.Models.Converters {


    internal interface IDataConverter {

        DepthFrameSequenceInfo FrameInfo { get; }

        void ConvertRawDataToViewportFrame(byte[] rawIn, byte[] viewportOut);
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SLAM.Models.Converters {

    internal interface IDataConverter {

        DepthFrameSequenceInfo FrameInfo { get; }

        void RawFrameToFullFrame(byte[] inRawFrame, byte[] outViewportFullFrameBuffer);
        void RawFrameToCurveFrame(byte[] inRawFrame, byte[] outViewportFullFrameBuffer);
    }
}
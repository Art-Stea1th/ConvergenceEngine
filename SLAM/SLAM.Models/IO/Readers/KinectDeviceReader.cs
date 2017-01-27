using System;
using SLAM.Models.IO.Adapters;

namespace SLAM.Models.IO.Readers {

    internal sealed class KinectDeviceReader : DeviceReader, IDisposable {

        public KinectDeviceReader() {
            adapter = new KinectDataAdapter(this);
        }
    }
}
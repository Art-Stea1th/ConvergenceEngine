using System;
using SLAM.Models.Data.Adapters;

namespace SLAM.Models.Data.Readers {

    internal sealed class KinectDeviceReader : DeviceReader, IDisposable {

        public KinectDeviceReader() {
            adapter = new KinectDataAdapter(this);
        }
    }
}
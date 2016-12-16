using System;
using SLAM.Models.DataModel.Adapters;

namespace SLAM.Models.DataModel.Readers {

    internal sealed class KinectDeviceReader : DeviceReader, IDisposable {

        public KinectDeviceReader() {
            adapter = new KinectDataAdapter(this);
        }
    }
}
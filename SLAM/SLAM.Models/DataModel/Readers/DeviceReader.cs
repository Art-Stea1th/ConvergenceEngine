using System;
using SLAM.Models.DataModel.Adapters;

namespace SLAM.Models.DataModel.Readers {

    internal abstract class DeviceReader : DataProvider, IDisposable {

        internal override bool Start(string param = null) {
            throw new NotImplementedException();
        }

        internal override void Stop() {
            throw new NotImplementedException();
        }

        public override void Dispose() {
            throw new NotImplementedException();
        }        
    }
}
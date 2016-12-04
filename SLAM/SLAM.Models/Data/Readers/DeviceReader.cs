using System;
using SLAM.Models.Data.Adapters;

namespace SLAM.Models.Data.Readers {

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
using System;


namespace SLAM.Models.DataModel.Writers {

    internal abstract class BaseWriter : IDisposable {

        public abstract void Dispose();
    }
}
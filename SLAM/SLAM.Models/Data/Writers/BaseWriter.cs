using System;


namespace SLAM.Models.Data.Writers {

    internal abstract class BaseWriter : IDisposable {

        public abstract void Dispose();
    }
}
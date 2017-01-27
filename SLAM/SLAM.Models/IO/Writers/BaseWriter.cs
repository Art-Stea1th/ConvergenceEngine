using System;


namespace SLAM.Models.IO.Writers {

    internal abstract class BaseWriter : IDisposable {

        public abstract void Dispose();
    }
}
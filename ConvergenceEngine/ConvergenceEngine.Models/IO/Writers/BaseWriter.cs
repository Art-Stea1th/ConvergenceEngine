using System;

namespace ConvergenceEngine.Models.IO.Writers {

    internal abstract class BaseWriter : IDisposable {
        public abstract void Dispose();
    }
}
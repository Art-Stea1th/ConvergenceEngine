using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ConvergenceEngine.ViewModels {

    public abstract class ViewModelBase : INotifyPropertyChanged, IDisposable {

        protected ViewModelBase() { }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void Set<T>(ref T oldValue, T newValue, [CallerMemberName] string propertyName = null) {
            oldValue = newValue;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Dispose() {
            OnDispose();
        }

        protected virtual void OnDispose() { }
    }
}
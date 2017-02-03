using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SLAM.ViewModels {

    public delegate bool CheckWindowExistsEventHandler(ViewModelBase viewModel);
    public delegate void NewWindowQueryEventHandler(ViewModelBase viewModel, ViewModelBase viewModelOwner = null);    

    public abstract class ViewModelBase : INotifyPropertyChanged, IDisposable {

        protected ViewModelBase() { }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void Set<T>(ref T oldValue, T newValue, [CallerMemberName] string propertyName = null) {
            oldValue = newValue;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event CheckWindowExistsEventHandler OnCheckWindowExists;
        public event NewWindowQueryEventHandler OnNewWindowQuery;

        protected bool WindowExists(ViewModelBase viewModel) {
            return (bool)OnCheckWindowExists?.Invoke(viewModel);
        }

        protected void CreateWindow(ViewModelBase viewModel, ViewModelBase viewModelOwner = null) {
            OnNewWindowQuery?.Invoke(viewModel, viewModelOwner);
        }        

        public void Dispose() {
            OnDispose();
        }

        protected virtual void OnDispose() { }
    }
}
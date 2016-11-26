using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace SLAM.ViewModels {

    public abstract class ViewModelBase : INotifyPropertyChanged, IDisposable {

        protected ViewModelBase() { }

        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void OnPropertyChanged([CallerMemberName]string propertyName = null) {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }        

        protected void Set<T>(ref T oldValue, T newValue, [CallerMemberName] string propertyName = null) {
            oldValue = newValue;
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected string GetMemberName<T, TValue>(Expression<Func<T, TValue>> memberAccess) {
            return ((MemberExpression)memberAccess.Body).Member.Name;
        }

        public void Dispose() {
            OnDispose();
        }

        protected virtual void OnDispose() { }
    }
}
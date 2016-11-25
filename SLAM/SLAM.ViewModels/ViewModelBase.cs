using System;
using System.ComponentModel;
using System.Linq.Expressions;

namespace SLAM.ViewModels {

    public abstract class ViewModelBase : INotifyPropertyChanged, IDisposable {

        protected ViewModelBase() { }

        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void RaisePropertyChanged(string propertyName) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) {
                handler.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
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
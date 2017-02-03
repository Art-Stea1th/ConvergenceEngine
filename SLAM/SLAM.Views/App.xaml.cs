using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;

namespace SLAM.Views {

    using ViewModels;

    public partial class App : Application {        

        private WindowManager windowManager;
        private ViewModelBase mainViewModel;

        protected override void OnStartup(StartupEventArgs e) {
            base.OnStartup(e);

            windowManager = new WindowManager();
            mainViewModel = new MainViewModel();

            windowManager.ShowWindow(mainViewModel);
        }
    }

    internal sealed class WindowManager {

        internal void ShowWindow(ViewModelBase viewModel, ViewModelBase viewModelOwner = null) {
            viewModel.OnNewWindowQuery += ShowWindow;
            viewModel.OnCheckWindowExists += WindowExists;
            CreateWindow(viewModel, viewModelOwner);
        }        

        private void CreateWindow(ViewModelBase viewModel, ViewModelBase viewModelOwner = null) {

            var windowName = GetViewNameByViewModel(viewModel);
            var windowType = FindViewByName(windowName);

            Window window = FindExistingWindowByViewModel(viewModel);

            if (window == null) {
                window = CreateWindow(windowType);
                window.Name = windowName;
                window.DataContext = viewModel;

                if (viewModelOwner != null) {
                    window.Owner = FindExistingWindowByViewModel(viewModelOwner);
                    window.ShowInTaskbar = false;
                }
                window.Show();
            }
            window.Activate();
        }

        private bool WindowExists(ViewModelBase viewModel) {
            var window = FindExistingWindowByViewModel(viewModel);
            return window != null ? true : false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private string GetViewNameByViewModel(ViewModelBase viewModel) {
            return viewModel.GetType().Name.Replace("ViewModel", "View");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Type FindViewByName(string viewName) {
            return GetType().Assembly.GetTypes().Single(v => v.IsClass && v.Name == viewName);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Window FindExistingWindowByViewModel(ViewModelBase viewModel) {
            return Application.Current.Windows.OfType<Window>().SingleOrDefault(
                w => w.Name == GetViewNameByViewModel(viewModel));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Window CreateWindow(Type windowType) {
            return (Window)Activator.CreateInstance(windowType);
        }
    }
}
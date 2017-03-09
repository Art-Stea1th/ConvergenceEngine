using System;
using System.Windows;

namespace ConvergenceEngine.Views {

    public partial class App : Application {

        public App() {

            IDisposable mainViewModel = null;

            Startup += (sender, args) => {
                MainWindow = new MainView();
                mainViewModel = MainWindow.DataContext as IDisposable;
                MainWindow.Show();
            };

            DispatcherUnhandledException += (sender, args) => { mainViewModel?.Dispose(); };
            Exit += (sender, args) => { mainViewModel?.Dispose(); };
        }
    }
}
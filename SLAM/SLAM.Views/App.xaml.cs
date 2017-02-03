using System.Windows;


namespace SLAM.Views {

    using AppWindows;
    using ViewModels.AppWindows;

    public partial class App : Application {

        protected override void OnStartup(StartupEventArgs e) {
            WindowManager.Instance.ShowWindow(new MainWindowViewModel());
            base.OnStartup(e);
        }
    }
}
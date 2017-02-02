using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;


namespace SLAM.Views {

    using ViewModels;

    public partial class App : Application {

        private List<ViewModelBase> viewmodels;

        protected override void OnStartup(StartupEventArgs e) {
            base.OnStartup(e);
            viewmodels = new List<ViewModelBase> { new MainViewModel() };
            Window main = NewWindowOnDemand(new MainViewModel());
            main.ApplyTemplate();
            main.DataContext = viewmodels[0];
            main.Show();
        }

        internal ViewModelBase GetViewModelFor(FrameworkElement view) {
            return (ViewModelBase)Activator.CreateInstance(
                typeof(ViewModelBase).Assembly.GetType(view.GetType().FullName + "Model"));
        }

        private Window NewWindowOnDemand(ViewModelBase viewmodel) {
            var viewName = viewmodel.GetType().Name.Replace("ViewModel", "View");
            var types = from viewType in typeof(App).Assembly.GetTypes()
                        where viewType.IsClass && viewType.Name == viewName
                        select viewType;
            return (Window)Activator.CreateInstance(types.Single());
        }

        private Window CreateWindow(ViewModelBase viewModel) {


            throw new NotImplementedException();
        }
    }
}
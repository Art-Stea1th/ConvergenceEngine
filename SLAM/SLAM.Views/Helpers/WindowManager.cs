using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SLAM.ViewModels.Helpers {

    public class WindowManager {

        public void ShowWindow(ViewModelBase viewModel) {
            Window w = CreateWindow(viewModel);
            w.WindowStyle = WindowStyle.ToolWindow;
            //w.Parent
            w.Show();
        }

        private Window CreateWindow(ViewModelBase viewModel) {
            var modelType = viewModel.GetType();
            //var windowTypeName = modelType.FullName;
            //windowTypeName = windowTypeName.Replace("ViewModels", "Views");
            var windowTypeName = modelType.Name;
            windowTypeName = windowTypeName.Replace("ViewModel", "");
            var windowTypes
                = from type in modelType.Assembly.GetTypes()
                  where type.IsClass && type.Name == windowTypeName
                  select type;
            return (Window)Activator.CreateInstance(windowTypes.Single());
        }
    }
}
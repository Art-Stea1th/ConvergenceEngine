using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace SLAM.ViewModels {

    using Helpers;

    public class MainViewModel : ViewModelBase {

        ViewModelBase linearDataWindowViewModel;


        private ICommand newWindowCommand;

        public ICommand NewWindow {
            get { return newWindowCommand; }
            private set { Set(ref newWindowCommand, value); }
        }

        public MainViewModel() {
            //windowManager = new WindowManager();
            linearDataWindowViewModel = new LinearDataViewModel();
            NewWindow = new RelayCommand(ExecuteNewWindowCommand, CanExecuteNewWindowCommand);
        }

        private void ExecuteNewWindowCommand(object obj) {
            MessageBox.Show("Execute NewWindowCommand Success!",
                            "NewWindowCommand",
                            MessageBoxButton.OK, MessageBoxImage.Exclamation);
            //windowManager.ShowWindow(linearDataWindowViewModel);
        }

        private bool CanExecuteNewWindowCommand(object obj) {
            return true;
        }
    }
}
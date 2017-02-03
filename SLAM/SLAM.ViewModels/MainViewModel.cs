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
            linearDataWindowViewModel = new LinearDataViewModel();
            NewWindow = new RelayCommand(ExecuteNewWindowCommand, CanExecuteNewWindowCommand);
        }

        private void ExecuteNewWindowCommand(object obj) {
            CreateWindow(linearDataWindowViewModel, this);
        }

        private bool CanExecuteNewWindowCommand(object obj) {
            return !WindowExists(linearDataWindowViewModel);
        }
    }
}
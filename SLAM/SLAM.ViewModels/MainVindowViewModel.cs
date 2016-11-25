using System.Windows;
using System.Windows.Input;

namespace SLAM.ViewModels {

    using Helpers;    

    public class MainVindowViewModel : ViewModelBase {

        private string currentFilePath;
        private ICommand exitApplicationCommand;

        public string CurrentFilePath {
            get { return currentFilePath;}
            set { currentFilePath = value; RaisePropertyChanged(GetMemberName((MainVindowViewModel vm) => vm.CurrentFilePath)); }
        }

        public ICommand ExitApplication {
            get {
                return
                    (exitApplicationCommand) ??
                    (exitApplicationCommand =
                    new RelayCommand(w => ((Window)w).Close()));
            }
        }
    }
}
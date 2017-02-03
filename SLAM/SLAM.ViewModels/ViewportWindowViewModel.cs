using SLAM.Models;


namespace SLAM.ViewModels {

    public abstract class ViewportWindowViewModel : ViewModelBase {

        private string title;

        public string Title {
            get { return title; }
            set { Set(ref title, value); }
        }
                
        public abstract void Initialize();
        public abstract void UpdateFrom(Model model);
    }
}
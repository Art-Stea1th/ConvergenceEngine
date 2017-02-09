namespace SLAM.ViewModels {

    using Models.Mapping;

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
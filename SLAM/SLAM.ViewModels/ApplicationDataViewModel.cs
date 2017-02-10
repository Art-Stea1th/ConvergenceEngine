using System.IO;

namespace SLAM.ViewModels {

    using Models.Mapping;

    public abstract class ApplicationDataViewModel : ViewModelBase {

        protected Model model;

        private string modelCurrentStateInfo;
        private bool modelReady;
        protected string currentFilePath; // <- !
        private int totalFramesCount;
        private int currentFrame;

        public string ModelCurrentStateInfo {
            get { return modelCurrentStateInfo; }
            set { Set(ref modelCurrentStateInfo, value); }
        }
        public bool ModelReady {
            get { return modelReady; }
            set { Set(ref modelReady, value); }
        }
        public string CurrentFileName {
            get { return Path.GetFileName(currentFilePath); } // <- !
            set { Set(ref currentFilePath, value); }
        }
        public int TotalFramesCount {
            get { return totalFramesCount; }
            set { Set(ref totalFramesCount, value); }
        }
        public int CurrentFrame {
            get { return currentFrame; }
            set { Set(ref currentFrame, value); model.MoveToPosition(currentFrame); }
        }

        protected virtual void InitializeData() {
            ModelCurrentStateInfo = model.CurrentStateInfo;
            ModelReady = model.Ready;
            TotalFramesCount = model.FramesCount;
        }
    }
}
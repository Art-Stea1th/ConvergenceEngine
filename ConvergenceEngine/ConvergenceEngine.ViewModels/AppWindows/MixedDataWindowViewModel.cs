using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace ConvergenceEngine.ViewModels.AppWindows {

    using Models.Mapping;

    public sealed class MixedDataWindowViewModel : ViewModelBase {

        private Map map;


        internal MixedDataWindowViewModel(Map map) {
            this.map = map;
            this.map.OnFrameUpdate += Update;
            Initialize();
        }

        public void Initialize() {

        }

        public void Update() {

        }
    }
}
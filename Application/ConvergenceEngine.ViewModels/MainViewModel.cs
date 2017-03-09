using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ConvergenceEngine.ViewModels {

    using Models.IO;
    using Models.Mapping;
    using Infrastructure.Interfaces;

    public sealed class MainViewModel : CommandsViewModel {

        private bool viewportSettingsVisible = true;       

        public bool ViewportSettingsVisible {
            get { return viewportSettingsVisible; }
            set { Set(ref viewportSettingsVisible, value); }
        }

        public MainViewModel() {
            InitializeCommands();
        }
    }
}
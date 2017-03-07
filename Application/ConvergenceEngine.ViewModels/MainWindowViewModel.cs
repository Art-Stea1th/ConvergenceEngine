using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConvergenceEngine.Infrastructure.Interfaces;

namespace ConvergenceEngine.ViewModels {


    public sealed class MainWindowViewModel : CommandsViewModel {

        private bool viewportSettingsVisible;

        public bool ViewportSettingsVisible {
            get { return viewportSettingsVisible; }
            set { Set(ref viewportSettingsVisible, value); }
        }

        public override void Update(IMap map) {
            throw new NotImplementedException();
        }
    }
}
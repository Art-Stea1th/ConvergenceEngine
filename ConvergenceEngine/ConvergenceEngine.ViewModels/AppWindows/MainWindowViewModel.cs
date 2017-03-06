using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace ConvergenceEngine.ViewModels.AppWindows {

    using Helpers;
    using Models;
    using Models.Mapping;
    using Models.Mapping.Segments;

    public sealed class MainWindowViewModel : CommandsViewModel {

        private DateTime lastTimeOfFrameUpdate;
        private TimeSpan frameUpdateLimit;

        private Map map;

        private ViewModelBase coloredDepthDataWindowViewModel;

        public MainWindowViewModel() {
            model = new Model(Update);
            Initialize();
        }

        private void Initialize() {
            lastTimeOfFrameUpdate = DateTime.Now;
            frameUpdateLimit = TimeSpan.FromMilliseconds(1000.0 / 59.94 /*29.97*/);
            InitializeData();
            CreateViewModelsForChildWindows();
            InitializeCommands();
        }

        private void CreateViewModelsForChildWindows() {
            coloredDepthDataWindowViewModel = new ColoredDepthDataWindowViewModel(model);
        }

        protected override void InitializeCommands() {
            base.InitializeCommands();

            ShowRawDataWindow = new RelayCommand(
                ex => ExecuteNewWindowCommand(coloredDepthDataWindowViewModel),
                canEx => CanExecuteNewWindowCommand(coloredDepthDataWindowViewModel));
        }

        private void Update() {

            ModelReady = model.Ready;

            if ((DateTime.Now - lastTimeOfFrameUpdate) >= frameUpdateLimit && ModelReady) {

                //CurrentSegments = map.CurrentSegments?.Select(s => (Tuple<Point, Point>)(s as Segment));
                //MapPoints = null /*model.Map.GetMapPoints()*/;

                lastTimeOfFrameUpdate = DateTime.Now;
            }            
        }
    }
}
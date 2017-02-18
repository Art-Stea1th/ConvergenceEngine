using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace SLAM.ViewModels.AppWindows {

    using Models.Mapping;
    using Helpers;

    public sealed class MainWindowViewModel : CommandsViewModel {

        private DateTime lastTimeOfFrameUpdate;
        private TimeSpan frameUpdateLimit;

        private IEnumerable<Point> mapViewportData;

        public IEnumerable<Point> MapViewportData {
            get { return mapViewportData; }
            set { Set(ref mapViewportData, value); }
        }

        private ViewModelBase coloredDepthDataWindowViewModel;
        private ViewModelBase pointsDataWindowViewModel;
        private ViewModelBase linearDataWindowViewModel;

        public MainWindowViewModel() {
            model = new Model(Update);
            Initialize();
        }

        private void Initialize() {
            lastTimeOfFrameUpdate = DateTime.Now;
            frameUpdateLimit = TimeSpan.FromMilliseconds(1000.0 / 29.97);
            InitializeData();
            CreateViewModelsForChildWindows();
            InitializeCommands();
        }

        private void CreateViewModelsForChildWindows() {
            coloredDepthDataWindowViewModel = new ColoredDepthDataWindowViewModel(model);
            pointsDataWindowViewModel = new PointsDataWindowViewModel(model);
            linearDataWindowViewModel = new LinearDataWindowViewModel(model);
        }

        protected override void InitializeCommands() {
            base.InitializeCommands();

            ShowRawDataWindow = new RelayCommand(
                ex => ExecuteNewWindowCommand(coloredDepthDataWindowViewModel),
                canEx => CanExecuteNewWindowCommand(coloredDepthDataWindowViewModel));

            ShowPointsDataWindow = new RelayCommand(
                ex => ExecuteNewWindowCommand(pointsDataWindowViewModel),
                canEx => CanExecuteNewWindowCommand(pointsDataWindowViewModel));

            ShowLinearDataWindow = new RelayCommand(
                ex => ExecuteNewWindowCommand(linearDataWindowViewModel),
                canEx => CanExecuteNewWindowCommand(linearDataWindowViewModel));
        }

        private void Update() {

            ModelReady = model.Ready;

            if ((DateTime.Now - lastTimeOfFrameUpdate) >= frameUpdateLimit && ModelReady) {
                IEnumerable<Point> mapPoints = model.Map.MapPoints;
                if (mapPoints != null) {
                    MapViewportData = mapPoints;
                    lastTimeOfFrameUpdate = DateTime.Now;
                }
            }            
        }
    }
}
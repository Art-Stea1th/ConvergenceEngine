using System;
using System.Windows;

namespace SLAM.ViewModels.AppWindows {

    using Helpers;

    public class MainWindowViewModel : ApplicationViewModel {

        private ViewportWindowViewModel coloredDepthDataWindowViewModel;
        private ViewportWindowViewModel pointsDataWindowViewModel;
        private ViewportWindowViewModel linearDataWindowViewModel;

        private Point[] mapViewportData;

        private DateTime lastTimeOfFrameUpdate;
        private TimeSpan frameUpdateLimit;

        public Point[] MapViewportData {
            get { return mapViewportData; }
            set { Set(ref mapViewportData, value); }
        }

        public MainWindowViewModel() {            
            lastTimeOfFrameUpdate = DateTime.Now;
            frameUpdateLimit = TimeSpan.FromMilliseconds(1000.0 / 29.97);
            InitializeWindows();
            InitializeViewports();            
            UpdateUI();
        }        

        private void InitializeWindows() {
            coloredDepthDataWindowViewModel = new ColoredDepthDataWindowViewModel();
            pointsDataWindowViewModel = new PointsDataWindowViewModel();
            linearDataWindowViewModel = new LinearDataWindowViewModel();

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

        protected override void InitializeViewports() {
            MapViewportData = null;
            coloredDepthDataWindowViewModel.Initialize();
            pointsDataWindowViewModel.Initialize();
            linearDataWindowViewModel.Initialize();
        }

        protected override async void UpdateViewports() {

            ModelReady = model.Ready;

            if ((DateTime.Now - lastTimeOfFrameUpdate) >= frameUpdateLimit && ModelReady) {

                model.MoveToPosition(CurrentFrame);

                MapViewportData = await model.GetActualMapFrameAsync();
                coloredDepthDataWindowViewModel.UpdateFrom(model);
                pointsDataWindowViewModel.UpdateFrom(model);
                linearDataWindowViewModel.UpdateFrom(model);

                lastTimeOfFrameUpdate = DateTime.Now;
            }
        }        
    }
}
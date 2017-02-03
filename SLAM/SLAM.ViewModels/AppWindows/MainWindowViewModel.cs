using System;
using System.Windows;

namespace SLAM.ViewModels.AppWindows {

    using Helpers;

    public class MainWindowViewModel : ApplicationViewModel {

        private ViewportWindowViewModel rawDataWindowViewModel;
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
            rawDataWindowViewModel = new RawDataWindowViewModel();
            pointsDataWindowViewModel = new PointsDataWindowViewModel();
            linearDataWindowViewModel = new LinearDataWindowViewModel();

            ShowRawDataWindow = new RelayCommand(
                ex => ExecuteNewWindowCommand(rawDataWindowViewModel),
                canEx => CanExecuteNewWindowCommand(rawDataWindowViewModel));

            ShowPointsDataWindow = new RelayCommand(
                ex => ExecuteNewWindowCommand(pointsDataWindowViewModel),
                canEx => CanExecuteNewWindowCommand(pointsDataWindowViewModel));

            ShowLinearDataWindow = new RelayCommand(
                ex => ExecuteNewWindowCommand(linearDataWindowViewModel),
                canEx => CanExecuteNewWindowCommand(linearDataWindowViewModel));
        }

        protected override void InitializeViewports() {
            MapViewportData = null;
            rawDataWindowViewModel.Initialize();
            pointsDataWindowViewModel.Initialize();
        }

        protected override async void UpdateViewports() {

            ModelReady = model.Ready;

            if ((DateTime.Now - lastTimeOfFrameUpdate) >= frameUpdateLimit && ModelReady) {

                model.MoveToPosition(CurrentFrame);

                MapViewportData = await model.GetActualMapFrameAsync();
                rawDataWindowViewModel.UpdateFrom(model);
                pointsDataWindowViewModel.UpdateFrom(model);

                lastTimeOfFrameUpdate = DateTime.Now;
            }
        }        
    }
}
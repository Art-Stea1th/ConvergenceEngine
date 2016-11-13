using Microsoft.Kinect;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace SLAM.Preview.ViewModels {

    public class MainWindowViewModel : ViewModelBase {        

        private Color[,] cellularData;
        private int spacingBetweenCells;

        private Color viewportColor;
        private int viewportResolutionX;
        private int viewportResolutionY;

        // --- Kinect ---

        private KinectSensor sensor;
        private DepthImagePixel[] depthPixels;

        #region INotifyPropertyChanged

        public Color[,] CellularData {
            get {
                return cellularData;
            }
            set {
                cellularData = value;
                OnPropertyChanged(GetMemberName((MainWindowViewModel c) => c.CellularData));
            }
        }

        public int SpacingBetweenCells {
            get {
                return spacingBetweenCells;
            }
            set {
                spacingBetweenCells = value;
                OnPropertyChanged(GetMemberName((MainWindowViewModel c) => c.SpacingBetweenCells));
            }
        }

        #endregion

        private void InitializeSensors() {

            foreach (var potentialSensor in KinectSensor.KinectSensors) {
                if (potentialSensor.Status == KinectStatus.Connected) {
                    sensor = potentialSensor;
                    break;
                }
            }            

            if (null != sensor) {

                sensor.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);
                depthPixels = new DepthImagePixel[sensor.DepthStream.FramePixelDataLength];
                sensor.DepthFrameReady += SensorDepthFrameReady;

                try {
                    sensor.Start();
                }
                catch (IOException) {
                    sensor = null;
                }
            }
        }

        private void InitializeViewport() {
            viewportResolutionX = 640;
            viewportResolutionY = 300;
            SpacingBetweenCells = 1;
            viewportColor = Color.FromRgb(24, 131, 215);
        }

        private void Initialize() {
            InitializeSensors();            
        }

        private int GetLinearIndex(int x, int y, int width) {
            return width * y + x;
        }

        private void SensorDepthFrameReady(object sender, DepthImageFrameReadyEventArgs e) {

            using (DepthImageFrame depthFrame = e.OpenDepthImageFrame()) {

                if (depthFrame != null) {

                    depthFrame.CopyDepthImagePixelDataTo(depthPixels);

                    Color[,] colors = new Color[viewportResolutionY, viewportResolutionX];

                    for (int x = 0; x < viewportResolutionX; x++) {

                        int middleLineIndex = depthFrame.Height / 2;
                        int linearIndex = GetLinearIndex(x, middleLineIndex, depthFrame.Width);

                        int centimeterDepthAsY = depthPixels[linearIndex].Depth / 10;

                        if (centimeterDepthAsY > 0 && centimeterDepthAsY < colors.GetLength(0)) {
                            colors[viewportResolutionY - centimeterDepthAsY - 1, viewportResolutionX - x - 1] = viewportColor;
                        }                        
                    }

                    CellularData = colors;
                }
            }
        }

        public MainWindowViewModel() {
            InitializeViewport();
            Initialize();
        }
    }
}
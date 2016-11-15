using Microsoft.Kinect;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace SLAM.Preview.ViewModels {

    using Model;
    using System.Numerics;

    public class MainWindowViewModel : ViewModelBase {

        // --- Viewport ---

        private Color[,] cellularData;
        private int spacingBetweenCells;

        private Color viewportColor;
        private int viewportResolutionX;
        private int viewportResolutionY;

        private Random random;

        // --- Kinect ---

        private KinectSensor sensor;
        private DepthImagePixel[] depthPixels;

        private int minDepth, maxDepth;

        // --- Math ---

        //private static readonly double HorizontalTanA = Math.Tan(28.5 * Math.PI / 180);
        private static readonly double pixelAngle = 57.0 / 640;

        private static readonly double hFOV = 58.5;

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

        public int ViewportResolutionX {
            get {
                return viewportResolutionX;
            }
            set {
                viewportResolutionX = value;
                OnPropertyChanged(GetMemberName((MainWindowViewModel c) => c.ViewportResolutionX));
            }
        }

        public int ViewportResolutionY {
            get {
                return viewportResolutionY;
            }
            set {
                viewportResolutionY = value;
                OnPropertyChanged(GetMemberName((MainWindowViewModel c) => c.ViewportResolutionY));
            }
        }

        #endregion

        //private void SensorDepthFrameReady(object sender, DepthImageFrameReadyEventArgs e) {

        //    using (DepthImageFrame depthFrame = e.OpenDepthImageFrame()) {

        //        if (depthFrame != null) {

        //            depthFrame.CopyDepthImagePixelDataTo(depthPixels);

        //            Color[,] colors = new Color[viewportResolutionY, viewportResolutionX = depthFrame.Width];

        //            for (int x = 0; x < viewportResolutionX; x++) {

        //                int middleLineIndex = depthFrame.Height / 2;
        //                int linearIndex = GetLinearIndex(x, middleLineIndex, depthFrame.Width);

        //                int millimeterDepthAsY = depthPixels[linearIndex].Depth;

        //                int realY = millimeterDepthAsY / 10;

        //                if (realY > 0 && realY < colors.GetLength(0)) {
        //                    colors[viewportResolutionY - realY - 1, viewportResolutionX - x - 1] = viewportColor;
        //                }
        //            }

        //            CellularData = colors;
        //        }
        //    }
        //}
        private void SensorDepthFrameReady(object sender, DepthImageFrameReadyEventArgs e) {


            using (DepthImageFrame depthFrame = e.OpenDepthImageFrame()) {

                //GC.Collect();
                Color[,] viewportSurface = new Color[viewportResolutionY, viewportResolutionX];

                if (depthFrame != null) {


                    depthFrame.CopyDepthImagePixelDataTo(depthPixels);

                    minDepth = depthFrame.MinDepth;
                    maxDepth = depthFrame.MaxDepth;

                    for (int i = 0; i < depthFrame.Width; i++) {

                        int middleLineIndex = depthFrame.Height / 2;
                        int linearIndex = GetLinearIndex(i, middleLineIndex, depthFrame.Width);

                        if (!depthPixels[linearIndex].IsKnownDepth) {
                            continue;
                        }

                        int currentPixelmillimeterDepth = depthPixels[linearIndex].Depth;

                        double currentPixelAngle = ((sensor.DepthStream.NominalHorizontalFieldOfView * 0.5) / depthFrame.Width) * i;

                        double x, y;
                        PolarToRectangular(currentPixelmillimeterDepth, currentPixelAngle, out x, out y);

                        int xIndex = (int)(x * 0.1)/* + 500*/;
                        int yIndex = (int)(y * 0.1)/* + 500*/;

                        //if (xIndex >= 0 && xIndex < viewportResolutionX && yIndex >= 0 && yIndex < viewportResolutionY) {
                        viewportSurface[yIndex, xIndex] = viewportColor;
                        //}
                    }
                    CellularData = viewportSurface;
                }
            }
        }

        private void PolarToRectangular(double radius, double angle, out double x, out double y) {
            x = radius * Math.Cos(angle * Math.PI / 180.0);
            y = radius * Math.Sin(angle * Math.PI / 180.0);
        }

        private int GetLinearIndex(int x, int y, int width) {
            return width * y + x;
        }

        public MainWindowViewModel() {
            Initialize();
        }

        private void Initialize() {
            InitializeViewport();
            InitializeSensor();
            ConfigureSensor();
            StartSensor();
        }

        private void InitializeViewport() {
            ViewportResolutionX = 601;
            ViewportResolutionY = 601;
            SpacingBetweenCells = 0;
            random = new Random();
            viewportColor = Color.FromRgb(24, 131, 215);
        }

        private void InitializeSensor() {

            foreach (var potentialSensor in KinectSensor.KinectSensors) {
                if (potentialSensor.Status == KinectStatus.Connected) {
                    sensor = potentialSensor;
                    break;
                }
            }
        }

        private void ConfigureSensor() {

            if (null != sensor) {

                sensor.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);

                depthPixels = new DepthImagePixel[sensor.DepthStream.FramePixelDataLength];
                sensor.DepthFrameReady += SensorDepthFrameReady;
            }
        }

        private void StartSensor() {
            try { sensor.Start(); }
            catch (IOException) { sensor = null; }
        }
    }
}
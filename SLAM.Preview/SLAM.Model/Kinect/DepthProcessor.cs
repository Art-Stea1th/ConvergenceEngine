using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace SLAM.Model.Kinect {

    public delegate void SensorLineReadyEvent(Vector3D[] nextLine);

    public class DepthProcessor {

        public event SensorLineReadyEvent OnSensorLineReady;

        private KinectSensor sensor;
        private DepthImagePixel[] depthPixels;

        private int minDepth, maxDepth;

        private void Initialize() {
            InitializeSensor();
            ConfigureSensor();
            StartSensor();
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

        private void SensorDepthFrameReady(object sender, DepthImageFrameReadyEventArgs e) {


            using (DepthImageFrame depthFrame = e.OpenDepthImageFrame()) {                

                if (depthFrame != null) {

                    Vector3D[] resultRectangularCoordinatesData = null;

                    OnSensorLineReady?.Invoke(resultRectangularCoordinatesData);
                    
                }
            }
        }
    }
}
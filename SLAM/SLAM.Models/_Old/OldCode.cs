/*
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
           
*/
        //private void SensorDepthFrameReady(object sender, DepthImageFrameReadyEventArgs e) {


        //    using (DepthImageFrame depthFrame = e.OpenDepthImageFrame()) {

        //        //GC.Collect();
        //        Color[,] viewportSurface = new Color[viewportResolutionY, viewportResolutionX];

        //        if (depthFrame != null) {


        //            depthFrame.CopyDepthImagePixelDataTo(depthPixels);

        //            minDepth = depthFrame.MinDepth;
        //            maxDepth = depthFrame.MaxDepth;

        //            for (int i = 0; i < depthFrame.Width; i++) {

        //                int middleLineIndex = depthFrame.Height / 2;
        //                int linearIndex = GetLinearIndex(i, middleLineIndex, depthFrame.Width);

        //                if (!depthPixels[linearIndex].IsKnownDepth) {
        //                    int tmp = depthPixels[linearIndex].Depth;
        //                    continue;
        //                }

        //                int currentPixelmillimeterDepth = depthPixels[linearIndex].Depth;

        //                double currentPixelAngle = ((sensor.DepthStream.NominalHorizontalFieldOfView * 0.5) / depthFrame.Width) * i;

        //                double x, y;
        //                //PolarToRectangular(currentPixelmillimeterDepth, currentPixelAngle, out x, out y);
        //                //PolarToRectangular(depthFrame.Width, i, currentPixelmillimeterDepth, currentPixelAngle, out x, out y);
        //                PerspectiveToRectangle(i, currentPixelmillimeterDepth, out x, out y); // !!
        //                //PerspectiveToRectangle(i * depthFrame.Width / 2, currentPixelmillimeterDepth, out x, out y);

        //                int xIndex = (int)(x * 0.1)/* + 500*/;
        //                int yIndex = (int)(y * 0.1)/* + 500*/;

        //                //int xIndex = (int)(x * depthFrame.Width / 2 * 0.1)/* + 500*/;
        //                //int yIndex = (int)(y * depthFrame.Width / 2 * 0.1)/* + 500*/;

        //                if (xIndex >= 0 && xIndex < viewportResolutionX && yIndex >= 0 && yIndex < viewportResolutionY) {
        //                    viewportSurface[yIndex, xIndex] = viewportColor;
        //                }
        //            }
        //            CellularData = viewportSurface;
        //        }
        //    }
        //}

/*
        private void PolarToRectangular(double radius, double angle, out double x, out double y) {
            x = radius * Math.Cos(angle * Math.PI / 180.0);
            y = radius * Math.Sin(angle * Math.PI / 180.0);
        }

        private void PolarToRectangular(int widthPixels, int pixelIndex, double radius, double angle, out double x, out double y) {

            //double correctedRadius = (radius + Math.Abs(widthPixels / 2.0 - pixelIndex) * 1.0);
            double correctedRadius = radius + Math.Sin(angle * Math.PI / 180.0);

            x = correctedRadius * Math.Cos(angle * Math.PI / 180.0);
            y = correctedRadius * Math.Sin(angle * Math.PI / 180.0);
        }

        private void PerspectiveToRectangle(double pixelIndex, double depth, out double x, out double y) {
            x = pixelIndex * 0.00780 * (1 + depth / 4.73); //0.0078 мм/пикс - размер пикселя //4,73 мм - фокусное расстояние
            y = depth;
        }
        //private void PerspectiveToRectangle(double pixelIndex, double depth, out double x, out double y) {
        //    x = pixelIndex * 0.00780 * (1 + depth / 4.73); //0.0078 мм/пикс - размер пикселя //4,73 мм - фокусное расстояние
        //    y = depth;
        //}

        private int GetLinearIndex(int x, int y, int width) {
            return width * y + x;
        }        
    }
}
*/
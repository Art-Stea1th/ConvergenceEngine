using System;
using System.Windows.Media.Media3D;

namespace SLAM.Models {

    // performance critical

    internal sealed class DataConverter {

        private DepthFrameSequenceInfo frameInfo;

        public DataConverter(DepthFrameSequenceInfo frameInfo) {
            this.frameInfo = frameInfo;
        }

        public void HorizontalMirror(byte[] frame, int width, int height) { // and swap hi & low bytes for each Int16\short depth

            for (int y = 0; y < height; ++y) {
                for (int x = 0; x < width / 2; ++x) {

                    int linearIndexNormal = y * width + x;
                    int linearIndexInverse = y * width + (width - 1 - x);

                    byte tmp = frame[linearIndexNormal];
                    frame[linearIndexNormal] = frame[linearIndexInverse];
                    frame[linearIndexInverse] = tmp;
                }
            }
        }

        public void ConvertRawToViewportFullFrame(byte[] inRawFrame, byte[] outViewportFullFrameBuffer) {

            HorizontalMirror(inRawFrame, frameInfo.Width * sizeof(short), frameInfo.Height);

            int fullDepth = frameInfo.MaxDepth - frameInfo.MinDepth;
            double intencityStep = 192.0 / fullDepth;

            int colorPixelIndex = 0;
            for (int i = 0; i < frameInfo.Length; ++i) {

                short depth = inRawFrame[i * sizeof(short)];
                short depthLowByte = inRawFrame[i * sizeof(short) + 1];

                depth <<= 8; depth |= depthLowByte; // <-- depth short construct                
                depth >>= 3;                        // <-- remove 3 unused low bits

                byte intensity = (byte)(255 - (depth * intencityStep));

                if (depth < frameInfo.MinDepth) {
                    outViewportFullFrameBuffer[colorPixelIndex++] = 192;
                    outViewportFullFrameBuffer[colorPixelIndex++] = 128;
                    outViewportFullFrameBuffer[colorPixelIndex++] = 0;
                }
                else if (depth > frameInfo.MaxDepth) {
                    outViewportFullFrameBuffer[colorPixelIndex++] = 32;
                    outViewportFullFrameBuffer[colorPixelIndex++] = 0;
                    outViewportFullFrameBuffer[colorPixelIndex++] = 0;
                }
                else {
                    outViewportFullFrameBuffer[colorPixelIndex++] = intensity;
                    outViewportFullFrameBuffer[colorPixelIndex++] = intensity;
                    outViewportFullFrameBuffer[colorPixelIndex++] = intensity;
                }
                ++colorPixelIndex;
            }
        }

        public void ConvertRawToViewportCurveFrame(byte[] inRawFrame, byte[] outViewportFullFrameBuffer) {

            HorizontalMirror(inRawFrame, frameInfo.Width * sizeof(short), frameInfo.Height);            

            int middleLineIndex = frameInfo.Height / 2;
            int startIndex = GetLinearIndex(0, middleLineIndex, frameInfo.Width);
            int endIndex = GetLinearIndex(frameInfo.Width - 1, middleLineIndex, frameInfo.Width);

            int fullDepth = frameInfo.MaxDepth - frameInfo.MinDepth;

            for (int i = startIndex; i < endIndex; ++i) {

                short depth = inRawFrame[i * sizeof(short)];
                short depthLowByte = inRawFrame[i * sizeof(short) + 1];

                depth <<= 8; depth |= depthLowByte; // <-- depth short construct                
                depth >>= 3;                        // <-- remove 3 unused low bits

                if (depth < frameInfo.MinDepth || depth > frameInfo.MaxDepth) { continue; }

                // --- Math ---

                double x = i - startIndex;  // 0   - 639
                double y = middleLineIndex; // 240 - 240
                double z = depth;           // 800 - 4000

                double angleBetweenRays = ((frameInfo.NominalHorizontalFOV * 0.5) / frameInfo.Width) * x;

                double resultX, resultY;
                PolarToRectangular(depth, angleBetweenRays, out resultX, out resultY);
                //PerspectiveToRectangle(x, angleBetweenRays, out resultX, out resultY);

                // --- EndMath ---

                int imageX = (int)(resultX * 0.1);
                int imageY = (int)(resultY * 0.1);

                int resultLinearIndex = GetLinearIndex(imageX * sizeof(int), imageY, frameInfo.Width * sizeof(int));

                outViewportFullFrameBuffer[resultLinearIndex++] = 128;
                outViewportFullFrameBuffer[resultLinearIndex++] = 192;
                outViewportFullFrameBuffer[resultLinearIndex++] = 255;
            }
        }

        private void PolarToRectangular(double radius, double angle, out double x, out double y) {
            x = radius * Math.Cos(angle * Math.PI / 180.0);
            y = radius * Math.Sin(angle * Math.PI / 180.0);
        }

        //private void PerspectiveToRectangle(double pixelIndex, double depth, out double x, out double y) {
        //    x = pixelIndex * 0.00780 * (1 + depth / 4.73); //0.0078 мм/пикс - размер пикселя //4,73 мм - фокусное расстояние
        //    y = depth;
        //}

        private int GetLinearIndex(int x, int y, int width) {
            return width * y + x;
        }

        // --- Math Magic ---

        private double AngleBetweenRays(double x1, double y1, double z1, double x2, double y2, double z2) {
            double dotProduct = x1 * x2 + y1 * y2 + x1 * z2;
            double magnitude1 = MagnitudeOfRay(x1, y1, z1);
            double magnitude2 = MagnitudeOfRay(x2, y2, z2);
            return Math.Acos(dotProduct / (magnitude1 * magnitude2));
        }

        private double MagnitudeOfRay(double x, double y, double z) {
            return Math.Sqrt(Math.Pow(x, 2.0) + Math.Pow(y, 2.0) + Math.Pow(z, 2.0));
        }        

        private double AngleBetweenRays(Point3D ray1, Point3D ray2) {
            double dotProduct = ray1.X * ray2.X + ray1.Y * ray2.Y + ray1.X * ray2.Z;
            double magnitude1 = MagnitudeOfRay(ray1);
            double magnitude2 = MagnitudeOfRay(ray2); ;
            return Math.Acos(dotProduct / (magnitude1 * magnitude2));
        }

        private double MagnitudeOfRay(Point3D ray) {
            return Math.Sqrt(Math.Pow(ray.X, 2.0) + Math.Pow(ray.Y, 2.0) + Math.Pow(ray.Z, 2.0));
        }
    }
}
using System;


namespace SLAM.Models.Converters {

    // Performance critical
    internal sealed class RapidConverter : IDataConverter {

        public DepthFrameSequenceInfo FrameInfo { get; private set; }

        public RapidConverter(DepthFrameSequenceInfo frameInfo) {
            FrameInfo = frameInfo;
        }

        public void RawFrameToFullFrame(byte[] inRawFrame, byte[] outViewportFullFrameBuffer) {

            HorizontalMirror(inRawFrame);

            int fullDepth = FrameInfo.MaxDepth - FrameInfo.MinDepth;
            double intencityStep = 192.0 / fullDepth;

            int colorPixelIndex = 0;
            for (int i = 0; i < FrameInfo.Length; ++i) {

                short depth = inRawFrame[i * sizeof(short)];
                short depthLowByte = inRawFrame[i * sizeof(short) + 1];

                depth <<= 8; depth |= depthLowByte; // <-- depth short construct                
                depth >>= 3;                        // <-- remove 3 unused low bits

                if (depth < FrameInfo.MinDepth) {
                    outViewportFullFrameBuffer[colorPixelIndex++] = 192;
                    outViewportFullFrameBuffer[colorPixelIndex++] = 128;
                    outViewportFullFrameBuffer[colorPixelIndex++] = 0;
                }
                else if (depth > FrameInfo.MaxDepth) {
                    outViewportFullFrameBuffer[colorPixelIndex++] = 32;
                    outViewportFullFrameBuffer[colorPixelIndex++] = 0;
                    outViewportFullFrameBuffer[colorPixelIndex++] = 0;
                }
                else {
                    byte intensity = (byte)(255 - (depth * intencityStep));

                    outViewportFullFrameBuffer[colorPixelIndex++] = intensity;
                    outViewportFullFrameBuffer[colorPixelIndex++] = intensity;
                    outViewportFullFrameBuffer[colorPixelIndex++] = intensity;
                }
                ++colorPixelIndex;
            }
        }

        public void RawFrameToCurveFrame(byte[] inRawFrame, byte[] outViewportFullFrameBuffer) {

            HorizontalMirror(inRawFrame);

            int middleLineIndex = FrameInfo.Height / 2;
            int startIndex = GetLinearIndex(0, middleLineIndex, FrameInfo.Width);
            int endIndex = GetLinearIndex(FrameInfo.Width - 1, middleLineIndex, FrameInfo.Width);

            int fullDepth = FrameInfo.MaxDepth - FrameInfo.MinDepth;

            for (int i = startIndex; i < endIndex; ++i) {

                short depth = inRawFrame[i * sizeof(short)];
                short depthLowByte = inRawFrame[i * sizeof(short) + 1];

                depth <<= 8; depth |= depthLowByte; // <-- depth short construct                
                depth >>= 3;                        // <-- remove 3 unused low bits

                if (depth < FrameInfo.MinDepth || depth > FrameInfo.MaxDepth) { continue; }

                // --- Math ---

                double x = i - startIndex;  // 0   - 639
                double y = middleLineIndex; // 240 - 240
                double z = depth;           // 800 - 4000

                double deltaAngleBetweenRays = ((FrameInfo.NominalHorizontalFOV * 0.5) / FrameInfo.Width) * x;

                double resultX, resultY;
                PolarToRectangular(depth, deltaAngleBetweenRays, out resultX, out resultY);
                //PerspectiveToRectangle(x, y, z, out resultX, out resultY);
                // --- EndMath ---

                int imageX = (int)(resultX * 0.1);
                int imageY = (int)(resultY * 0.1);

                int resultLinearIndex = GetLinearIndex(imageX * sizeof(int), imageY, FrameInfo.Width * sizeof(int));

                if (resultLinearIndex >= 0) {
                    outViewportFullFrameBuffer[resultLinearIndex++] = 128;
                    outViewportFullFrameBuffer[resultLinearIndex++] = 192;
                    outViewportFullFrameBuffer[resultLinearIndex++] = 255;
                }
            }
        }

        // Horizontal Mirror Raw Frame and swap hi & low bytes for each Int16\short depth
        private void HorizontalMirror(byte[] frame) {

            int width = FrameInfo.Width * sizeof(short);
            int height = FrameInfo.Height;

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

        private void PolarToRectangular(double radius, double angle, out double x, out double y) {
            x = radius * Math.Cos(angle * Math.PI / 180.0);
            y = radius * Math.Sin(angle * Math.PI / 180.0);
        }

        private void PerspectiveToRectangle(double x, double y, double z, out double rectX, out double rectY) {

            double cx = 339.307;
            double cy = 242.739;
            double fx = 1 / 594.214;
            double fy = 1 / 591.0405;
            rectX = (x - cx) * z * fx;
            //rectY = (y - cy) * z * fy;
            rectY = z;
        }

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
    }
}
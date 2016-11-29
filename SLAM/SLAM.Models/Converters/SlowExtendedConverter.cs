using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;


namespace SLAM.Models.Converters {

    // Comfort critical, for Math-Magic Experiments optimized =)
    internal sealed class SlowExtendedConverter : IDataConverter {

        public DepthFrameSequenceInfo FrameInfo { get; private set; }

        internal SlowExtendedConverter(DepthFrameSequenceInfo frameInfo) {
            FrameInfo = frameInfo;
        }

        public void RawFrameToFullFrame(byte[] inRawFrame, byte[] outViewportFullFrameBuffer) {

            int screenWidth  = FrameInfo.Width * sizeof(int);
            int screenHeight = FrameInfo.Height;

            foreach (var colorPoint in FullFrameColorPixelsFromRawFrame(inRawFrame)) {

                int screenX = (int)colorPoint.Item1.X * sizeof(int);
                int screenY = (int)colorPoint.Item1.Y;

                int linearIndex = LinearIndex(screenX, screenY, screenWidth, screenHeight);

                Color color = colorPoint.Item2;

                foreach (byte colorByte in SubstractIntBgraColorFrom(color)) {
                    outViewportFullFrameBuffer[linearIndex++] = colorByte;
                }
            }
        }

        public void RawFrameToCurveFrame(byte[] inRawFrame, byte[] outViewportFullFrameBuffer) {

            double minZDepth = FrameInfo.MinDepth;
            double maxZDepth = FrameInfo.MaxDepth;
            double horizontalFOV = FrameInfo.NominalHorizontalFOV * 0.5;
            double width = FrameInfo.Width;
            double height = FrameInfo.Height;
            int middleLineIndex = FrameInfo.Height / 2;

            foreach (Vector3D nextDepthPoint in DepthPointsFromRawFrame(inRawFrame)) {

                double x = nextDepthPoint.X;
                double y = nextDepthPoint.Y;
                double z = nextDepthPoint.Z;

                if (y == middleLineIndex && z >= minZDepth && z <= maxZDepth) {

                    double deltaAngleBetweenRays = (horizontalFOV / width) * x;

                    double resultX, resultY;
                    PolarToRectangular(z, deltaAngleBetweenRays, out resultX, out resultY);

                    WriteToScreen(outViewportFullFrameBuffer, resultX * 0.1, resultY * 0.1);
                }
            }
        }

        private void PolarToRectangular(double radius, double angle, out double x, out double y) {
            x = radius * Math.Cos(angle * Math.PI / 180.0);
            y = radius * Math.Sin(angle * Math.PI / 180.0);
        }

        private void WriteToScreen(byte[] outViewportFullFrameBuffer, double imageX, double imageY) {

            int screenWidth  = FrameInfo.Width * sizeof(int);
            int screenHeight = FrameInfo.Height;
            int screenX = (int)imageX * sizeof(int);
            int screenY = (int)imageY;
            int linearIndex = LinearIndex(screenX, screenY, screenWidth, screenHeight);

            foreach (byte colorComponent in SubstractIntBgraColorFrom(Color.FromArgb(255, 255, 192, 128))) {
                outViewportFullFrameBuffer[linearIndex++] = colorComponent;
            }
        }        

        private int LinearIndex(double x, double y, double width, double height) {
            return (int)Math.Round(y * width + x);
        }

        private IEnumerable<byte> SubstractIntBgraColorFrom(Color color) {
            yield return color.B;
            yield return color.G;
            yield return color.R;
            yield return color.A;
        }

        private IEnumerable<Tuple<Vector, Color>> FullFrameColorPixelsFromRawFrame(byte[] rawFrame) {

            int fullDepth = FrameInfo.MaxDepth - FrameInfo.MinDepth;
            double intencityStep = 192.0 / fullDepth;

            foreach (Vector3D nextDepthPoint in DepthPointsFromRawFrame(rawFrame)) {
                if (nextDepthPoint.Z < FrameInfo.MinDepth) {
                    yield return new Tuple<Vector, Color>(
                            new Vector(nextDepthPoint.X, nextDepthPoint.Y),
                            Color.FromArgb(255, 0, 128, 192));
                }
                else if (nextDepthPoint.Z > FrameInfo.MaxDepth) {
                    yield return new Tuple<Vector, Color>(
                            new Vector(nextDepthPoint.X, nextDepthPoint.Y),
                            Color.FromArgb(255, 0, 0, 32));
                }
                else {
                    byte intensity = (byte)(255 - (nextDepthPoint.Z * intencityStep));
                    yield return new Tuple<Vector, Color>(
                            new Vector(nextDepthPoint.X, nextDepthPoint.Y),
                            Color.FromArgb(255, intensity, intensity, intensity));
                }
            }
        }

        private IEnumerable<Vector3D> DepthPointsFromRawFrame(byte[] rawFrame) {

            int x = 0, y = 0;

            foreach (short nextPixel in DepthPixelsFromFawFrameBytes(rawFrame)) {
                yield return new Vector3D(x, y, nextPixel);
                ++x;
                if (x == FrameInfo.Width) {
                    x = 0; ++y;
                }
            }
        }

        private IEnumerable<short> DepthPixelsFromFawFrameBytes(byte[] rawFrame) {

            short depth = 0;
            bool isHighByte = true;

            foreach (byte nextByte in BytesHorizontalMirrorFromRawFrame(rawFrame)) {
                if (isHighByte) {
                    depth = nextByte; depth <<= 8;
                }
                else {
                    short lowByte = nextByte;
                    depth |= lowByte; depth >>= 3;
                    yield return depth;
                }
                isHighByte = !isHighByte;
            }
        }

        // Horizontal Mirror Raw Frame and swap hi & low bytes for each Int16\short depth
        private IEnumerable<byte> BytesHorizontalMirrorFromRawFrame(byte[] rawFrame) {

            int width = FrameInfo.Width * sizeof(short);
            int height = FrameInfo.Height;

            for (int y = 0; y < height; ++y) {
                for (int x = 0; x < width; ++x) {
                    int linearIndexInverse = y * width + (width - 1 - x);
                    yield return rawFrame[linearIndexInverse];
                }
            }
        }

        // --- Math Magic ---
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
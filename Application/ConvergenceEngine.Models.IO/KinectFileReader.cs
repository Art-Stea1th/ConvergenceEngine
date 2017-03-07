using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;

namespace ConvergenceEngine.Models.IO {

    using Infrastructure.Interfaces;

    public sealed class KinectFileReader : IDataProvider {

        private readonly SequenceInfo sequenceInfo;

        private ConcurrentQueue<Tuple<Point[], short[]>> frames; // ~ 610 kb
        private const int bufferLimit = 64;          // 610 * 64 ~ 38.125 mb

        private DateTime nextFrameRedyInvokeLast;
        private TimeSpan nextFrameRedyInvokeInterval;
        private double fps;
        private bool IsStarted, fillbuffersStarted, generateSequenceStarted;

        public event Action<IEnumerable<Point>> OnNextDepthLineReady;
        public event Action<IEnumerable<short>> OnNextFullFrameReady;

        public int FrameWidth { get { return sequenceInfo.Width; } }
        public int FrameHeight { get { return sequenceInfo.Height; } }

        public int MinDepth { get { return sequenceInfo.MinDepth; } }
        public int MaxDepth { get { return sequenceInfo.MaxDepth; } }

        public double FPS { get { return fps; } set { fps = LimitedValue(value, 1.0, 60.0); } }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private double LimitedValue(double value, double min, double max) {
            return value < min ? min : value > max ? max : value;
        }

        public static KinectFileReader CreateReader(string fileName) {
            var sequenceInfo = SequenceInfo.Read(fileName);
            if (sequenceInfo.IsValid) {
                return new KinectFileReader(sequenceInfo);
            }
            return null;
        }

        private KinectFileReader(SequenceInfo sequenceInfo) {
            this.sequenceInfo = sequenceInfo;
            OnCreate();
        }

        private void OnCreate() {
            Tuple<Point[], short[]> firstFrame = null;

            using (var stream = new FileStream(sequenceInfo.FileName, FileMode.Open)) {
                using (var reader = new BinaryReader(stream)) {
                    FillBuffers(reader);
                    frames.TryDequeue(out firstFrame);
                    NextFrameRedyInvoke(firstFrame.Item1, firstFrame.Item2);
                }
            }
        }

        public void Start() {
            if (!IsStarted) {
                IsStarted = true;
                new Thread(() => FillBufferProcess()).Start();
                new Thread(() => GenerateSequenceProcess()).Start();
            }
        }

        public void Stop() {
            while (fillbuffersStarted || generateSequenceStarted) {
                IsStarted = false;
                Thread.Sleep(10);
            }
        }

        private void FillBufferProcess() {

            fillbuffersStarted = true;

            using (var stream = new FileStream(sequenceInfo.FileName, FileMode.Open)) {
                using (var reader = new BinaryReader(stream)) {
                    while (IsStarted) {
                        FillBuffers(reader);
                        Thread.Sleep(10);
                    }
                }
            }
            fillbuffersStarted = false;
        }

        private void FillBuffers(BinaryReader reader) {

            int bytesPerFrame = sequenceInfo.BytesPerFrame;
            byte[] nextRawBuffer = new byte[bytesPerFrame];

            while (frames.Count < bufferLimit && reader.Read(nextRawBuffer, 0, bytesPerFrame) == bytesPerFrame) {
                HorizontalMirror(nextRawBuffer);
                short[] nextDepthFrame = DepthsFrameFrom(nextRawBuffer);
                Point[] nextDepthLine = DepthLineFrom(nextDepthFrame);
                frames.Enqueue(new Tuple<Point[], short[]>(nextDepthLine, nextDepthFrame));
            }
        }

        private void GenerateSequenceProcess() {

            generateSequenceStarted = true;

            Tuple<Point[], short[]> nextFrame = null;

            while (IsStarted) {
                if (DateTime.Now >= nextFrameRedyInvokeLast + nextFrameRedyInvokeInterval) {
                    while (IsStarted && !frames.TryDequeue(out nextFrame)) {
                        Thread.Sleep(nextFrameRedyInvokeInterval.Milliseconds / 10);
                    }
                    NextFrameRedyInvoke(nextFrame.Item1, nextFrame.Item2);
                }
                Thread.Sleep(nextFrameRedyInvokeInterval.Milliseconds / 10);
            }
            generateSequenceStarted = false;
        }

        private void NextFrameRedyInvoke(Point[] nextLine, short[] nextFrame) {
            OnNextDepthLineReady?.Invoke(nextLine);
            OnNextFullFrameReady?.Invoke(nextFrame);
            nextFrameRedyInvokeLast = DateTime.Now;
            nextFrameRedyInvokeInterval = TimeSpan.FromSeconds(1.0 / fps);
        }

        private void HorizontalMirror(byte[] frame) {

            int width  = FrameWidth * sizeof(short);
            int height = FrameHeight;

            for (int y = 0; y < height; ++y) {
                for (int x = 0; x < width / 2; ++x) {

                    int linearIndexNormal = GetLinearIndex(x, y, width);
                    int linearIndexInverse = GetLinearIndex((width - 1 - x), y, width);

                    byte tmp = frame[linearIndexNormal];
                    frame[linearIndexNormal] = frame[linearIndexInverse];
                    frame[linearIndexInverse] = tmp;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int GetLinearIndex(int x, int y, int width) {
            return width * y + x;
        }

        private short[] DepthsFrameFrom(byte[] rawFrame) {

            short[] depthFrame = new short[rawFrame.Length * sizeof(short)];
            short nextDepth = 0;

            for (int i = 0; i < rawFrame.Length; i += 2) {
                nextDepth = rawFrame[i];
                nextDepth <<= 8;
                nextDepth |= (short)rawFrame[i + 1]; // <-- depth short construct
                nextDepth <<= 3;                     // <-- remove 3 unused low bits & return
                depthFrame[i / 2] = nextDepth;
            }
            return depthFrame;
        }

        private Point[] DepthLineFrom(short[] depthFrame) {

            Point[] depthLine = new Point[FrameWidth];

            int y = FrameHeight / 2;
            int offset = FrameWidth * (y - 1);

            for (int x = 0; x < FrameWidth; ++x) {
                depthLine[x] = PerspectiveToRectangle(x, y, depthFrame[offset + x]);
            }
            return depthLine;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Point PerspectiveToRectangle(double x, double y, double z) {
            return new Point((x - 320.0) * (0.003501 * 0.5) * (z * 0.1), z * 0.1);
        }

        public void Dispose() {
            Stop();
        }
    }
}